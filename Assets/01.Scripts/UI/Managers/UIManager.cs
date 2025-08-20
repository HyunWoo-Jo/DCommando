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
    /// UI 매니저 - ViewModel을 통한 UI 생성 및 위치 관리
    /// 이 컴포넌트는 Main Cavnas에 달려야함
    /// System에서 처리안하고 View에서 처리하는 이유
    /// UIManager는 기본적인 UI의 위치를 가지고 있어 위치 처리에 유리 
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Inject] private UIViewModel _viewModel;

        private Dictionary<UIName, Vector3> _uiAnchorPosDictionary = new();

        private readonly CompositeDisposable _disposables = new();
        #region 초기화
        private void Awake()
        {
            var anchorList = GetComponentsInChildren<UIAnchor>();

            foreach (var anchor in anchorList) {
                _uiAnchorPosDictionary.Add(anchor.Ui_Name, anchor.transform.position);
                Destroy(anchor.gameObject);
            }
           
            EventBus.Subscribe<UICreationEventAsync>(CreationEventAsync).AddTo(_disposables);
            EventBus.Subscribe<UICreationEvent>(CreationEvent).AddTo(_disposables);
            EventBus.Subscribe<UICloseEvent>(CloseEvent).AddTo(_disposables);
        }
        private void OnDestroy() {
            _disposables?.Dispose();
        }
        #endregion
        #region Event
        private async void CreationEventAsync(UICreationEventAsync creationEvent) {
            GameDebug.Log($"{creationEvent.id} - {creationEvent.uiName} Creation Event 발생");
            
            Transform tr = await OpenUIMoveToAnchorAsync<Transform>(creationEvent.id, creationEvent.uiName);
        }

        private void CreationEvent(UICreationEvent creationEvent) {
            OpenUIMoveToAnchor(creationEvent.id, creationEvent.uiName);
        }

        public void CloseEvent(UICloseEvent closeEvent) {
            _viewModel.CloseUI(closeEvent.id, closeEvent.uiName, closeEvent.uiObj);
        }
        #endregion
        #region 유틸
        public GameObject OpenUIMoveToAnchor(int id, UIName uiName) {
            var obj = OpenUI(id, uiName);
            if (obj == null) {
                GameDebug.Log($"{uiName.ToString()} 생성 실패");
                return null;
            }
            MoveToAnchor(uiName, obj.transform);
            return obj;
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
        #endregion
        #region ViewModel 호출 
        /// <summary>
        /// UI 열기
        /// </summary>
        public async UniTask<T> OpenUIAsync<T>(int id, UIName uiName) where T : Component
        {
            return await _viewModel.OpenUIAsync<T>(id, uiName);
        }

        public GameObject OpenUI(int id, UIName uiName) {
            return _viewModel.OpenUI(id, uiName);
        }
        
        
        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(int id, UIName uiName, GameObject uiObj)
        {
            _viewModel.CloseUI(id, uiName, uiObj);
        }
        #endregion
    }
}