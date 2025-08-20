using Game.Core;

namespace Game.Policies {
    /// <summary>
    /// 전투 정책 인터페이스
    /// </summary>
    public interface ICombatPolicy {
        #region 데미지 계산

        /// <summary>
        /// 최종 데미지 계산
        /// </summary>
        int CalculateFinalDamage(int baseDamage, int defense);

        /// <summary>
        /// 크리티컬 데미지 계산
        /// </summary>
        int CalculateCriticalDamage(int baseDamage);

        /// <summary>
        /// 흡혈량 계산
        /// </summary>
        int CalculateLifestealAmount(int damage, float lifestealRate = 0.15f);

        #endregion

        #region 공격 유효성 검증

        /// <summary>
        /// 자기 자신 공격 검증
        /// </summary>
        bool CanAttackSelf(int attackerId, int targetId);

        /// <summary>
        /// 데미지 타입별 방어력 무시 여부
        /// </summary>
        bool ShouldIgnoreDefense(DamageType damageType);

        /// <summary>
        /// 크리티컬 적용 가능 여부
        /// </summary>
        bool CanApplyCritical(DamageType damageType);

        #endregion

        #region 상태 효과 정책

        /// <summary>
        /// 화상 효과 적용 가능 여부
        /// </summary>
        bool CanApplyBurnEffect(DamageType damageType);

        /// <summary>
        /// 빙결 효과 적용 가능 여부
        /// </summary>
        bool CanApplyFreezeEffect(DamageType damageType);

        /// <summary>
        /// 감전 효과 적용 가능 여부
        /// </summary>
        bool CanApplyStunEffect(DamageType damageType);

        /// <summary>
        /// 기본 독 지속시간 반환
        /// </summary>
        float GetDefaultPoisonDuration();

        #endregion

        #region 스탯 정책

        /// <summary>
        /// 최소 데미지 반환
        /// </summary>
        int GetMinimumDamage();

        /// <summary>
        /// 크리티컬 데미지 배율 반환
        /// </summary>
        float GetCriticalDamageMultiplier();

        /// <summary>
        /// 기본 흡혈률 반환
        /// </summary>
        float GetDefaultLifestealRate();

        #endregion
    }
}