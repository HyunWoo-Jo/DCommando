using Zenject;
using System;
using Game.Models;
using Game.Systems;
using R3;
using Cysharp.Threading.Tasks;

namespace Game.ViewModels
{
    public class GoldViewModel : IDisposable
    {   
        [Inject] private GoldModel _goldModel;
        [Inject] private GoldSystem _goldSystem;
        
        private readonly CompositeDisposable _disposables = new();
        
        // UI 바인딩용 프로퍼티
        public ReadOnlyReactiveProperty<int> CurrentGold { get; private set; }
        public ReadOnlyReactiveProperty<string> GoldText { get; private set; }
        public ReadOnlyReactiveProperty<bool> CanAfford { get; private set; }
        
        // UI 이벤트
        public readonly Subject<string> OnNotificationEvent = new();
        
        private int _checkAmount = 0;
        
        [Inject]
        public void Initialize()
        {
            // UI 바인딩 설정
            CurrentGold = _goldModel.CurrentGold;
            
            GoldText = _goldModel.CurrentGold
                .Select(gold => $"{gold:N0}")
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            
            CanAfford = _goldModel.CurrentGold
                .Select(gold => gold >= _checkAmount)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            
            // 시스템 이벤트 구독
            _goldSystem.OnGoldAddedEvent
                .Subscribe(amount => OnNotificationEvent.OnNext($"골드 {amount:N0} 획득!"))
                .AddTo(_disposables);
                
            _goldSystem.OnGoldSpentEvent
                .Subscribe(amount => OnNotificationEvent.OnNext($"골드 {amount:N0} 소모"))
                .AddTo(_disposables);
        }

        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() {
            // 초기화가 완료되었음을 알림
            OnNotificationEvent.OnNext("골드 시스템 초기화 완료");
        }
        
        public void SetCheckAmount(int amount)
        {
            _checkAmount = amount;
        }
        
        public async UniTask<bool> AddGoldAsync(int amount)
        {
            return await _goldSystem.AddGoldAsync(amount);
        }
        
        public async UniTask<bool> SpendGoldAsync(int amount)
        {
            var success = await _goldSystem.SpendGoldAsync(amount);
            if (!success)
            {
                OnNotificationEvent.OnNext("골드가 부족합니다!");
            }
            return success;
        }
        
        public bool CanSpend(int amount)
        {
            return _goldSystem.CanSpend(amount);
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
            OnNotificationEvent?.Dispose();
        }
    }
}