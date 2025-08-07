using System;
using UnityEngine;
using Zenject;
using Game.Core;
using Game.Models;
using Game.Core.Event;
using R3;

namespace Game.Systems {
    /// <summary>
    /// 전투 시스템 - CombatModel과 HealthSystem을 활용한 전투 로직 처리
    /// </summary>
    public class CombatSystem : IInitializable, IDisposable {
        // 의존성 주입
        [Inject] private CombatModel _combatModel;
        [Inject] private HealthSystem _healthSystem;

        // 전투 관련 상수
        private const int MIN_DAMAGE = 1;
        private const float CRITICAL_DAMAGE_MULTIPLIER = 2.0f;
        private const float DEFAULT_LIFESTEAL_RATE = 0.15f;
        private const float DEFAULT_POISON_DURATION = 5.0f;

        private readonly CompositeDisposable _disposables = new();


        #region 초기화, 해제
        public void Initialize() {
  
        }
        public void Dispose() {
            _disposables?.Dispose();
        }
        #endregion
        #region 캐릭터 전투 등록/해제

        /// <summary>
        /// 전투 캐릭터 등록 (전투 스탯)
        /// </summary>
        public bool RegisterCombatCharacter(int characterId, int baseAttack, int baseDefense) {

            // CombatModel에 전투 스탯 등록
            _combatModel.AddCombat(characterId, baseAttack, baseDefense);

            GameDebug.Log($"전투 캐릭터 등록 Character {characterId}: 공격력 {baseAttack}, 방어력 {baseDefense}");
            return true;
        }

        /// <summary>
        /// 전투 캐릭터 등록 해제
        /// </summary>
        public void UnregisterCombatCharacter(int characterId) {
            _healthSystem.UnregisterCharacter(characterId);
            _combatModel.RemoveCombat(characterId);

            GameDebug.Log($"전투 캐릭터 해제 Character {characterId}");
        }

        #endregion


        #region 공격 처리

        /// <summary>
        /// 기본 공격 처리
        /// </summary>
        public bool ProcessAttack(int attackerId, int targetId) {


            if (!ValidateAttack(attackerId, targetId)) return false;

            int attackPower = _combatModel.GetFinalAttack(attackerId);
            int defense = _combatModel.GetFinalDefense(targetId);

            return ApplyDamage(attackerId, targetId, attackPower, defense);
        }

        #endregion

        #region 데미지 계산 및 적용

        /// <summary>
        /// 데미지 계산 및 적용
        /// </summary>
        private bool ApplyDamage(int attackerId, int targetId, int baseDamage, int defense) {
            // 최종 데미지 계산 (방어력 적용)
            int finalDamage = CalculateFinalDamage(baseDamage, defense);

            // HealthSystem을 통해 데미지 적용
            bool success = _healthSystem.TakeDamage(targetId, finalDamage);

            if (success) {
                GameDebug.Log($"캐릭터 {attackerId}가 {targetId}에게 {finalDamage} 데미지 (기본: {baseDamage}, 방어력: {defense})");
            }

            return success;
        }

        /// <summary>
        /// 최종 데미지 계산
        /// </summary>
        private int CalculateFinalDamage(int baseDamage, int defense) {
            return Mathf.Max(MIN_DAMAGE, baseDamage - defense);
        }

        #endregion


        #region 스탯 관리

        /// <summary>
        /// 공격력 버프
        /// </summary>
        public void ApplyAttackBuff(int characterId, int bonusAttack) {
            if (!_combatModel.HasCombat(characterId)) return;

            _combatModel.AddBonusAttack(characterId, bonusAttack);
            GameDebug.Log($"Character {characterId} 공격력 증가 +{bonusAttack}");
        }

        /// <summary>
        /// 방어력 버프
        /// </summary>
        public void ApplyDefenseBuff(int characterId, int bonusDefense) {
            if (!_combatModel.HasCombat(characterId)) return;

            _combatModel.AddBonusDefense(characterId, bonusDefense);
            GameDebug.Log($"Character {characterId} 방어력 증가 +{bonusDefense}");
        }

        /// <summary>
        /// 공격력 배율 적용
        /// </summary>
        public void ApplyAttackMultiplier(int characterId, float multiplier) {
            if (!_combatModel.HasCombat(characterId)) return;

            _combatModel.MultiplyAttack(characterId, multiplier);
            GameDebug.Log($"Character {characterId} 공격력 배율 적용 x{multiplier:F2}");
        }

        /// <summary>
        /// 방어력 배율 적용
        /// </summary>
        public void ApplyDefenseMultiplier(int characterId, float multiplier) {
            if (!_combatModel.HasCombat(characterId)) return;

            _combatModel.MultiplyDefense(characterId, multiplier);
            GameDebug.Log($"Character {characterId} 방어력 배율 적용 x{multiplier:F2}");
        }

        #endregion

        #region 상태 조회

        /// <summary>
        /// 캐릭터가 전투 가능한지 확인
        /// </summary>
        public bool CanFight(int characterId) {
            return _combatModel.HasCombat(characterId) &&
                   _healthSystem.HasCharacter(characterId) &&
                   !_healthSystem.IsDead(characterId);
        }

        /// <summary>
        /// 최종 공격력 조회
        /// </summary>
        public int GetFinalAttack(int characterId) {
            return _combatModel.GetFinalAttack(characterId);
        }

        /// <summary>
        /// 최종 방어력 조회
        /// </summary>
        public int GetFinalDefense(int characterId) {
            return _combatModel.GetFinalDefense(characterId);
        }

        #endregion

        #region 검증 메서드

        /// <summary>
        /// 공격 유효성 검증
        /// </summary>
        private bool ValidateAttack(int attackerId, int targetId) {
            // 공격자 검증
            if (!_combatModel.HasCombat(attackerId)) {
                GameDebug.LogError($"공격자 Combat 데이터 없음 Character {attackerId}");
                return false;
            }

            if (!_healthSystem.HasCharacter(attackerId)) {
                GameDebug.LogError($"공격자 Health 데이터 없음 Character {attackerId}");
                return false;
            }

            if (_healthSystem.IsDead(attackerId)) {
                GameDebug.LogError($"사망한 공격자 Character {attackerId}");
                return false;
            }

            // 대상 검증
            if (!_combatModel.HasCombat(targetId)) {
                GameDebug.LogError($"대상 Combat 데이터 없음 Character {targetId}");
                return false;
            }

            if (!_healthSystem.CanTakeDamage(targetId)) {
                GameDebug.LogError($"데미지를 받을 수 없는 대상 Character {targetId}");
                return false;
            }

            return true;
        }

        #endregion

    }
}