using Game.Core;
using Game.Core.Event;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI {
    public class PauseButtonUI : MonoBehaviour {
        [SerializeField] private Button _button;
        [Inject] private ITimeManager _timeManager;
        [Inject] private IEventBus _eventBus;

        private void Awake() {
            _button.onClick.AddListener(OnPause);
        }

        private void OnPause() {
            _timeManager.Pause();
            _eventBus.Publish(new UICreationEvent(UIName.PausePanel_UI, UIType.Popup, null));
        }
    }
}