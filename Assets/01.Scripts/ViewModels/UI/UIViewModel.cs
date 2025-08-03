using Zenject;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Systems;
using Game.Core;

namespace Game.ViewModels
{
    public class UIViewModel 
    {
        [Inject] private UISystem _uiSystem;

        /// <summary>
        /// Screen UI 열기
        /// </summary>
        public async UniTask<T> OpenUIAsync<T>(int id, UIName uiName) where T : Component
        {
            return await _uiSystem.CreateUIAsync<T>(id, uiName);
        }


        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(UIName uiName, GameObject hudUiObj = null)
        {
            _uiSystem.CloseUI(uiName, hudUiObj);
        }

    }
}