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
    public class UIManager : MonoBehaviour
    {
        [Inject] private UIViewModel _viewModel;

        private Dictionary<UIName, Vector3> _uiAnchorPosDictionary = new();

        private readonly CompositeDisposable _disposables = new();

        private void Awake()
        {
            var anchorList = GetComponentsInChildren<UIAnchor>();

            foreach (var anchor in anchorList) {
                _uiAnchorPosDictionary.Add(anchor.Ui_Name, anchor.transform.position);
                Destroy(anchor.gameObject);
            }
           
            EventBus.Subscribe<UICreationEvent>(CreationEvent).AddTo(_disposables);
            EventBus.Subscribe<UICloseEvent>(CloseEvent).AddTo(_disposables);
        }
        private void OnDestroy() {
            _disposables?.Dispose();
        }

        public async void CreationEvent(UICreationEvent creationEvent) {
            GameDebug.Log($"{creationEvent.uiName} Creation Event 발생");
            Transform tr = await OpenUIMoveToAnchorAsync<Transform>(creationEvent.id, creationEvent.uiName);
        }

        public void CloseEvent(UICloseEvent closeEvent) {
            _viewModel.CloseUI(closeEvent.uiName, closeEvent.uiObj);
        }


        /// <summary>
        /// 생성 이동
        /// </summary>
        public async UniTask<T> OpenUIMoveToAnchorAsync<T>(int id, UIName uiName) where T : Component {
            T t = await OpenUIAsync<T>(id, uiName);
            if(t == null) {
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
            if(_uiAnchorPosDictionary.TryGetValue(uiName, out var anchorPos)) {
                pos.position = anchorPos;
            }
        }


        /// <summary>
        /// UI 열기
        /// </summary>
        public async UniTask<T> OpenUIAsync<T>(int id, UIName uiName) where T : Component
        {
            return await _viewModel.OpenUIAsync<T>(id, uiName);
        }
        
        
        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(UIName uiName, GameObject uiObj)
        {
            _viewModel.CloseUI(uiName, uiObj);
        }

    }
}