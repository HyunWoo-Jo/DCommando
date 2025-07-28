using Zenject;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Systems;

namespace ViewModels
{
    public class UIViewModel 
    {
        [Inject] private UISystem _uiSystem;

        /// <summary>
        /// Screen UI 열기
        /// </summary>
        public async UniTask<T> OpenScreenAsync<T>(string uiName) where T : Component
        {
            return await _uiSystem.OpenScreenAsync<T>(uiName);
        }

        /// <summary>
        /// Popup UI 열기
        /// </summary>
        public async UniTask<T> OpenPopupAsync<T>(string uiName) where T : Component
        {
            return await _uiSystem.OpenPopupAsync<T>(uiName);
        }

        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(string uiName)
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