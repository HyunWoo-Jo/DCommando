using UnityEngine;
using Game.Model;
using Zenject;
using Game.Models;
using UnityEngine.EventSystems;
using Game.Core.Event;
using Game.Core;
namespace Game.Systems {
    /// <summary>
    /// Health �� �������ִ� Component
    /// </summary>
    public class HealthComponent : MonoBehaviour {
        [Inject] private HealthModel.Factory _modelFactory;
        private HealthModel _healthModel;


        [SerializeField] private Vector2 _offset;
        [SerializeField] private bool _isUseUI = true; // UI ��� ����
        
        public HealthModel HealthModel => _healthModel;

        private void Awake() {
            // Factroy�� ����
            _healthModel = _modelFactory.Create();
        }
        private void Start() {
            if (_isUseUI) {
                EventBus.Publish(new UICreationEvent(UIName.Health_UI, (obj) => {
                    obj.GetComponent<IHealthInjecter>().InjectHealth(_healthModel, this.gameObject, _offset);
                }));
            }
        }

    }
}
