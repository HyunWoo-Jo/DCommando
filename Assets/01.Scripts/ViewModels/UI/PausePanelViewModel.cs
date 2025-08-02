
using Zenject;
using System;
using Game.Core.Event;
using Game.Core;
namespace Game.ViewModels
{
    public class PausePanelViewModel {
           
        
        /// <summary>
        /// 계속 버튼 눌렀을때 호출
        /// </summary>
        public void OnContinueButton() {
            GameTime.Resume();
            EventBus.Publish<UICloseEvent>(new UICloseEvent(UIName.PausePanel_UI));
        }

    }
} 
