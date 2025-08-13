using UnityEngine;

namespace Game.Core.Event
{
    /// <summary>
    /// 골드 획득 이벤트
    /// </summary>
    public readonly struct GoldGainedEvent {
        public readonly int amount;

        public GoldGainedEvent(int amount) {
            this.amount = amount;
        }
    }
}
