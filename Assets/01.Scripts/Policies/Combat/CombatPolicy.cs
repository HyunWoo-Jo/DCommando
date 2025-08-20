using UnityEngine;
using Game.Core;

namespace Game.Policies {
    /// <summary>
    /// 전투 정책 - 전투 관련 규칙과 계산 로직
    /// </summary>
    public class CombatPolicy : ICombatPolicy {
        // 전투 관련 상수
        private const int MIN_DAMAGE = 1;
        private const float CRITICAL_DAMAGE_MULTIPLIER = 2.0f;
        private const float DEFAULT_LIFESTEAL_RATE = 0.15f;
        private const float DEFAULT_POISON_DURATION = 5.0f;

        #region 데미지 계산

        /// <summary>
        /// 최종 데미지 계산
        /// </summary>
        public int CalculateFinalDamage(int baseDamage, int defense) {
            // 방어력이 공격력보다 높으면 최소 데미지 보장
            return Mathf.Max(MIN_DAMAGE, baseDamage - defense);
        }

        /// <summary>
        /// 크리티컬 데미지 계산
        /// </summary>
        public int CalculateCriticalDamage(int baseDamage) {
            return Mathf.RoundToInt(baseDamage * CRITICAL_DAMAGE_MULTIPLIER);
        }

        /// <summary>
        /// 흡혈량 계산
        /// </summary>
        public int CalculateLifestealAmount(int damage, float lifestealRate = DEFAULT_LIFESTEAL_RATE) {
            return Mathf.RoundToInt(damage * lifestealRate);
        }

        #endregion

        #region 공격 유효성 검증

        /// <summary>
        /// 자기 자신 공격 검증
        /// </summary>
        public bool CanAttackSelf(int attackerId, int targetId) {
            return attackerId != targetId;
        }

        /// <summary>
        /// 데미지 타입별 방어력 무시 여부
        /// </summary>
        public bool ShouldIgnoreDefense(DamageType damageType) {
            return damageType == DamageType.Pure || damageType == DamageType.Poison;
        }

        /// <summary>
        /// 크리티컬 적용 가능 여부
        /// </summary>
        public bool CanApplyCritical(DamageType damageType) {
            return damageType != DamageType.Heal;
        }

        #endregion

        #region 상태 효과 정책

        /// <summary>
        /// 화상 효과 적용 가능 여부
        /// </summary>
        public bool CanApplyBurnEffect(DamageType damageType) {
            return damageType == DamageType.Fire;
        }

        /// <summary>
        /// 빙결 효과 적용 가능 여부
        /// </summary>
        public bool CanApplyFreezeEffect(DamageType damageType) {
            return damageType == DamageType.Ice;
        }

        /// <summary>
        /// 감전 효과 적용 가능 여부
        /// </summary>
        public bool CanApplyStunEffect(DamageType damageType) {
            return damageType == DamageType.Lightning;
        }

        /// <summary>
        /// 기본 독 지속시간 반환
        /// </summary>
        public float GetDefaultPoisonDuration() {
            return DEFAULT_POISON_DURATION;
        }

        #endregion

        #region 스탯 정책

        /// <summary>
        /// 최소 데미지 반환
        /// </summary>
        public int GetMinimumDamage() {
            return MIN_DAMAGE;
        }

        /// <summary>
        /// 크리티컬 데미지 배율 반환
        /// </summary>
        public float GetCriticalDamageMultiplier() {
            return CRITICAL_DAMAGE_MULTIPLIER;
        }

        /// <summary>
        /// 기본 흡혈률 반환
        /// </summary>
        public float GetDefaultLifestealRate() {
            return DEFAULT_LIFESTEAL_RATE;
        }

        #endregion
    }
}