using UnityEngine;

namespace Game.Core
{
    public interface ITimeManager {
        float TimeScale { get; }
        bool IsPaused { get; }

        void Pause();
        void Resume();
        void SetTimeScale(float scale);
    }
}
