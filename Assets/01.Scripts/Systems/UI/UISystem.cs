using Game.Models;
using Game.Services;
using Game.Data;
using R3;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Zenject;
using Game.Core;
using Game.Core.Event;
namespace Game.Systems
{
    public class UISystem {
        private IEventBus _eventBus;

        [Inject] private readonly UIModel _uiModel;
        [Inject] private readonly IUIService _uiService;
        [Inject] private readonly SO_UIConfig _uiConfig;

        // UI 관리
        private readonly Dictionary<UI_Name, GameObject> _activeUIs = new();
        private readonly List<UI_Name> _activePopups = new();

        // UI 이벤트
        public readonly Subject<UI_Name> OnScreenOpenedEvent = new();
        public readonly Subject<UI_Name> OnPopupOpenedEvent = new();

        // UI 부모 Transform들
        private Transform _screenParent;
        private Transform _popupParent;


        [Inject]
        public UISystem(IEventBus eventBus) {
            _eventBus = eventBus;
            SetupUIParents();
            // 추후 EventBus에 연결
        }

        /// <summary>
        /// 기본 UI Parents 생성
        /// </summary>
        private void SetupUIParents() {
            var canvas = FindOrCreateCanvas();
            _screenParent = CreateUILayer(canvas.transform, "Screens", 0);
            _popupParent = CreateUILayer(canvas.transform, "Popups", 100);
        }

        /// <summary>
        /// Object를 검색해 Main Canvas 등록
        /// </summary>
        /// <returns></returns>
        private Canvas FindOrCreateCanvas() {
            var canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null) {
                var canvasGO = new GameObject("Main_Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            return canvas;
        }

        /// <summary>
        /// Layer 생성
        /// </summary>
        /// <returns></returns>
        private Transform CreateUILayer(Transform parent, string layerName, int sortOrder) {
            var layerGO = new GameObject(layerName);
            var subCanvas = layerGO.AddComponent<Canvas>();
            subCanvas.overrideSorting = true;
            subCanvas.sortingOrder = sortOrder;
            layerGO.transform.SetParent(parent, false);
            return layerGO.transform;
        }


        /// <summary>
        /// UI Screen에 생성
        /// </summary>

        public async UniTask<T> OpenScreenAsync<T>(UI_Name uiName, int sortingOrder = -1) where T : Component {
            var uiInfo = _uiConfig.GetUIInfo(uiName.ToString());
            if (uiInfo == null) return null;

            _uiModel.SetLoadingState(true);

            var uiComponent = await _uiService.LoadUIAsync<T>(uiInfo.addressableKey);
            if (uiComponent != null) {
                uiComponent.transform.SetParent(_screenParent, false);
                _activeUIs[uiName] = uiComponent.gameObject;
                _uiModel.SetCurrentScreen(uiName);
                SetSortOrder(uiComponent.gameObject, sortingOrder);
                OnScreenOpenedEvent.OnNext(uiName);
            }

            _uiModel.SetLoadingState(false);
            return uiComponent;
        }

        /// <summary>
        /// UI Popup에 생성
        /// </summary>

        public async UniTask<T> OpenPopupAsync<T>(UI_Name uiName, int sortingOrder = -1) where T : Component {
            var uiInfo = _uiConfig.GetUIInfo(uiName.ToString());
            if (uiInfo == null) return null;

            var uiComponent = await _uiService.LoadUIAsync<T>(uiInfo.addressableKey);
            if (uiComponent != null) {
                uiComponent.transform.SetParent(_popupParent, false);
                _activeUIs[uiName] = uiComponent.gameObject;
                _activePopups.Add(uiName);
                SetSortOrder(uiComponent.gameObject, sortingOrder);
                OnPopupOpenedEvent.OnNext(uiName);
            }

            return uiComponent;
        }

        /// <summary>
        /// UI에 Order 추가
        /// </summary>
        /// <param name="sortingOrder"> -1 예외</param>
        public void SetSortOrder(GameObject obj, int sortingOrder) {
            if (sortingOrder == -1) return;
            Canvas subCanvas = obj.GetComponent<Canvas>() ?? obj.AddComponent<Canvas>();
            subCanvas.overrideSorting = true;
            subCanvas.sortingOrder = sortingOrder;
        }

        public void CloseUI(UI_Name uiName) {
            if (_activeUIs.TryGetValue(uiName, out var uiObject)) {
                _uiService.ReleaseUI(uiObject);
                _activeUIs.Remove(uiName);
                _activePopups.Remove(uiName);
            }
        }
    }
}