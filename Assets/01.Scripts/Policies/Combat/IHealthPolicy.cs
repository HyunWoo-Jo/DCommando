
namespace Game.Policies {
    /// <summary>
    /// 체력 정책 인터페이스
    /// </summary>
    public interface IHealthPolicy {
        #region 체력 유효성 검증

        /// <summary>
        /// 최소 체력 유효성 검증
        /// </summary>
        bool IsValidMaxHp(int maxHp);

        /// <summary>
        /// 현재 체력 유효성 검증
        /// </summary>
        bool IsValidCurrentHp(int currentHp);

        /// <summary>
        /// 데미지 양 유효성 검증
        /// </summary>
        bool IsValidDamageAmount(int damage);

        /// <summary>
        /// 치료량 유효성 검증
        /// </summary>
        bool IsValidHealAmount(int healAmount);

        /// <summary>
        /// 부활 체력 유효성 검증
        /// </summary>
        bool IsValidReviveHp(int reviveHp, int maxHp);

        #endregion

        #region 상태 확인 정책

        /// <summary>
        /// 사망 상태 확인
        /// </summary>
        bool IsDead(int currentHp);

        /// <summary>
        /// 데미지를 받을 수 있는지 확인
        /// </summary>
        bool CanTakeDamage(int currentHp);

        /// <summary>
        /// 치료할 수 있는지 확인
        /// </summary>
        bool CanHeal(int currentHp, int maxHp);

        /// <summary>
        /// 부활할 수 있는지 확인
        /// </summary>
        bool CanRevive(int currentHp);

        /// <summary>
        /// 이미 풀피인지 확인
        /// </summary>
        bool IsFullHealth(int currentHp, int maxHp);

        #endregion

        #region 체력 계산

        /// <summary>
        /// 데미지 적용 후 체력 계산
        /// </summary>
        int CalculateHpAfterDamage(int currentHp, int damage);

        /// <summary>
        /// 치료 적용 후 체력 계산
        /// </summary>
        int CalculateHpAfterHeal(int currentHp, int healAmount, int maxHp);

        /// <summary>
        /// 체력 비율 계산
        /// </summary>
        float CalculateHpRatio(int currentHp, int maxHp);

        /// <summary>
        /// 실제 데미지량 계산 (적용 전후 차이)
        /// </summary>
        int CalculateActualDamage(int beforeHp, int afterHp);

        /// <summary>
        /// 실제 치료량 계산 (적용 전후 차이)
        /// </summary>
        int CalculateActualHeal(int beforeHp, int afterHp);

        /// <summary>
        /// 완전 회복에 필요한 치료량 계산
        /// </summary>
        int CalculateFullHealAmount(int currentHp, int maxHp);

        #endregion

        #region 기본값 정책

        /// <summary>
        /// 기본 부활 체력 반환 (최대 체력)
        /// </summary>
        int GetDefaultReviveHp(int maxHp);

        /// <summary>
        /// 최소 데미지 반환
        /// </summary>
        int GetMinimumDamage();

        /// <summary>
        /// 최소 치료량 반환
        /// </summary>
        int GetMinimumHeal();

        /// <summary>
        /// 최소 체력 반환
        /// </summary>
        int GetMinimumHp();

        #endregion

        #region 특수 정책

        /// <summary>
        /// 즉사 데미지 계산
        /// </summary>
        int CalculateInstantKillDamage(int currentHp);

        /// <summary>
        /// 체력 변경 시 최대/최소 범위 적용
        /// </summary>
        int ClampHp(int hp, int maxHp);

        #endregion
    }
}