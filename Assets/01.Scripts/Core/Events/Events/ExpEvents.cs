namespace Game.Core.Event {
    // 경험치 획득 이벤트(경험치 처리후 발생)
    public readonly struct ExpGainedEvent {
        public readonly int amount;
        public readonly int currentExp;

        public ExpGainedEvent(int amount, int currentExp) {
            this.amount = amount;
            this.currentExp = currentExp;
        }
    }

    // 레벨업 이벤트
    public readonly struct LevelUpEvent {
        public readonly int newLevel;
        public readonly int previousLevel;

        public LevelUpEvent(int newLevel, int previousLevel) {
            this.newLevel = newLevel;
            this.previousLevel = previousLevel;
        }
    }

    // 최대 레벨 도달 이벤트
    public readonly struct MaxLevelReachedEvent {
        public readonly int maxLevel;

        public MaxLevelReachedEvent(int maxLevel) {
            this.maxLevel = maxLevel;
        }
    }

    // 경험치 변경 이벤트
    public readonly struct ExpChangedEvent {
        public readonly int currentExp;
        public readonly int maxExp;
        public readonly float progress;

        public ExpChangedEvent(int currentExp, int maxExp, float progress) {
            this.currentExp = currentExp;
            this.maxExp = maxExp;
            this.progress = progress;
        }
    }

    // 외부에서 경험치 보상 요청 이벤트
    public readonly struct ExpRewardEvent {
        public readonly int amount;

        public ExpRewardEvent(int amount) {
            this.amount = amount;
        }
    }
}