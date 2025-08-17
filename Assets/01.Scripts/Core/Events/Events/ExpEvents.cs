using UnityEngine;

namespace Game.Core.Event {

    /// <summary>
    /// 경험치 획득 이벤트(경험치 처리후 발생)
    /// </summary>
    public readonly struct ExpGainedEvent {
        public readonly int amount;
        public readonly int currentExp;

        public ExpGainedEvent(int amount, int currentExp) {
            this.amount = amount;
            this.currentExp = currentExp;
        }
    }

    /// <summary>
    /// 레벨업 이벤트
    /// </summary>
    public readonly struct LevelUpEvent {
        public readonly int newLevel;
        public readonly int previousLevel;

        public LevelUpEvent(int newLevel, int previousLevel) {
            this.newLevel = newLevel;
            this.previousLevel = previousLevel;
        }
    }

    /// <summary>
    /// 최대 레벨 도달 이벤트
    /// </summary>
    public readonly struct MaxLevelReachedEvent {
        public readonly int maxLevel;

        public MaxLevelReachedEvent(int maxLevel) {
            this.maxLevel = maxLevel;
        }
    }

    /// <summary>
    /// 경험치 변경 이벤트
    /// </summary>
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

    /// <summary>
    /// 외부에서 경험치 보상 요청 이벤트
    /// </summary>
    public readonly struct ExpRewardEvent {
        public readonly int amount;

        public ExpRewardEvent(int amount) {
            this.amount = amount;
        }
    }

    /// <summary>
    /// 경험치 드롭을 요청하는 이벤트
    /// </summary>
    public readonly struct ExpDropRequestEvent {
        public readonly Vector3 position;
        public readonly int amount;

        public ExpDropRequestEvent(Vector3 position, int amount) {
            this.position = position;
            this.amount = amount;
        }
    }
}