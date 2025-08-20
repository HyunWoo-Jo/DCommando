using Game.Core;
using Game.Core.Event;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.UI {
    public class PauseButtonUI : MonoBehaviour {
        [SerializeField] private Button _button;

        private void Awake() {
            _button.onClick.AddListener(OnPause);
        }

        private void OnPause() {
            GameTime.Pause();
            EventBus.Publish(new UICreationEvent(gameObject.GetInstanceID(), UIName.PausePanel_UI));
        }
    }
}