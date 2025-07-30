
using UnityEngine;
using Zenject;
using System;
using Game.ViewModels;
using UnityEngine.UI;
using Game.Core.Event;
////////////////////////////////////////////////////////////////////////////////////
// Auto Generated Code
namespace Game.UI
{
    public class PausePanelView : MonoBehaviour
    {
        [Inject] private PausePanelViewModel _viewModel;
        [Header("Unity 레퍼")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _giveupButton;
        

        private void Awake() {
#if UNITY_EDITOR // Assertion
            RefAssert();
#endif
            Bind();
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
        }

        private void Resize() {
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height);
        }

        // UI 갱신
        private void UpdateUI() {
            
        }
////////////////////////////////////////////////////////////////////////////////////
        // your logic here

    }
}
