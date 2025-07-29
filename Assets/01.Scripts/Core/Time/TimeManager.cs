using UnityEngine;

namespace Game.Core
{
    public class TimeManager : ITimeManager {

        private float _timeScale = 1f;
        private bool _isPaused = false;

        public float TimeScale { get => _timeScale; }

        public bool IsPaused { get => _isPaused; }

        public void Pause() {
            _isPaused = true;
        }

        public void Resume() {
            _isPaused = false;
        }

        public void SetTimeScale(float scale) {
            _timeScale = scale;
        }
    }
}
