using UnityEngine;
using UnityEngine.UI;
using Game.ViewModels;
using R3;
using System;
using Cysharp.Threading.Tasks;

namespace Game.UI
{
    public class GoldView : MonoBehaviour
    {
        [Header("UI 컴포넌트")]
        [SerializeField] private Text _goldText;
        [SerializeField] private Button _testAddButton;
        [SerializeField] private Button _testSpendButton;
        
        private GoldViewModel _viewModel;
        private CompositeDisposable _disposables = new();
        
        public void Initialize(GoldViewModel viewModel)
        {
            _viewModel = viewModel;
            BindViewModel();
        }
        
        private void BindViewModel()
        {
            // 골드 텍스트 바인딩
            _viewModel.GoldText
                .Subscribe(text => _goldText.text = $"Gold: {text}")
                .AddTo(_disposables);
            
            // 알림 이벤트 바인딩
            _viewModel.OnNotificationEvent
                .Subscribe(message => Debug.Log($"[Gold] {message}"))
                .AddTo(_disposables);
            
            // 버튼 이벤트 바인딩 (UniTaskVoid 사용)
            _testAddButton.onClick.AddListener(() => OnAddGoldClicked().Forget());
            _testSpendButton.onClick.AddListener(() => OnSpendGoldClicked().Forget());
        }
        
        private async UniTaskVoid OnAddGoldClicked()
        {
            await _viewModel.AddGoldAsync(100);
        }
        
        private async UniTaskVoid OnSpendGoldClicked()
        {
            await _viewModel.SpendGoldAsync(50);
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
            _viewModel?.Dispose();
        }
    }
}