using UnityEngine;

namespace Game.Policies
{
    public class CrystalPolicy : ICrystalPolicy
    {
        private const int DAILY_FREE_CRYSTAL_LIMIT = 1000;
        private const int MAX_CRYSTAL_LIMIT = 9999999;
        
        public bool CanSpendCrystal(int currentAmount, int spendAmount)
        {
            if (spendAmount <= 0)
            {
                Debug.LogWarning("크리스탈 소모량이 0 이하입니다.");
                return false;
            }
            
            if (currentAmount < spendAmount)
            {
                Debug.LogWarning($"크리스탈이 부족합니다. 현재: {currentAmount}, 필요: {spendAmount}");
                return false;
            }
            
            return true;
        }
        
        public bool CanGainCrystal(int currentAmount, int gainAmount, int maxAmount)
        {
            if (gainAmount <= 0)
            {
                Debug.LogWarning("크리스탈 획득량이 0 이하입니다.");
                return false;
            }
            
            if (currentAmount + gainAmount > maxAmount)
            {
                Debug.LogWarning($"최대 크리스탈 보유량을 초과합니다. 현재: {currentAmount}, 획득: {gainAmount}, 최대: {maxAmount}");
                return false;
            }
            
            return true;
        }
        
        public bool IsWithinDailyGainLimit(int todayGained, int gainAmount)
        {
            if (todayGained + gainAmount > DAILY_FREE_CRYSTAL_LIMIT)
            {
                Debug.LogWarning($"일일 무료 크리스탈 획득 한도를 초과합니다. 오늘 획득: {todayGained}, 추가 획득: {gainAmount}, 한도: {DAILY_FREE_CRYSTAL_LIMIT}");
                return false;
            }
            
            return true;
        }
        
        public bool ShouldSpendPaidFirst()
        {
            // 일반적으로 무료 크리스탈부터 소모하는 정책
            return false;
        }
        
        public bool IsValidTransaction(int amount, string source)
        {
            if (amount <= 0)
            {
                Debug.LogError("유효하지 않은 거래 금액입니다.");
                return false;
            }
            
            if (string.IsNullOrEmpty(source))
            {
                Debug.LogError("거래 소스가 명시되지 않았습니다.");
                return false;
            }
            
            // 허용된 소스 목록 검증
            string[] validSources = 
            {
                "quest_reward",
                "daily_login",
                "achievement",
                "purchase",
                "event_reward",
                "admin_grant"
            };
            
            bool isValidSource = false;
            foreach (string validSource in validSources)
            {
                if (source == validSource)
                {
                    isValidSource = true;
                    break;
                }
            }
            
            if (!isValidSource)
            {
                Debug.LogError($"허용되지 않은 크리스탈 소스입니다: {source}");
                return false;
            }
            
            return true;
        }
    }
}