using UnityEngine;
using Game.Core;
using Zenject;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
using Game.Core.Event;
namespace Game.UI {
    /// <summary>
    /// Play Scene
    /// </summary>
    public class PlaySceneUIConstructor : MonoBehaviour {
        private void Awake() {
            int id = gameObject.GetInstanceID();
            // UI 생성, 이동
            EventBus.Publish(new UICreationEvent(id, UIName.Gold_UI));
            EventBus.Publish(new UICreationEvent(id, UIName.Crystal_UI));
            EventBus.Publish(new UICreationEvent(id, UIName.MoveController_UI));
            EventBus.Publish(new UICreationEvent(id, UIName.Pause_UI));
            EventBus.Publish(new UICreationEvent(id, UIName.Exp_UI));
        }
    }

}