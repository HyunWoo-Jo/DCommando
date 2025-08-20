using Game.Core;
using UnityEngine;
using Zenject;
using Game.ViewModels;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Game.UI {
    public class GameWinView : MonoBehaviour {
        [Inject] private SceneViewModel _sceneViewModel;
        [Inject] private PausePanelViewModel _viewModel;
        [Header("Unity 레퍼")]
        [SerializeField] private Button _goToLobbyButton;
        private void Awake() {
#if UNITY_EDITOR // Assertion
            RefAssert();
#endif
            Bind();
        }
        private void Start() {
            gameObject.Resize();
        }

#if UNITY_EDITOR
        // 검증
        private void RefAssert() {
            Assert.IsNotNull(_goToLobbyButton);
        }
#endif
        private void Bind() {
            _goToLobbyButton.onClick
                .AddListener(() => _sceneViewModel.LoadSceneWithLoading(SceneName.MainScene, 0.5f));
        }
    }
}
