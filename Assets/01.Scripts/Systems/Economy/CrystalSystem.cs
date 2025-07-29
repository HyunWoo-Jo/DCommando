using Game.Models;
using Game.Services;
using Game.Policies;
using R3;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Game.Systems
{
    /// <summary>
    /// 크리스탈 시스템 - Gold 시스템과 유사한 구조
    /// </summary>
    public class CrystalSystem
    {
        [Inject] private CrystalModel _crystalModel;
        [Inject] private ICrystalService _crystalService;
        [Inject] private ICrystalPolicy _crystalPolicy;
        
        public ReadOnlyReactiveProperty<int> RORP_Crystal => _crystalModel.RORP_CurrentCrystal;
        public ReadOnlyReactiveProperty<int> RORP_FreeCrystal => _crystalModel.RORP_FreeCrystal;
        public ReadOnlyReactiveProperty<int> RORP_PaidCrystal => _crystalModel.RORP_PaidCrystal;
        
        /// <summary>
        /// 크리스탈 획득
        /// </summary>
        public async UniTask<bool> GainCrystalAsync(int amount, string source, bool isPaid = false)
        {
            if (!_crystalPolicy.IsValidTransaction(amount, source))
                return false;
            
            _crystalModel.AddCrystal(amount, isPaid);
            
            // Firebase 로그 (구현 예정)
            await _crystalService.LogCrystalGainAsync(amount, source, isPaid);
            
            Debug.Log($"크리스탈 획득: +{amount} ({source}), 총: {RORP_Crystal.CurrentValue}");
            return true;
        }
        
        /// <summary>
        /// 크리스탈 소모
        /// </summary>
        public async UniTask<bool> SpendCrystalAsync(int amount, string purpose)
        {
            if (!_crystalPolicy.CanSpendCrystal(RORP_Crystal.CurrentValue, amount))
                return false;
            
            if (!_crystalModel.SpendCrystal(amount))
                return false;
            
            // Firebase 로그 (구현 예정)
            await _crystalService.LogCrystalSpendAsync(amount, purpose);
            
            Debug.Log($"크리스탈 소모: -{amount} ({purpose}), 총: {RORP_Crystal.CurrentValue}");
            return true;
        }
        
        /// <summary>
        /// 크리스탈 소모 가능 여부
        /// </summary>
        public bool CanSpend(int amount)
        {
            return _crystalPolicy.CanSpendCrystal(RORP_Crystal.CurrentValue, amount);
        }
        
        /// <summary>
        /// 크리스탈 시스템 초기화
        /// </summary>
        public async UniTask InitializeAsync()
        {
            // Firebase에서 데이터 로드 (구현 예정)
            var crystalData = await _crystalService.LoadCrystalDataAsync();
            if (crystalData != null)
            {
                _crystalModel.SetFreeCrystal(crystalData.freeCrystal);
                _crystalModel.SetPaidCrystal(crystalData.paidCrystal);
            }
            else
            {
                // 기본값 설정
                _crystalModel.SetFreeCrystal(100);
                _crystalModel.SetPaidCrystal(0);
            }
            
            Debug.Log($"크리스탈 시스템 초기화 완료. 총 크리스탈: {RORP_Crystal.CurrentValue}");
        }
        
        /// <summary>
        /// 서버와 동기화
        /// </summary>
        public async UniTask<bool> SyncWithServerAsync()
        {
            return await _crystalService.SyncCrystalWithServerAsync();
        }
    }
}