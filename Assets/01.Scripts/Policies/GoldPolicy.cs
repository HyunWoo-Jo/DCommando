namespace Game.Policies
{
    public class GoldPolicy : IGoldPolicy
    {
        public bool CanAddGold(int currentGold, int addAmount, int maxGold)
        {
            return currentGold + addAmount <= maxGold;
        }
        
        public bool CanSpendGold(int currentGold, int spendAmount)
        {
            return currentGold >= spendAmount && spendAmount >= 0;
        }
        
        public int ClampGold(int gold, int minGold, int maxGold)
        {
            if (gold < minGold) return minGold;
            if (gold > maxGold) return maxGold;
            return gold;
        }
    }
}