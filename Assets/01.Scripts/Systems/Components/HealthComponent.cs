using UnityEngine;
using Game.Models;
using Zenject;
using UnityEngine.EventSystems;
using Game.Core.Event;
using Game.Core;
using System;
using Cysharp.Threading.Tasks;
using R3;
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

        private CompositeDisposable _Initdisposable = new(); // 초기화용 1회성
        private CompositeDisposable _disposable = new();

        // 해제
        private void OnDestroy() {
            _healthSystem.UnregisterCharacter(gameObject.GetInstanceID());
            _Initdisposable?.Dispose();
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
                // UI 생성 Bind
                EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenHealthUI).AddTo(_Initdisposable);
                EventBus.Publish(new UICreationEventAsync(id, UIName.Health_UI));

                // UI Hide
                EventBus.Subscribe<CharacterDeathEvent>(OnDeathHideUI).AddTo(_disposable);
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
                _Initdisposable?.Dispose(); 
            }
        }

        // 죽으면 View를 Hide상태로 변경
        private void OnDeathHideUI(CharacterDeathEvent deathEvent) {
            if (deathEvent.characterID == gameObject.GetInstanceID()) {
                _uiObject.SetActive(false);
            }
        }
    }
}
