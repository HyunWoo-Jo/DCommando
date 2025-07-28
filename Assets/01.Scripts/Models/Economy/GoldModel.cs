using R3;

namespace Game.Models
{
    public class GoldModel
    {
        private readonly ReactiveProperty<int> _currentGold = new(0);
        private readonly ReactiveProperty<int> _maxGold = new(999999);
        
        public ReadOnlyReactiveProperty<int> CurrentGold => _currentGold;
        public ReadOnlyReactiveProperty<int> MaxGold => _maxGold;
        
        public void SetGold(int amount)
        {
            _currentGold.Value = amount;
        }
        
        public void SetMaxGold(int max)
        {
            _maxGold.Value = max;
        }
        
        public bool CanSpend(int amount)
        {
            return _currentGold.Value >= amount;
        }
    }
}