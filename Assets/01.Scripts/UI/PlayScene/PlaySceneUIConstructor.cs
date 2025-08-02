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
        [SerializeField] private UIManager _manager;
        private void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(_manager);
#endif
            // UI 생성, 이동
            _ = _manager.OpenUIMoveToAnchorAsync<GoldView>(UIName.Gold_UI);

            _ = _manager.OpenUIMoveToAnchorAsync<CrystalView>(UIName.Crystal_UI);

            _ = _manager.OpenUIMoveToAnchorAsync<ControllerView>(UIName.MoveController_UI);

            _ = _manager.OpenUIMoveToAnchorAsync<PauseButtonUI>(UIName.Pause_UI);
        }
    }

}