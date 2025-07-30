using UnityEngine;
using R3;
using Game.Core.Event;
namespace Game.Core
{
    internal class TimeManager : ITimeManager {
   
        private float _timeScale = 1f;
        private readonly ReactiveProperty<bool> RP_isPaused = new (false);
        private CompositeDisposable _disposables = new();
        public float TimeScale { get => _timeScale; }

        public bool IsPaused { get => RP_isPaused.CurrentValue; }

        public TimeManager() {
            // Event 발생 셋팅
            RP_isPaused
                .Skip(1) 
                .Subscribe(PublicEvent)
                .AddTo(_disposables);
        }


        /// <summary>
        /// Event 발행
        /// </summary>
        /// <param name="isPaused"></param>
        private void PublicEvent(bool isPaused) {
            if (isPaused) {
                EventBus.Publish(new PauseGameEvent());
            } else {
                EventBus.Publish(new ResumeGameEvent());
            }
        }

        public void Pause() {
            RP_isPaused.Value = true;
            
        }

        public void Resume() {
            RP_isPaused.Value = false;
        }

        public  void SetTimeScale(float scale) {
            _timeScale = scale;
        }
    }
}
