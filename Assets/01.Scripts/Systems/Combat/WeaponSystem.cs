using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System;
using Game.Core;
using Game.Models;
using Game.Services;
using Game.Data;
using Game.Core.Event;
using R3;

namespace Game.Systems {
    /// <summary>
    /// 무기 시스템 - 무기 공격 로직 처리
    /// </summary>
    public class WeaponSystem {
        // DI 주입
        [Inject] private readonly ISkillDataService _skillDataService;
        [Inject] private readonly EquipSystem _equipSystem;
        [Inject] private readonly CombatModel _combatModel;
        [Inject] private readonly CombatSystem _combatSystem;

        /// <summary>
        /// 무기 공격 실행
        /// </summary>
        public void PerformAttack(WeaponComponent weapon) {
            if (!weapon.IsInitialized || weapon.IsAttacking) {
                GameDebug.LogWarning($"공격 불가: Initialized={weapon.IsInitialized}, Attacking={weapon.IsAttacking}");
                return;
            }

            weapon.SetAttacking(true);

            // 범위 내 타겟 찾기
            var targets = FindTargetsInRange(weapon);

            // 각 타겟에 대해 데미지 처리
            foreach (var target in targets) {
                ProcessAttackTarget(weapon, target);
            }

            GameDebug.Log($"무기 공격 실행: {weapon.SkillName}, 타겟 수: {targets.Count}, 공격력: {weapon.GetFinalAttack()}");

            // 공격 완료
            weapon.SetAttacking(false);
        }

        /// <summary>
        /// 범위 내 타겟 찾기
        /// </summary>
        public List<Collider2D> FindTargetsInRange(WeaponComponent weapon) {
            var targets = new List<Collider2D>();

            if (weapon.SkillData == null || weapon.AttackPoint == null) return targets;

            Vector2 attackPosition = weapon.AttackPoint.position;

            switch (weapon.SkillData.RangeType) {
                case SkillRangeType.Circle:
                targets.AddRange(FindCircleTargets(weapon, attackPosition));
                break;
                case SkillRangeType.Sector:
                targets.AddRange(FindSectorTargets(weapon, attackPosition));
                break;
                case SkillRangeType.Rectangle:
                targets.AddRange(FindRectangleTargets(weapon, attackPosition));
                break;
                case SkillRangeType.Line:
                targets.AddRange(FindLineTargets(weapon, attackPosition));
                break;
            }

            return targets;
        }

        /// <summary>
        /// 무기 장착 처리
        /// </summary>
        public async void EquipWeapon(WeaponComponent weapon, GameObject owner) {
            try {
                // Service를 통해 스킬 데이터 로드
                var skillData = await _skillDataService.LoadSkillDataAsync(weapon.SkillName);

                if (skillData == null) {
                    GameDebug.LogError($"스킬 데이터 로드 실패: {weapon.SkillName}");
                    return;
                }

                weapon.SetSkillData(skillData);

                // CombatModel에 무기 데이터 등록
                _combatModel.AddBonusAttack(weapon.WeaponId, weapon.BaseAttack);
                _combatModel.AddBonusDefense(weapon.WeaponId, weapon.BaseDefense);
                _combatModel.AddBonusAttackSpeed(weapon.WeaponId, weapon.BaseAttackSpeed);

                weapon.SetInitialized(true);
                GameDebug.Log($"무기 초기화 완료: {weapon.SkillName}, Range: {skillData.Range}, Attack: {weapon.GetFinalAttack()}");
            } catch (System.Exception e) {
                GameDebug.LogError($"무기 초기화 실패: {weapon.SkillName}, 에러: {e.Message}");
            }
        }

        /// <summary>
        /// 무기 해제 처리
        /// </summary>
        public void UnequipWeapon(WeaponComponent weapon) {
            if (weapon.IsInitialized) {
                weapon.SetSkillData(null);
                _combatModel.AddBonusAttack(weapon.WeaponId, -weapon.BaseAttack);
                _combatModel.AddBonusDefense(weapon.WeaponId, -weapon.BaseDefense);
                _combatModel.AddBonusAttackSpeed(weapon.WeaponId, -weapon.BaseAttackSpeed);
                weapon.SetInitialized(false);
                _skillDataService.UnloadSkill(weapon.SkillName);
                _equipSystem.UnLoadWeapon(weapon);
                GameDebug.Log($"무기 해제: {weapon.SkillName}, ID: {weapon.WeaponId}");
            }
        }

