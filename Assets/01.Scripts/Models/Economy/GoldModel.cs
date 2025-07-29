using R3;

namespace Game.Models
{
    public class GoldModel
    {
        private readonly ReactiveProperty<int> RP_currentGold = new(0);
        private readonly ReactiveProperty<int> RP_maxGold = new(999999);
        
        public ReadOnlyReactiveProperty<int> RORP_CurrentGold => RP_currentGold;
        public ReadOnlyReactiveProperty<int> RORP_MaxGold => RP_maxGold;
        
        public void SetGold(int amount)
        {
            RP_currentGold.Value = amount;
        }
        
        public void SetMaxGold(int max)
        {
            RP_maxGold.Value = max;
        }
        
        public bool CanSpend(int amount)
        {
            return RP_currentGold.Value >= amount;
        }
    }
}