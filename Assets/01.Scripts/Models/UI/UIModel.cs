using UnityEngine;
using R3;
using Game.Core;

namespace Game.Models
{
    /// <summary>
    /// UI 상태 관리 모델
    /// </summary>
    public class UIModel
    {
        private readonly ReactiveProperty<UI_Name> _currentScreen = new(UI_Name.None);
        private readonly ReactiveProperty<bool> _isAnyPopupOpen = new(false);
        private readonly ReactiveProperty<bool> _isLoading = new(false);
        
        public ReadOnlyReactiveProperty<UI_Name> CurrentScreen => _currentScreen;
        public ReadOnlyReactiveProperty<bool> IsAnyPopupOpen => _isAnyPopupOpen;
        public ReadOnlyReactiveProperty<bool> IsLoading => _isLoading;
        
        public void SetCurrentScreen(UI_Name screenName)
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