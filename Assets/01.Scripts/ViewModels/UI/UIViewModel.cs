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
        public void CloseUI(int id, UIName uiName, GameObject hudUiObj = null)
        {
            _uiSystem.CloseUI(id, uiName, hudUiObj);
        }

        /// <summary>
        /// Damage Prefab을 로드
        /// </summary>
        /// <returns></returns>
        public async UniTask<GameObject> LoadDamageUIPrefabAsync() {
            return await _uiSystem.LoadPrefabsAsync(UIName.Damage_UI);
        }

        public void ReleaseDamageUI() {
            _uiSystem.ReleasePrefab(UIName.Damage_UI);
        }

        /// <summary>
        /// UI 부모를 가지고옴
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Transform GetParent(UIType name) {
            return _uiSystem.GetUIParent(name);
        }

    }
}