        #region Private Methods

        private List<Collider2D> FindCircleTargets(WeaponComponent weapon, Vector2 center) {
            var targets = new List<Collider2D>();
            var colliders = Physics2D.OverlapCircleAll(center, weapon.Range, weapon.TargetLayers);
            targets.AddRange(colliders);
            return targets;
        }

        private List<Collider2D> FindSectorTargets(WeaponComponent weapon, Vector2 center) {
            var targets = new List<Collider2D>();
            var potentialTargets = Physics2D.OverlapCircleAll(center, weapon.Range, weapon.TargetLayers);

            float halfAngle = weapon.Angle * 0.5f;
            Vector3 forward = weapon.Forward;

            foreach (var collider in potentialTargets) {
                Vector2 directionToTarget = (collider.transform.position - weapon.AttackPoint.position).normalized;
                float angleToTarget = Vector2.Angle(forward, directionToTarget);

                if (angleToTarget <= halfAngle) {
                    targets.Add(collider);
                }
            }

            return targets;
        }

        private List<Collider2D> FindRectangleTargets(WeaponComponent weapon, Vector2 center) {
            var targets = new List<Collider2D>();

            // 사각형 영역 계산
            Vector2 boxSize = new Vector2(weapon.Width, weapon.Range);
            Vector2 boxCenter = center + weapon.Forward * (weapon.Range * 0.5f);

            var colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, weapon.AttackPoint.eulerAngles.z, weapon.TargetLayers);

            targets.AddRange(colliders);

            return targets;
        }

        private List<Collider2D> FindLineTargets(WeaponComponent weapon, Vector2 center) {
            var targets = new List<Collider2D>();

            if (weapon.SkillData.Width > 0) {
                // 두께가 있는 선 (사각형으로 처리)
                Vector2 boxSize = new Vector2(weapon.Width, weapon.Range);
                Vector2 boxCenter = center + (Vector2)weapon.Forward * (weapon.Range * 0.5f);

                var colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, weapon.AttackPoint.eulerAngles.z, weapon.TargetLayers);
                targets.AddRange(colliders);
            } else {
                // 단순한 선 (RaycastAll 사용)
                Vector2 direction = weapon.Forward;
                var hits = Physics2D.RaycastAll(center, direction, weapon.Range, weapon.TargetLayers);

                foreach (var hit in hits) {
                    if (hit.collider != null) {
                        targets.Add(hit.collider);
                    }
                }
            }

            return targets;
        }

        /// <summary>
        /// 개별 타겟에 대한 공격 처리
        /// </summary>
        private void ProcessAttackTarget(WeaponComponent weapon, Collider2D target) {
            // 최종 데미지 계산
            float finalDamage = CalculateFinalDamage(weapon);

            // 데미지 처리 (CombatSystem을 통해)
            int attackerId = weapon.WeaponId; // 소유자 ID를 사용
            int targetId = target.gameObject.GetInstanceID();

            // 무기 타입에 따른 데미지 타입 결정
            DamageType damageType = weapon.DamageType;

            _combatSystem.ProcessAttack(attackerId, targetId, damageType, finalDamage);

            GameDebug.Log($"무기 공격 처리: {target.name}, 데미지: {finalDamage}");
        }

        /// <summary>
        /// 최종 데미지 계산 (스킬 데이터 + CombatModel 스탯)
        /// </summary>
        private float CalculateFinalDamage(WeaponComponent weapon) {
            // 스킬 데이터에서 기본 데미지와 배율
            float skillBaseDamage = weapon.SkillData.AdditionalDamage;
            float skillMultiplier = weapon.SkillData.DamageMultiplier;

            // CombatModel에서 현재 공격력
            int weaponAttack = weapon.GetFinalAttack();

            // 최종 데미지 계산: (무기공격력 + 스킬추가데미지) * 스킬배율
            float finalDamage = (weaponAttack + skillBaseDamage) * skillMultiplier;

            return Mathf.Max(0, finalDamage);
        }

        #endregion

        
    }
}