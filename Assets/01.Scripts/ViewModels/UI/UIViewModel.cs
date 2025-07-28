using Zenject;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Systems;
using Game.Core;

namespace ViewModels
{
    public class UIViewModel 
    {
        [Inject] private UISystem _uiSystem;

        /// <summary>
        /// Screen UI 열기
        /// </summary>
        public async UniTask<T> OpenScreenAsync<T>(UI_Name uiName) where T : Component
        {
            return await _uiSystem.OpenScreenAsync<T>(uiName);
        }

        /// <summary>
        /// Popup UI 열기
        /// </summary>
        public async UniTask<T> OpenPopupAsync<T>(UI_Name uiName) where T : Component
        {
            return await _uiSystem.OpenPopupAsync<T>(uiName);
        }

        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(UI_Name uiName)
        {
            _uiSystem.CloseUI(uiName);
        }

        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() 
        {
            // UI 상태 변경 알림 로직
        }
    }
}