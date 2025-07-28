using UnityEngine;
using R3;

namespace Game.Models
{
    /// <summary>
    /// UI 상태 관리 모델
    /// </summary>
    public class UIModel
    {
        private readonly ReactiveProperty<string> _currentScreen = new(string.Empty);
        private readonly ReactiveProperty<bool> _isAnyPopupOpen = new(false);
        private readonly ReactiveProperty<bool> _isLoading = new(false);
        
        public ReadOnlyReactiveProperty<string> CurrentScreen => _currentScreen;
        public ReadOnlyReactiveProperty<bool> IsAnyPopupOpen => _isAnyPopupOpen;
        public ReadOnlyReactiveProperty<bool> IsLoading => _isLoading;
        
        public void SetCurrentScreen(string screenName)
        {
            _currentScreen.Value = screenName;
        }
        
        public void SetPopupState(bool isOpen)
        {
            _isAnyPopupOpen.Value = isOpen;
        }
        
        public void SetLoadingState(bool isLoading)
        {
            _isLoading.Value = isLoading;
        }
    }
}