using System;
using R3;
using UnityEngine;

namespace Game.Models
{
    public class HealthUIModel 
    {
        // ReactiveProperty - RP_ 접두사 사용
        private readonly ReactiveProperty<int> RP_currentHealth = new ReactiveProperty<int>(100);
        private readonly ReactiveProperty<int> RP_maxHealth = new ReactiveProperty<int>(100);
        
        // ReadOnlyReactiveProperty - RORP_ 접두사
        private readonly ReadOnlyReactiveProperty<int> RORP_currentHealth;
        private readonly ReadOnlyReactiveProperty<int> RORP_maxHealth;
        
        // 상수 - UPPER_CASE
        private const int MIN_HEALTH = 0;
        private const int DEFAULT_MAX_HEALTH = 100;
        
        // 공개 속성 - PascalCase  
        public ReadOnlyReactiveProperty<int> CurrentHealth => RORP_currentHealth;
        public ReadOnlyReactiveProperty<int> MaxHealth => RORP_maxHealth;
        
        // 이벤트
        public event Action<int> OnHealthChanged;
        public event Action<int> OnMaxHealthChanged;
        public event Action<int, int> OnDamageTaken; // (damage, remainingHealth)
        public event Action<int, int> OnHealed; // (healAmount, currentHealth)
        public event Action OnDied;
        public event Action OnRevived;
        
        // 속성 - PascalCase
        public bool IsDead => RP_currentHealth.CurrentValue <= MIN_HEALTH;
        public bool IsFullHealth => RP_currentHealth.CurrentValue >= RP_maxHealth.CurrentValue;
        public float HealthRatio => RP_maxHealth.CurrentValue > 0 ? (float)RP_currentHealth.CurrentValue / RP_maxHealth.CurrentValue : 0f;
        
        public HealthUIModel()
        {
            // 초기값 설정
            RP_maxHealth.Value = DEFAULT_MAX_HEALTH;
            RP_currentHealth.Value = DEFAULT_MAX_HEALTH;
            
            // ReadOnlyReactiveProperty 초기화
            RORP_currentHealth = RP_currentHealth.ToReadOnlyReactiveProperty();
            RORP_maxHealth = RP_maxHealth.ToReadOnlyReactiveProperty();
            
            // 변화 감지
            RP_currentHealth.Subscribe(OnCurrentHealthChanged);
            RP_maxHealth.Subscribe(OnMaxHealthChangedInternal);
        }
        
        // 체력 변경
        public void TakeDamage(int damage)
        {
            if (damage <= 0 || IsDead) return;
            
            bool wasDead = IsDead;
            int previousHealth = RP_currentHealth.CurrentValue;
            RP_currentHealth.Value = Mathf.Max(MIN_HEALTH, RP_currentHealth.CurrentValue - damage);
            
            OnDamageTaken?.Invoke(damage, RP_currentHealth.CurrentValue);
            
            // 사망 처리
            if (!wasDead && IsDead)
            {
                OnDied?.Invoke();
            }
        }
        
        public void Heal(int healAmount)
        {
            if (healAmount <= 0) return;
            
            bool wasDead = IsDead;
            RP_currentHealth.Value = Mathf.Min(RP_maxHealth.CurrentValue, RP_currentHealth.CurrentValue + healAmount);
            
            OnHealed?.Invoke(healAmount, RP_currentHealth.CurrentValue);
            
            // 부활 처리
            if (wasDead && !IsDead)
            {
                OnRevived?.Invoke();
            }
        }
        
        public void SetMaxHealth(int newMaxHealth)
        {
            if (newMaxHealth <= 0) return;
            
            RP_maxHealth.Value = newMaxHealth;
            
            // 현재 체력이 새로운 최대치보다 크면 조정
            if (RP_currentHealth.CurrentValue > newMaxHealth)
            {
                RP_currentHealth.Value = newMaxHealth;
            }
        }
        
        public void FullHeal()
        {
            bool wasDead = IsDead;
            RP_currentHealth.Value = RP_maxHealth.CurrentValue;
            
            if (wasDead)
            {
                OnRevived?.Invoke();
            }
        }
        
        public void SetCurrentHealth(int health)
        {
            bool wasDead = IsDead;
            RP_currentHealth.Value = Mathf.Clamp(health, MIN_HEALTH, RP_maxHealth.CurrentValue);
            
            if (wasDead && !IsDead)
            {
                OnRevived?.Invoke();
            }
            else if (!wasDead && IsDead)
            {
                OnDied?.Invoke();
            }
        }
        
        // 상태 확인
        public bool CanTakeDamage()
        {
            return !IsDead;
        }
        
        public bool CanHeal()
        {
            return !IsFullHealth;
        }
        
        public int GetMissingHealth()
        {
            return RP_maxHealth.CurrentValue - RP_currentHealth.CurrentValue;
        }
        
        public float GetHealthPercentage()
        {
            return HealthRatio * 100f;
        }
        
        // 내부 메서드 - private
        private void OnCurrentHealthChanged(int newHealth)
        {
            OnHealthChanged?.Invoke(newHealth);
        }
        
        private void OnMaxHealthChangedInternal(int newMaxHealth)
        {
            OnMaxHealthChanged?.Invoke(newMaxHealth);
        }
        
        // 디버그 메서드
#if UNITY_EDITOR
        public void DebugSetHealth(int health)
        {
            SetCurrentHealth(health);
        }
        
        public void DebugKill()
        {
            SetCurrentHealth(0);
        }
        
        public void DebugRevive()
        {
            FullHeal();
        }
#endif
    }
}