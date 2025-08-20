using UnityEngine;

namespace Game.Core
{
    public enum WipeDirection {
        Right,
        Left,
        FillLeft, // 채워져있을때 0 방향으로 <-
        FillRight, // ->
    }
    public interface IWipeUI {
        void Wipe(WipeDirection direction, float targetTime, bool isAutoActiveClose);
    }
}
