using Game.Core;

namespace Game.Policies {
    /// <summary>
    /// 체력 정책 - 체력 관련 규칙과 계산 로직
    /// </summary>
    public class HealthPolicy : IHealthPolicy {
        // 체력 관련 상수
        private const int MIN_DAMAGE = 0; // 최소 데미지
        private const int MIN_HEAL = 1; // 최소 힐량
        private const int MIN_HP = 1; // 최소 등록 HP

        #region 체력 유효성 검증

        /// <summary>
        /// 최소 체력 유효성 검증
        /// </summary>
        public bool IsValidMaxHp(int maxHp) {
            return maxHp >= MIN_HP;
        }

        /// <summary>
        /// 현재 체력 유효성 검증
        /// </summary>
        public bool IsValidCurrentHp(int currentHp) {
            return currentHp >= 0;
        }

        /// <summary>
        /// 데미지 양 유효성 검증
        /// </summary>
        public bool IsValidDamageAmount(int damage) {
            return damage >= MIN_DAMAGE;
        }

        /// <summary>
        /// 치료량 유효성 검증
        /// </summary>
        public bool IsValidHealAmount(int healAmount) {
            return healAmount >= MIN_HEAL;
        }

        /// <summary>
        /// 부활 체력 유효성 검증
        /// </summary>
        public bool IsValidReviveHp(int reviveHp, int maxHp) {
            return reviveHp >= MIN_HP && reviveHp <= maxHp;
        }

        #endregion

        #region 상태 확인 정책

        /// <summary>
        /// 사망 상태 확인
        /// </summary>
        public bool IsDead(int currentHp) {
            return currentHp <= 0;
        }

        /// <summary>
        /// 데미지를 받을 수 있는지 확인
        /// </summary>
        public bool CanTakeDamage(int currentHp) {
            return !IsDead(currentHp);
        }

        /// <summary>
        /// 치료할 수 있는지 확인
        /// </summary>
        public bool CanHeal(int currentHp, int maxHp) {
            return !IsDead(currentHp) && currentHp < maxHp;
        }

        /// <summary>
        /// 부활할 수 있는지 확인
        /// </summary>
        public bool CanRevive(int currentHp) {
            return IsDead(currentHp);
        }

        /// <summary>
        /// 이미 풀피인지 확인
        /// </summary>
        public bool IsFullHealth(int currentHp, int maxHp) {
            return currentHp >= maxHp;
        }

        #endregion

        #region 체력 계산

        /// <summary>
        /// 데미지 적용 후 체력 계산
        /// </summary>
        public int CalculateHpAfterDamage(int currentHp, int damage) {
            return UnityEngine.Mathf.Max(0, currentHp - damage);
        }

        /// <summary>
        /// 치료 적용 후 체력 계산
        /// </summary>
        public int CalculateHpAfterHeal(int currentHp, int healAmount, int maxHp) {
            return UnityEngine.Mathf.Min(maxHp, currentHp + healAmount);
        }

        /// <summary>
        /// 체력 비율 계산
        /// </summary>
        public float CalculateHpRatio(int currentHp, int maxHp) {
            if (maxHp <= 0) return 0f;
            return (float)currentHp / maxHp;
        }

        /// <summary>
        /// 실제 데미지량 계산 (적용 전후 차이)
        /// </summary>
        public int CalculateActualDamage(int beforeHp, int afterHp) {
            return beforeHp - afterHp;
        }

        /// <summary>
        /// 실제 치료량 계산 (적용 전후 차이)
        /// </summary>
        public int CalculateActualHeal(int beforeHp, int afterHp) {
            return afterHp - beforeHp;
        }

        /// <summary>
        /// 완전 회복에 필요한 치료량 계산
        /// </summary>
        public int CalculateFullHealAmount(int currentHp, int maxHp) {
            return UnityEngine.Mathf.Max(0, maxHp - currentHp);
        }

        #endregion

        #region 기본값 정책

        /// <summary>
        /// 기본 부활 체력 반환 (최대 체력)
        /// </summary>
        public int GetDefaultReviveHp(int maxHp) {
            return maxHp;
        }

        /// <summary>
        /// 최소 데미지 반환
        /// </summary>
        public int GetMinimumDamage() {
            return MIN_DAMAGE;
        }

        /// <summary>
        /// 최소 치료량 반환
        /// </summary>
        public int GetMinimumHeal() {
            return MIN_HEAL;
        }

        /// <summary>
        /// 최소 체력 반환
        /// </summary>
        public int GetMinimumHp() {
            return MIN_HP;
        }

        #endregion

        #region 특수 정책

        /// <summary>
        /// 즉사 데미지 계산
        /// </summary>
        public int CalculateInstantKillDamage(int currentHp) {
            return currentHp;
        }

        /// <summary>
        /// 체력 변경 시 최대/최소 범위 적용
        /// </summary>
        public int ClampHp(int hp, int maxHp) {
            return UnityEngine.Mathf.Clamp(hp, 0, maxHp);
        }

        #endregion
    }
}