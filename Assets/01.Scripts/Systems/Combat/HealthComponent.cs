using UnityEngine;
using Game.Model;
using Zenject;
using Game.Models;
using UnityEngine.EventSystems;
using Game.Core.Event;
using Game.Core;
using System;
namespace Game.Systems {
    /// <summary>
    /// Health 를 관리해주는 Component
    /// </summary>
    public class HealthComponent : MonoBehaviour {
        [Inject] private HealthModel.Factory _modelFactory;
        private HealthModel _healthModel;


        [SerializeField] private Vector2 _offset;
        [SerializeField] private bool _isUseUI = true; // UI 사용 여부
        
        public HealthModel HealthModel => _healthModel;


        private IDisposable _disposable;
        private void Awake() {
            // Factroy로 생성
            _healthModel = _modelFactory.Create();
        }
        private void Start() {
            if (_isUseUI) {
                _disposable = EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenHealthUI);
                EventBus.Publish(new UICreationEvent(gameObject.GetInstanceID(), UIName.Health_UI));
            }
            
        }

        private void OnOpenHealthUI(UIOpenedNotificationEvent openEvent) {
            if (openEvent.id == gameObject.GetInstanceID() && openEvent.uiName == UIName.Health_UI) {
                openEvent.uiObject.GetComponent<IHealthInjecter>().InjectHealth(_healthModel, this.gameObject, _offset);
                _disposable?.Dispose();
            }
        }
    }
}
