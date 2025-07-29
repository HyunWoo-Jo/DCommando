using UnityEngine;
using Zenject;
using Game.ViewModels;
using Cysharp.Threading.Tasks;
using Game.Core;
using System.Collections.Generic;

namespace Game.UI
{
    /// <summary>
    /// UI 매니저 - ViewModel을 통한 UI 생성 및 관리
    /// 이 컴포넌트는 Main Cavnas에 달려야함
    /// </summary>
    public class UI_Manager : MonoBehaviour
    {
        [Inject] private UIViewModel _viewModel;
        
        private static UI_Manager _instance;
        public static UI_Manager Instance => _instance;

        private Dictionary<UI_Name, UIAnchor> _uiAnchorDictionary = new();


        private void Awake()
        {
            var anchorList = GetComponentsInChildren<UIAnchor>();

            foreach (var anchor in anchorList) {
                _uiAnchorDictionary.Add(anchor.Ui_Name, anchor);
            }
        }

        /// <summary>
        /// Anchor 위치로 이동
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="pos"></param>
        public void MoveToAnchor(UI_Name uiName, Transform pos) {
            if(_uiAnchorDictionary.TryGetValue(uiName, out var ancher)) {
                pos.position = ancher.AnchorTransform.position;
            }
        }


        /// <summary>
        /// Screen UI 열기
        /// </summary>
        public async UniTask<T> OpenScreenAsync<T>(UI_Name uiName) where T : Component
        {
            return await _viewModel.OpenScreenAsync<T>(uiName);
        }
        
        /// <summary>
        /// Popup UI 열기
        /// </summary>
        public async UniTask<T> OpenPopupAsync<T>(UI_Name uiName) where T : Component
        {
            return await _viewModel.OpenPopupAsync<T>(uiName);
        }
        
        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(UI_Name uiName)
        {
            _viewModel.CloseUI(uiName);
        }
        
    }
}