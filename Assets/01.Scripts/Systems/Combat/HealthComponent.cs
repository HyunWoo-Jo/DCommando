using UnityEngine;
using Game.Models;
using Zenject;
using UnityEngine.EventSystems;
using Game.Core.Event;
using Game.Core;
using System;
namespace Game.Systems {
    /// <summary>
    /// Health 를 관리해주는 Component
    /// </summary>
    public class HealthComponent : MonoBehaviour {

        [SerializeField] private HealthData _healthData;

        [Inject] private readonly HealthSystem _healthSystem;

        [SerializeField] private Vector2 _offset;
        [SerializeField] private bool _isUseUI = true; // UI 사용 여부

        private GameObject _uiObject;

        private IDisposable _disposable;

        // 해제
        private void OnDestroy() {
            _healthSystem.UnregisterCharacter(gameObject.GetInstanceID());
            _disposable?.Dispose();
            if (_uiObject != null) {
                EventBus.Publish(new UICloseEvent(gameObject.GetInstanceID(), UIName.Health_UI, _uiObject));
            }
        }

        private void Start() {
            int id = gameObject.GetInstanceID();
            _healthSystem.RegisterCharacter(id, _healthData.maxHp);
            _healthSystem.SetCurrentHp(id, _healthData.currentHp);

            if (_isUseUI) {
                _disposable = EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenHealthUI);
                EventBus.Publish(new UICreationEvent(id, UIName.Health_UI));
            }
            
        }

        /// <summary>
        /// UI 생성시 호출
        /// </summary>
        /// <param name="openEvent"></param>
        private void OnOpenHealthUI(UIOpenedNotificationEvent openEvent) {
            if (openEvent.id == gameObject.GetInstanceID() && openEvent.uiName == UIName.Health_UI) {
                _uiObject = openEvent.uiObject;
                openEvent.uiObject.GetComponent<IHealthInitializable>().InitHealth(this.gameObject, _offset);
                _disposable?.Dispose();
            }
        }
    }
}
