using R3;
using System;

namespace Game.Models
{
    /// <summary>
    /// 크리스탈 재화 데이터 모델
    /// </summary>
    public class CrystalModel : IDisposable
    {
        private readonly ReactiveProperty<int> RP_currentCrystal = new(0);
        private readonly ReactiveProperty<int> RP_maxCrystal = new(9999999);
        private readonly ReactiveProperty<int> RP_freeCrystal = new(0);      // 무료 크리스탈
        private readonly ReactiveProperty<int> RP_paidCrystal = new(0);      // 유료 크리스탈
        
        public ReadOnlyReactiveProperty<int> RORP_CurrentCrystal => RP_currentCrystal;
        public ReadOnlyReactiveProperty<int> RORP_MaxCrystal => RP_maxCrystal;
        public ReadOnlyReactiveProperty<int> RORP_FreeCrystal => RP_freeCrystal;
        public ReadOnlyReactiveProperty<int> RORP_PaidCrystal => RP_paidCrystal;
        
        // Zenject에서 관리
        public void Dispose() {
            RP_currentCrystal?.Dispose();
            RP_maxCrystal?.Dispose();
            RP_freeCrystal?.Dispose();
            RP_paidCrystal?.Dispose();
        }

        /// <summary>
        /// 크리스탈 설정 (전체)
        /// </summary>
        public void SetCrystal(int amount)
        {
            RP_currentCrystal.Value = amount;
        }
        
        /// <summary>
        /// 무료 크리스탈 설정
        /// </summary>
        public void SetFreeCrystal(int amount)
        {
            RP_freeCrystal.Value = amount;
            UpdateTotalCrystal();
        }
        
        /// <summary>
        /// 유료 크리스탈 설정
        /// </summary>
        public void SetPaidCrystal(int amount)
        {
            RP_paidCrystal.Value = amount;
            UpdateTotalCrystal();
        }
        
        /// <summary>
        /// 크리스탈 추가 (무료/유료 구분)
        /// </summary>
        public void AddCrystal(int amount, bool isPaid = false)
        {
            if (isPaid)
            {
                RP_paidCrystal.Value += amount;
            }
            else
            {
                RP_freeCrystal.Value += amount;
            }
            UpdateTotalCrystal();
        }
        
        /// <summary>
        /// 크리스탈 소모 (무료 먼저 소모)
        /// </summary>
        public bool SpendCrystal(int amount)
        {
            if (!CanSpend(amount)) return false;
            
            int remainingToSpend = amount;
            
            // 무료 크리스탈부터 소모
            if (RP_freeCrystal.Value > 0)
            {
                int freeToSpend = UnityEngine.Mathf.Min(RP_freeCrystal.Value, remainingToSpend);
                RP_freeCrystal.Value -= freeToSpend;
                remainingToSpend -= freeToSpend;
            }
            
            // 남은 금액을 유료 크리스탈에서 소모
            if (remainingToSpend > 0)
            {
                RP_paidCrystal.Value -= remainingToSpend;
            }
            
            UpdateTotalCrystal();
            return true;
        }
        
        /// <summary>
        /// 최대 크리스탈 설정
        /// </summary>
        public void SetMaxCrystal(int max)
        {
            RP_maxCrystal.Value = max;
        }
        
        /// <summary>
        /// 크리스탈 소모 가능 여부 확인
        /// </summary>
        public bool CanSpend(int amount)
        {
            return RP_currentCrystal.Value >= amount;
        }
        
        /// <summary>
        /// 전체 크리스탈 수 업데이트
        /// </summary>
        private void UpdateTotalCrystal()
        {
            RP_currentCrystal.Value = RP_freeCrystal.Value + RP_paidCrystal.Value;
        }
    }
}