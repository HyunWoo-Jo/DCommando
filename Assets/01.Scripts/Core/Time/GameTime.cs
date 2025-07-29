using UnityEngine;

namespace Game.Core
{
    // 편의를 위한 Static
    public static class GameTime {
        private static ITimeManager _timeManager = new TimeManager();

        public static void ChangeTimeManager(ITimeManager timeManager) => _timeManager = timeManager;

        public static float TimeScale => _timeManager.TimeScale;
        public static bool IsPaused => _timeManager.IsPaused;
        public static float DeltaTime => IsPaused ? 0f : UnityEngine.Time.deltaTime;
        public static void Pause() => _timeManager.Pause();

        public static void Resume() => _timeManager.Resume();

        public static void SetTimeScale(float scale) => _timeManager.SetTimeScale(scale);
    }
}
