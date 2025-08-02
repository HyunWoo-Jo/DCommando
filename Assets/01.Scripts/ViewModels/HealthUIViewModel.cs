using Zenject;
using System;
using R3;
using Game.Models;

namespace Game.ViewModels
{
    public class HealthUIViewModel 
    {
        [Inject] private HealthUIModel _healthModel;
        
        // ReactiveProperty
        private readonly ReactiveProperty<int> RP_currentHealth = new ReactiveProperty<int>();
        
        // ReadOnlyReactiveProperty - RORP_ 접두사
        private readonly ReadOnlyReactiveProperty<int> RORP_maxHealth;
        private readonly ReadOnlyReactiveProperty<float> RORP_healthRatio;
        private readonly ReadOnlyReactiveProperty<bool> RORP_isLowHealth;
        private readonly ReadOnlyReactiveProperty<bool> RORP_isDead;
        
        // 상수
        private const float LOW_HEALTH_THRESHOLD = 0.3f;
        
        // 공개 속성
        public ReadOnlyReactiveProperty<int> CurrentHealth => RP_currentHealth.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<int> MaxHealth => RORP_maxHealth;
        public ReadOnlyReactiveProperty<float> HealthRatio => RORP_healthRatio;
        public ReadOnlyReactiveProperty<bool> IsLowHealth => RORP_isLowHealth;
        public ReadOnlyReactiveProperty<bool> IsDead => RORP_isDead;
        
        // 생성자에서 ReadOnlyReactiveProperty 초기화
        public HealthUIViewModel()
        {
            // 최대 체력 (모델에서 읽기 전용으로 가져옴)
            RORP_maxHealth = _healthModel.MaxHealth;
            
            // 체력 비율 계산 - R3 올바른 사용법
            RORP_healthRatio = RP_currentHealth
                .CombineLatest(RORP_maxHealth, (current, max) => max > 0 ? (float)current / max : 0f)
                .ToReadOnlyReactiveProperty();
            
            // 저체력 상태 계산
            RORP_isLowHealth = RORP_healthRatio
                .Select(ratio => ratio <= LOW_HEALTH_THRESHOLD && ratio > 0f)
                .ToReadOnlyReactiveProperty();
            
            // 사망 상태 계산
            RORP_isDead = RP_currentHealth
                .Select(health => health <= 0)
                .ToReadOnlyReactiveProperty();
        }
        
        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() 
        {
            // 모델에서 현재 체력 동기화
            RP_currentHealth.Value = _healthModel.CurrentHealth.CurrentValue;
            
            // 모델의 체력 변화 구독
            _healthModel.CurrentHealth
                .Subscribe(health => RP_currentHealth.Value = health);
        }
        
        // 체력 관련 명령
        public void TakeDamage(int damage)
        {
            _healthModel.TakeDamage(damage);
        }
        
        public void Heal(int healAmount)
        {
            _healthModel.Heal(healAmount);
        }
        
        public void SetMaxHealth(int maxHealth)
        {
            _healthModel.SetMaxHealth(maxHealth);
        }
        
        public void FullHeal()
        {
            _healthModel.FullHeal();
        }
        
        // 상태 확인 메서드
        public bool CanTakeDamage()
        {
            return !RORP_isDead.CurrentValue;
        }
        
        public bool CanHeal()
        {
            return !RORP_isDead.CurrentValue && RP_currentHealth.CurrentValue < RORP_maxHealth.CurrentValue;
        }
        
        public float GetHealthPercentage()
        {
            return RORP_healthRatio.CurrentValue * 100f;
        }
        
        public int GetMissingHealth()
        {
            return RORP_maxHealth.CurrentValue - RP_currentHealth.CurrentValue;
        }
    }
}