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

        [SerializeField] private HealthData _healthData;

        [Inject] private readonly HealthModel _healthModel;
        [Inject] private readonly HealthSystem _healthSystem;

        [SerializeField] private Vector2 _offset;
        [SerializeField] private bool _isUseUI = true; // UI 사용 여부
        

        private IDisposable _disposable;

        private void Start() {
            int id = gameObject.GetInstanceID();
            _healthSystem.RegisterCharacter(id, _healthData.maxHp);
            _healthSystem.SetCurrentHp(id, _healthData.currentHp);

            if (_isUseUI) {
                _disposable = EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenHealthUI);
                EventBus.Publish(new UICreationEvent(id, UIName.Health_UI));
            }
            
        }

        private void OnOpenHealthUI(UIOpenedNotificationEvent openEvent) {
            if (openEvent.id == gameObject.GetInstanceID() && openEvent.uiName == UIName.Health_UI) {
                openEvent.uiObject.GetComponent<IHealthInitializable>().InitHealth(this.gameObject, _offset);
                _disposable?.Dispose();
            }
        }
    }
}
