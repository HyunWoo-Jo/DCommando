
using UnityEngine;
using Zenject;
using System;
using Game.ViewModels;
using UnityEngine.UI;
using Game.Core.Event;
using Game.Core;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class PausePanelView : MonoBehaviour
    {
        [Inject] private SceneViewModel _sceneViewModel;
        [Inject] private PausePanelViewModel _viewModel;
        [Header("Unity 레퍼")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _giveupButton;
        

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
            Assert.IsNotNull(_continueButton);
            Assert.IsNotNull(_giveupButton);
        }
#endif
        private void Bind() {
            _continueButton.onClick
                .AddListener(() => _viewModel.OnContinueButton());

            _giveupButton.onClick
                .AddListener(() => _sceneViewModel.LoadSceneWithLoading(SceneName.MainScene, 0.5f));
        }


    }
}
