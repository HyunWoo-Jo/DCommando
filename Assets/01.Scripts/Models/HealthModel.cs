using R3;
using Zenject;

namespace Game.Models
{
    public class HealthModel
    {
        public class Factory : PlaceholderFactory<HealthModel> { }

        // 상수
        private const int MIN_HP = 0;
        private const int DEFAULT_MAX_HP = 100;
        private const int REVIVE_MIN_HP = 1;

        private readonly ReactiveProperty<int> RP_maxHp = new(DEFAULT_MAX_HP);
        private readonly ReactiveProperty<int> RP_currentHp = new(DEFAULT_MAX_HP);
        private readonly ReactiveProperty<bool> RP_isDead = new(false);
        
        public ReadOnlyReactiveProperty<int> RORP_MaxHp => RP_maxHp;
        public ReadOnlyReactiveProperty<int> RORP_CurrentHp => RP_currentHp;
        public ReadOnlyReactiveProperty<bool> RORP_IsDead => RP_isDead;

        // 기본 체력 설정
        public void SetMaxHp(int hp)
        {
            RP_maxHp.Value = hp;
            // 현재 체력이 최대 체력을 초과하지 않도록
            if (RP_currentHp.Value > hp)
            {
                RP_currentHp.Value = hp;
            }
            CheckDead();
        }

        public void SetCurrentHp(int hp)
        {
            RP_currentHp.Value = UnityEngine.Mathf.Clamp(hp, MIN_HP, RP_maxHp.Value);
            CheckDead();
        }

        // 데미지 받기
        public void TakeDamage(int damage)
        {
            if (RP_isDead.Value) return;
            
            RP_currentHp.Value = UnityEngine.Mathf.Max(MIN_HP, RP_currentHp.Value - damage);
            CheckDead();
        }

        // 치료
        public void Heal(int healAmount)
        {
            if (RP_isDead.Value) return;
            
            RP_currentHp.Value = UnityEngine.Mathf.Min(RP_maxHp.Value, RP_currentHp.Value + healAmount);
            CheckDead();
        }

        // 부활
        public void Revive(int reviveHp = -1)
        {
            int hp = reviveHp == -1 ? RP_maxHp.Value : reviveHp;
            RP_currentHp.Value = UnityEngine.Mathf.Clamp(hp, REVIVE_MIN_HP, RP_maxHp.Value);
            RP_isDead.Value = false;
        }

        // 체력 비율
        public float GetHpRatio()
        {
            return RP_maxHp.Value == MIN_HP ? 0f : (float)RP_currentHp.Value / RP_maxHp.Value;
        }

        // 사망 체크
        private void CheckDead()
        {
            RP_isDead.Value = RP_currentHp.Value <= MIN_HP;
        }

        // 초기화
        public void Initialize(int maxHp = DEFAULT_MAX_HP)
        {
            RP_maxHp.Value = maxHp;
            RP_currentHp.Value = maxHp;
            RP_isDead.Value = false;
        }
    }
}