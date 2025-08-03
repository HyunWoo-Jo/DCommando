using R3;

namespace Game.Models {
    public class ExpModel {
        private readonly ReactiveProperty<int> RP_currentExp = new(0);
        private readonly ReactiveProperty<int> RP_currentLevel = new(1);
        private readonly ReactiveProperty<int> RP_maxExp = new(100);
        private readonly ReactiveProperty<int> RP_maxLevel = new(100);

        public ReadOnlyReactiveProperty<int> RORP_CurrentExp => RP_currentExp;
        public ReadOnlyReactiveProperty<int> RORP_CurrentLevel => RP_currentLevel;
        public ReadOnlyReactiveProperty<int> RORP_MaxExp => RP_maxExp;
        public ReadOnlyReactiveProperty<int> RORP_MaxLevel => RP_maxLevel;

        public void SetExp(int amount) {
            RP_currentExp.Value = amount;
        }

        public void SetLevel(int level) {
            RP_currentLevel.Value = level;
        }

        public void SetMaxExp(int maxExp) {
            RP_maxExp.Value = maxExp;
        }

        public void SetMaxLevel(int maxLevel) {
            RP_maxLevel.Value = maxLevel;
        }

        public bool CanLevelUp() {
            return RP_currentExp.Value >= RP_maxExp.Value && RP_currentLevel.Value < RP_maxLevel.Value;
        }

        public bool IsMaxLevel() {
            return RP_currentLevel.Value >= RP_maxLevel.Value;
        }

        public float GetExpProgress() {
            if (RP_maxExp.Value <= 0) return 0f;
            return (float)RP_currentExp.Value / RP_maxExp.Value;
        }
    }
}