using Game.Models;
using Game.Services;
using Game.Policies;
using R3;
using Cysharp.Threading.Tasks;

namespace Game.Systems
{
    public class GoldSystem
    {
        private readonly GoldModel _goldModel;
        private readonly IGoldService _goldService;
        private readonly IGoldPolicy _goldPolicy;
        private readonly SO_GoldConfig _config;
        
        // 골드 변경 이벤트
        public readonly Subject<int> OnGoldChangedEvent = new();
        public readonly Subject<int> OnGoldAddedEvent = new();
        public readonly Subject<int> OnGoldSpentEvent = new();
        
        public GoldSystem(GoldModel goldModel, IGoldService goldService, 
                         IGoldPolicy goldPolicy, SO_GoldConfig config)
        {
            _goldModel = goldModel;
            _goldService = goldService;
            _goldPolicy = goldPolicy;
            _config = config;
            
            // 최대 골드 설정
            _goldModel.SetMaxGold(_config.maxGold);
        }
        
        public async UniTask InitializeAsync()
        {
            var savedGold = await _goldService.LoadGoldAsync();
            var clampedGold = _goldPolicy.ClampGold(savedGold, _config.minGold, _config.maxGold);
            _goldModel.SetGold(clampedGold);
        }
        
        public async UniTask<bool> AddGoldAsync(int amount)
        {
            if (amount <= 0) return false;
            
            var currentGold = _goldModel.CurrentGold.CurrentValue;
            if (!_goldPolicy.CanAddGold(currentGold, amount, _config.maxGold))
                return false;
            
            var newGold = _goldPolicy.ClampGold(currentGold + amount, _config.minGold, _config.maxGold);
            _goldModel.SetGold(newGold);
            
            await _goldService.SaveGoldAsync(newGold);
            
            OnGoldAddedEvent.OnNext(amount);
            OnGoldChangedEvent.OnNext(newGold);
            
            return true;
        }
        
        public async UniTask<bool> SpendGoldAsync(int amount)
        {
            if (amount <= 0) return false;
            
            var currentGold = _goldModel.CurrentGold.CurrentValue;
            if (!_goldPolicy.CanSpendGold(currentGold, amount))
                return false;
            
            var newGold = currentGold - amount;
            _goldModel.SetGold(newGold);
            
            await _goldService.SaveGoldAsync(newGold);
            
            OnGoldSpentEvent.OnNext(amount);
            OnGoldChangedEvent.OnNext(newGold);
            
            return true;
        }
        
        public bool CanSpend(int amount)
        {
            return _goldModel.CanSpend(amount);
        }
    }
}