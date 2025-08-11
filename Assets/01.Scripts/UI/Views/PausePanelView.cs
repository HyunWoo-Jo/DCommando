
using UnityEngine;
using Zenject;
using System;
using Game.ViewModels;
using UnityEngine.UI;
using Game.Core.Event;
using Game.Core;

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
            Resize();
        }

#if UNITY_EDITOR
        // 검증
        private void RefAssert() {

        }
#endif
        private void Bind() {
            _continueButton.onClick
                .AddListener(() => _viewModel.OnContinueButton());

            _giveupButton.onClick
                .AddListener(() => _sceneViewModel.LoadSceneWithLoading(SceneName.MainScene, 0.5f));
        }

        private void Resize() {
            RectTransform rect = GetComponent<RectTransform>();
            // 앵커를 이용해 부모 전체 영역을 차지
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }

        // UI 갱신
        private void UpdateUI() {
            
        }

    }
}
