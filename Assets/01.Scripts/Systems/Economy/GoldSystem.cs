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
            Initialize();
        }
        
        public void Initialize()
        {
            var savedGold = _config.startingGold;
            var clampedGold = _goldPolicy.ClampGold(savedGold, _config.minGold, _config.maxGold);
            _goldModel.SetGold(clampedGold);
        }
        
        public bool AddGold(int amount)
        {
            if (amount <= 0) return false;

            var currentGold = _goldModel.RORP_CurrentGold.CurrentValue;

            // 정책 검사 (최대 골드 초과 여부 확인)
            if (!_goldPolicy.CanAddGold(currentGold, amount, _config.maxGold))
                return false;

            // 골드 계산 정책에 따라 제한
            var newGold = _goldPolicy.ClampGold(currentGold + amount, _config.minGold, _config.maxGold);
            _goldModel.SetGold(newGold);

            // 추후 체크용 
            _ = _goldService.CheckGoldAsync(newGold);

            // 이벤트 Call
            OnGoldAddedEvent.OnNext(amount);
            OnGoldChangedEvent.OnNext(newGold);

            return true;
        }
        
        public bool SpendGold(int amount)
        {
            if (amount <= 0) return false; 
            var currentGold = _goldModel.RORP_CurrentGold.CurrentValue;

            // 정책 검사 (소비 가능 여부 확인)
            if (!_goldPolicy.CanSpendGold(currentGold, amount))
                return false;
            
            // 골드 감소
            var newGold = currentGold - amount;
            _goldModel.SetGold(newGold);

            // 추후 체크용 
            _ = _goldService.CheckGoldAsync(newGold);

            // 이벤트 Call
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