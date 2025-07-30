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
        private readonly ReactiveProperty<UIName> RP_currentScreen = new(UIName.None);
        private readonly ReactiveProperty<bool> RP_isAnyPopupOpen = new(false);
        private readonly ReactiveProperty<bool> RP_isLoading = new(false);
        
        public ReadOnlyReactiveProperty<UIName> RORP_CurrentScreen => RP_currentScreen;
        public ReadOnlyReactiveProperty<bool> RORP_IsAnyPopupOpen => RP_isAnyPopupOpen;
        public ReadOnlyReactiveProperty<bool> RORP_IsLoading => RP_isLoading;
        
        public void SetCurrentScreen(UIName screenName)
        {
            RP_currentScreen.Value = screenName;
        }
        
        public void SetPopupState(bool isOpen)
        {
            RP_isAnyPopupOpen.Value = isOpen;
        }
        
        public void SetLoadingState(bool isLoading)
        {
            RP_isLoading.Value = isLoading;
        }
    }
}