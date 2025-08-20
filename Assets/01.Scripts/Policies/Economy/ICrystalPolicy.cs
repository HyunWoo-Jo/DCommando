namespace Game.Policies
{
    /// <summary>
    /// 크리스탈 정책 인터페이스
    /// </summary>
    public interface ICrystalPolicy
    {
        /// <summary>
        /// 크리스탈 소모 가능 여부 검증
        /// </summary>
        bool CanSpendCrystal(int currentAmount, int spendAmount);
        
        /// <summary>
        /// 크리스탈 획득 가능 여부 검증
        /// </summary>
        bool CanGainCrystal(int currentAmount, int gainAmount, int maxAmount);
        
        /// <summary>
        /// 일일 크리스탈 획득 제한 확인
        /// </summary>
        bool IsWithinDailyGainLimit(int todayGained, int gainAmount);
        
        /// <summary>
        /// 유료 크리스탈 소모 우선순위 정책
        /// </summary>
        bool ShouldSpendPaidFirst();
        
        /// <summary>
        /// 크리스탈 거래 유효성 검증
        /// </summary>
        bool IsValidTransaction(int amount, string source);
    }
}