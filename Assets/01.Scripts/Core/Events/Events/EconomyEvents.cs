using UnityEngine;

namespace Game.Core.Event {
    /// <summary>
    /// 골드 획득후 발생하는 이벤트
    /// </summary>
    public readonly struct GoldGainedEvent {
        public readonly int amount;

        public GoldGainedEvent(int amount) {
            this.amount = amount;
        }
    }

    /// <summary>
    /// 골드 보상 이벤트
    /// </summary>
    public readonly struct GoldRewardEvent {
        public readonly int amount;
        public GoldRewardEvent(int amount) {
            this.amount = amount;
        }

    }

    /// <summary>
    /// 골드가 땅에 드롭요청이 되었을때 발생하는 이벤트
    /// </summary>
    public readonly struct GoldDropRequestEvent {
        public readonly Vector3 position;
        public readonly int amount;

        public GoldDropRequestEvent(Vector3 position, int amount) {
            this.position = position;
            this.amount = amount;
        }
    }

}