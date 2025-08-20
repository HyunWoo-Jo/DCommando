using UnityEngine;
using Game.Core;
using Zenject;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using Game.Core.Event;
using System.Collections.Generic;
namespace Game.UI {
    /// <summary>
    /// UI를 생성함
    /// </summary>
    [DefaultExecutionOrder(1000)] // 모든 이벤트가 구독되고 실행이 되도록 늦게 실행
    public class UIConstructor : MonoBehaviour {

        [Header("생성할 UI")]
        [SerializeField] private List<UIName> _uiNames = new();
        
        private void Awake() {
            int id = gameObject.GetInstanceID();
            // UI 생성, 이동
            foreach (var name in _uiNames) {
                EventBus.Publish(new UICreationEvent(id, name));
            }
            Destroy(gameObject);
        }
    }

}