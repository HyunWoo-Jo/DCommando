using UnityEngine;
using Zenject;
using System;
using Game.Models;
using Game.Core;
using R3;
using Game.Core.Event;
using Zenject.SpaceFighter;

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
            GameDebug.Log("CombatSystem 초기화 완료");
            EventBus.Subscribe<DamageRequestEvent>(OnProcessRequest).AddTo(_disposables);
            EventBus.Subscribe<EnemyDefeatedEvent>(_ => _combatModel.AddKillCount()).AddTo(_disposables); // enemy가 죽으면 Model Update
            EventBus.Subscribe<StatChangeEvent>(OnUpgradeEvent).AddTo(_disposables);
        }

        public void Dispose() {
            _disposables?.Dispose();
            GameDebug.Log("CombatSystem 해제 완료");
        }

        public void OnUpgradeEvent(StatChangeEvent e) {
            if(e.upgradeType == UpgradeType.Power) {
                int id =AIDataProvider.GetPlayer().GetInstanceID();
                _combatModel.AddBonusAttack(id, (int)e.value);
            } else if(e.upgradeType == UpgradeType.Defense) {
                int id = AIDataProvider.GetPlayer().GetInstanceID();
                _combatModel.AddBonusDefense(id, (int)e.value);
            } else if(e.upgradeType == UpgradeType.AttackSpeed) {
                int id = AIDataProvider.GetPlayer().GetInstanceID();
                _combatModel.AddBonusAttackSpeed(id, e.value);
            }
        }
        #endregion



        #region 캐릭터 전투 등록/해제

        /// <summary>
        /// 전투 캐릭터 등록 (전투 스탯)
        /// </summary>
        public bool RegisterCombatCharacter(int characterId, int baseAttack, int baseDefense, float baseAttackSpeed) {
            // CombatModel에 전투 스탯 등록
            _combatModel.RegisterCombat(characterId, baseAttack, baseDefense, baseAttackSpeed);

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

        #region 공격 처리 (WeaponSystem 호환)

        public void OnProcessRequest(DamageRequestEvent drEvent) {
            ProcessAttack(drEvent.ownerID, drEvent.targetID, drEvent.damageType, false);
        }


        /// <summary>
        /// 타입별 공격 처리 (커스텀 데미지 지원 - WeaponSystem용)
        /// </summary>
        public bool ProcessAttack(int attackerId, int targetId, DamageType damageType, float customDamage = -1f) {
            if (!ValidateAttack(attackerId, targetId)) return false;

            // 커스텀 데미지가 지정되지 않으면 기본 공격력 사용
            float attackPower = customDamage >= 0 ? customDamage : _combatModel.GetFinalAttack(attackerId);
            int defense = _combatModel.GetFinalDefense(targetId);

            return ApplyDamage(attackerId, targetId, Mathf.RoundToInt(attackPower), defense, damageType);
        }

        /// <summary>
        /// 타입별 공격 처리 (몬스터 처리)
        /// </summary>
        public bool ProcessAttack(int attackerId, int targetId, DamageType damageType, bool isCritical = false) {
            if (!ValidateAttack(attackerId, targetId)) return false;

            int attackPower = _combatModel.GetFinalAttack(attackerId);
            int defense = _combatModel.GetFinalDefense(targetId);

            return ApplyDamage(attackerId, targetId, attackPower, defense, damageType, isCritical);
        }

        /// <summary>
        /// 물리 공격 처리
        /// </summary>
        public bool ProcessPhysicalAttack(int attackerId, int targetId, bool isCritical = false) {
            return ProcessAttack(attackerId, targetId, DamageType.Physical, isCritical);
        }

        /// <summary>
        /// 마법 공격 처리
        /// </summary>
        public bool ProcessMagicAttack(int attackerId, int targetId, bool isCritical = false) {
            return ProcessAttack(attackerId, targetId, DamageType.Magic, isCritical);
        }

        /// <summary>
        /// 화염 공격 처리 / 지속 피해
        /// </summary>
        public bool ProcessFireAttack(int attackerId, int targetId, bool isCritical = false) {
            bool result = ProcessAttack(attackerId, targetId, DamageType.Fire, isCritical);

            // 화상 효과 추가 (선택적)
            if (result) {
                ApplyBurnEffect(attackerId, targetId);
            }

            return result;
        }

        /// <summary>
        /// 얼음 공격 처리
        /// </summary>
        public bool ProcessIceAttack(int attackerId, int targetId, bool isCritical = false) {
            bool result = ProcessAttack(attackerId, targetId, DamageType.Ice, isCritical);

            // 빙결 효과 추가 (선택적)
            if (result) {
                ApplyFreezeEffect(targetId);
            }

            return result;
        }

        /// <summary>
        /// 번개 공격 처리
        /// </summary>
        public bool ProcessLightningAttack(int attackerId, int targetId, bool isCritical = false) {
            bool result = ProcessAttack(attackerId, targetId, DamageType.Lightning, isCritical);

            // 감전 효과 추가 (선택적)
            if (result) {
                ApplyStunEffect(targetId);
            }

            return result;
        }

        /// <summary>
        /// 독 데미지 처리 (방어력 무시) / 지속피해
        /// </summary>
        public bool ProcessPoisonDamage(int targetId, int poisonDamage, int attackerId = -1) {
            if (!_healthSystem.CanTakeDamage(targetId)) return false;

            bool success = _healthSystem.TakeDamage(targetId, poisonDamage, DamageType.Poison);

            if (attackerId != -1) {
                GameDebug.Log($"독 효과 적용 Attacker {attackerId} -> Target {targetId}: {poisonDamage} damage over {DEFAULT_POISON_DURATION}초");
            } else {
                GameDebug.Log($"독 데미지 적용 Target {targetId}: {poisonDamage} damage");
            }

            return success;
        }

        /// <summary>
        /// 순수 데미지 처리 (방어력 무시)
        /// </summary>
        public bool ProcessPureDamage(int attackerId, int targetId, int pureDamage) {
            if (!ValidateAttack(attackerId, targetId)) return false;

            bool success = _healthSystem.TakeDamage(targetId, pureDamage, DamageType.Pure);
            GameDebug.Log($"순수 데미지 적용 Character {attackerId} -> {targetId}: {pureDamage} pure damage");

            return success;
        }

        /// <summary>
        /// 치료 처리
        /// </summary>
        public bool ProcessHeal(int targetId, int healAmount, int healerId = -1) {
            if (!_healthSystem.CanHeal(targetId)) return false;

            bool success = _healthSystem.Heal(targetId, healAmount);

            if (success && healerId != -1) {
                GameDebug.Log($"치료 적용 Character {healerId} -> {targetId}: {healAmount} heal");
            }

            return success;
        }

        /// <summary>
        /// 흡혈 효과 처리
        /// </summary>
        public bool ProcessLifesteal(int attackerId, int targetId, int damage, float lifestealRate = DEFAULT_LIFESTEAL_RATE) {
            if (!_healthSystem.HasCharacter(attackerId)) return false;

            int healAmount = Mathf.RoundToInt(damage * lifestealRate);
            bool success = _healthSystem.Heal(attackerId, healAmount);

            if (success) {
                GameDebug.Log($"흡혈 효과 Character {attackerId} 회복 {healAmount} HP");
            }

            return success;
        }

        #endregion

        #region 데미지 계산 및 적용

        /// <summary>
        /// 데미지 계산 및 적용
        /// </summary>
        private bool ApplyDamage(int attackerId, int targetId, int baseDamage, int defense, DamageType damageType, bool isCritical = false) {
            int finalDamage;

            // 순수 데미지나 독은 방어력 무시
            if (damageType == DamageType.Pure || damageType == DamageType.Poison) {
                finalDamage = baseDamage;
            } else {
                finalDamage = CalculateFinalDamage(baseDamage, defense);
            }

            // 크리티컬 적용 (치료는 제외)
            if (isCritical && damageType != DamageType.Heal) {
                finalDamage = Mathf.RoundToInt(finalDamage * CRITICAL_DAMAGE_MULTIPLIER);
            }

            // HealthSystem을 통해 데미지 적용
            bool success = _healthSystem.TakeDamage(targetId, finalDamage, damageType);

            if (success) {
                string criticalText = isCritical ? " (크리티컬!)" : "";
                GameDebug.Log($"캐릭터 {attackerId}가 {targetId}에게 {finalDamage} {damageType} 데미지{criticalText}");


            } else {
                GameDebug.LogWarning($"데미지 적용 실패 {attackerId} -> {targetId}");
            }

            return success;
        }

        /// <summary>
        /// 최종 데미지 계산
        /// </summary>
        private int CalculateFinalDamage(int baseDamage, int defense) {
            // 방어력이 공격력보다 높으면 최소 데미지 보장
            return Mathf.Max(MIN_DAMAGE, baseDamage - defense);
        }

        #endregion

        #region 상태 효과 (확장 가능)

        /// <summary>
        /// 화상 효과 적용
        /// </summary>
        private void ApplyBurnEffect(int attackerId, int targetId) {
            // TODO: 지속 데미지 시스템 구현시 추가
            GameDebug.Log($"화상 효과 적용 Target {targetId}");
        }

        /// <summary>
        /// 빙결 효과 적용
        /// </summary>
        private void ApplyFreezeEffect(int targetId) {
            // TODO: 상태이상 시스템 구현시 추가
            GameDebug.Log($"빙결 효과 적용 Target {targetId}");
        }

        /// <summary>
        /// 감전 효과 적용
        /// </summary>
        private void ApplyStunEffect(int targetId) {
            // TODO: 상태이상 시스템 구현시 추가
            GameDebug.Log($"감전 효과 적용 Target {targetId}");
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
        public ReadOnlyReactiveProperty<CombatData> GetRORP_CombatData(int id) {
            return _combatModel.GetRORP_CombatData(id);
        }
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


        

        public float GetFinalAttackSpeed(int characterId) {
            return _combatModel.GetFinalAttackSpeed(characterId);
        }

        /// <summary>
        /// 전투 데이터 존재 여부 확인
        /// </summary>
        public bool HasCombatData(int characterId) {
            return _combatModel.HasCombat(characterId);
        }

        /// <summary>
        /// 전투 통계 정보 (디버그용)
        /// </summary>
        public void LogCombatStats() {
            int combatCount = _combatModel.GetCombatCount();
            GameDebug.Log($"전투 시스템 현황: {combatCount}개 캐릭터 등록");

            foreach (int characterId in _combatModel.GetAllCombatIDs()) {
                var combatData = _combatModel.GetCombatData(characterId);
                bool canFight = CanFight(characterId);
                GameDebug.Log($"Character {characterId}: ATK {combatData.FinalAttack}, DEF {combatData.FinalDefense}, 전투가능: {canFight}");
            }
        }


        #endregion

        #region 검증 메서드

        /// <summary>
        /// 공격 유효성 검증
        /// </summary>
        private bool ValidateAttack(int attackerId, int targetId) {
            // 자기 자신 공격 방지
            if (attackerId == targetId) {
                GameDebug.LogWarning($"자기 자신을 공격할 수 없음 Character {attackerId}");
                return false;
            }

            // 공격자 검증
            if (!_combatModel.HasCombat(attackerId)) {
                GameDebug.LogError($"공격자 Combat 데이터 없음 Character {attackerId}");
                return false;
            }

            if (!_healthSystem.HasCharacter(attackerId)) {
                GameDebug.LogWarning($"공격자 Health 데이터 없음 Character {attackerId}");
                return false;
            }

            if (_healthSystem.IsDead(attackerId)) {
                GameDebug.LogWarning($"사망한 공격자 Character {attackerId}");
                return false;
            }

            // 대상 검증
            if (!_combatModel.HasCombat(targetId)) {
                GameDebug.LogError($"대상 Combat 데이터 없음 Character {targetId}");
                return false;
            }

            if (!_healthSystem.CanTakeDamage(targetId)) {
                GameDebug.LogWarning($"데미지를 받을 수 없는 대상 Character {targetId}");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 연결된 시스템 상태 검증
        /// </summary>
        public bool ValidateSystemIntegrity() {
            if (_combatModel == null) {
                GameDebug.LogError("CombatModel이 주입되지 않음");
                return false;
            }

            if (_healthSystem == null) {
                GameDebug.LogError("HealthSystem이 주입되지 않음");
                return false;
            }

            GameDebug.Log("CombatSystem 무결성 검증 통과");
            return true;
        }

        #endregion
    }
}