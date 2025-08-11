using Game.ViewModels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Game.Core;
using UnityEngine.Assertions;
namespace Game.UI
{
    public class PlayButtonUI : MonoBehaviour
    {
        [Inject] private SceneViewModel _sceneViewModel;
        [SerializeField] private Button _button;

        private void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(_button, "button ¿Ã «“¥Á æ»µ ");
#endif

            _button.onClick.AddListener(OnClick);
        }

        private void OnClick() {
            _sceneViewModel.LoadSceneWithLoading(SceneName.PlayScene, 0.5f);
        }
    }
}
