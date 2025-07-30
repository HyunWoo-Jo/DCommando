using UnityEngine;
using Zenject;
using Game.ViewModels;
using Cysharp.Threading.Tasks;
using Game.Core;
using System.Collections.Generic;
using Game.Core.Event;
using UnityEngine.UI;
using System;
using R3;
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

        private Dictionary<UIName, UIAnchor> _uiAnchorDictionary = new();

        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            var anchorList = GetComponentsInChildren<UIAnchor>();

            foreach (var anchor in anchorList) {
                _uiAnchorDictionary.Add(anchor.Ui_Name, anchor);
            }
            _disposables.Add(EventBus.Subscribe<UICreationEvent>(CreationEvent));
            _disposables.Add(EventBus.Subscribe<UICloseEvent>(CloseEvent));
        }
        private void OnDestroy() {
            _disposables?.Dispose();
        }

        public async void CreationEvent(UICreationEvent creationEvent) {
            GameDebug.Log($"{creationEvent.uiName} Creation Event 발생");
            Transform tr;
            switch (creationEvent.uiType) {
                case UIType.Screen:
                tr = await OpenScreenMoveToAnchorAsync<Transform>(creationEvent.uiName);
                break;
                case UIType.Popup:
                tr = await OpenPopupMoveToAnchorAsync<Transform>(creationEvent.uiName);
                break;
                case UIType.HUD:
                break;
                case UIType.Overlay:
                break;
            }
        }

        public void CloseEvent(UICloseEvent closeEvent) {
            _viewModel.CloseUI(closeEvent.uiName);

        }


        /// <summary>
        /// 생성 이동
        /// </summary>
        public async UniTask<T> OpenScreenMoveToAnchorAsync<T>(UIName uiName) where T : Component {
            T t = await OpenScreenAsync<T>(uiName);
            if(t == null) {
                GameDebug.Log($"{uiName.ToString()} 생성 실패");
                return t;
            }
            MoveToAnchor(uiName, t.transform);
            return t;
        }
        /// <summary>
        /// 생성 이동
        /// </summary>
        public async UniTask<T> OpenPopupMoveToAnchorAsync<T>(UIName uiName) where T : Component {
            T t = await OpenPopupAsync<T>(uiName);
            if (t == null) {
                GameDebug.Log($"{uiName.ToString()} 생성 실패");
                return t;
            }
            MoveToAnchor(uiName, t.transform);
            return t;
        }

        /// <summary>
        /// Anchor 위치로 이동
        /// </summary>
        /// <param name="uiName"></param>
        /// <param name="pos"></param>
        public void MoveToAnchor(UIName uiName, Transform pos) {
            if(_uiAnchorDictionary.TryGetValue(uiName, out var ancher)) {
                pos.position = ancher.AnchorTransform.position;
            }
        }


        /// <summary>
        /// Screen UI 열기
        /// </summary>
        public async UniTask<T> OpenScreenAsync<T>(UIName uiName) where T : Component
        {
            return await _viewModel.OpenScreenAsync<T>(uiName);
        }
        
        /// <summary>
        /// Popup UI 열기
        /// </summary>
        public async UniTask<T> OpenPopupAsync<T>(UIName uiName) where T : Component
        {
            return await _viewModel.OpenPopupAsync<T>(uiName);
        }
        
        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(UIName uiName)
        {
            _viewModel.CloseUI(uiName);
        }

    }
}