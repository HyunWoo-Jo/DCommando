using UnityEngine;
using Game.Core;
using Zenject;
using UnityEngine.Assertions;
using Cysharp.Threading.Tasks;
namespace Game.UI {
    /// <summary>
    /// Play Scene
    /// </summary>
    public class PlaySceneUIConstructor : MonoBehaviour {
        [SerializeField] private UI_Manager _manager;
        private void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(_manager);
#endif
            // UI 생성, 이동
            _manager.OpenScreenAsync<GoldView>(UI_Name.Gold_UI)
                .ContinueWith(t => _manager.MoveToAnchor(UI_Name.Gold_UI, t.transform));

            _ = _manager.OpenScreenAsync<CrystalView>(UI_Name.Crystal_UI)
                .ContinueWith(t => _manager.MoveToAnchor(UI_Name.Crystal_UI, t.transform));

            _ = _manager.OpenScreenAsync<ControllerView>(UI_Name.MoveController_UI)
                .ContinueWith(t => _manager.MoveToAnchor(UI_Name.MoveController_UI, t.transform));
        }
    }

}