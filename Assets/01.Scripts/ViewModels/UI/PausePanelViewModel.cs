
using Zenject;
using System;
using Game.Core.Event;
using Game.Core;
namespace Game.ViewModels
{
    public class PausePanelViewModel {
           
        
        /// <summary>
        /// ��� ��ư �������� ȣ��
        /// </summary>
        public void OnContinueButton() {
            GameTime.Resume();
            EventBus.Publish<UICloseEvent>(new UICloseEvent(Core.UIName.PausePanel_UI));
        }

    }
} 
