namespace Game.Policies {
    public interface IGoldPolicy
    {
        bool CanAddGold(int currentGold, int addAmount, int maxGold);
        bool CanSpendGold(int currentGold, int spendAmount);
        int ClampGold(int gold, int minGold, int maxGold);
    }
}