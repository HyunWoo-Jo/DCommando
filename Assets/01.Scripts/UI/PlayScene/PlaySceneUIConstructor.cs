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
            _ = _manager.OpenScreenMoveToAnchorAsync<GoldView>(UIName.Gold_UI);

            _ = _manager.OpenScreenMoveToAnchorAsync<CrystalView>(UIName.Crystal_UI);

            _ = _manager.OpenScreenMoveToAnchorAsync<ControllerView>(UIName.MoveController_UI);

            _ = _manager.OpenScreenMoveToAnchorAsync<PauseButtonUI>(UIName.Pause_UI);
        }
    }

}