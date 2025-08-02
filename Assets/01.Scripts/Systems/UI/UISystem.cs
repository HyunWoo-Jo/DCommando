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
using System;
using System.Linq;
using UnityEngine.UI;

namespace Game.Systems {
    /// <summary>
    /// UI 관리하는 System
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class UISystem {
        [Inject] private readonly IUIService _uiService;
        [Inject] private readonly SO_UIConfig _uiConfig;

        private readonly Dictionary<UIName, GameObject> _instanceUIs = new(); // 일반 단일 생성 UI
        private readonly Dictionary<UIName, List<GameObject>> _instanceHudUIs = new(); // 다중 생성 HUD UI
        private readonly Dictionary<UIType, Transform> _instanceUIParents = new(); // UI 부모 
        private Dictionary<UIName, UIType> _uiTypeMappingDict;// uiName mapping uiType

        #region Zenject 관리
        [Inject]
        public void Initialize() {
            SetupInstanceUIParents();
            InitializeUIService();
        }
        #endregion

        #region 초기화
        /// <summary>
        /// UIService에 InstanceUI 매핑 설정
        /// </summary>
        private void InitializeUIService() {
            var uiConfigDict = _uiConfig.GetUIDictionary();

            // UI Name map addressablesKey
            var instanceUIDict = uiConfigDict
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.addressableKey);
            _uiService.Initialize(instanceUIDict);

            // UiTypeMapping 생성
            var uiTypeDict = uiConfigDict
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.uiType);
            _uiTypeMappingDict = uiTypeDict;
        }

        /// <summary>
        /// InstanceUI Type별 부모 Transform 생성
        /// </summary>
        private void SetupInstanceUIParents() {
            var canvas = FindOrCreateCanvas();
            _instanceUIParents[UIType.HUD] = CreateUILayer(canvas.transform, "HUD", 10, false);
            _instanceUIParents[UIType.Screen] = CreateUILayer(canvas.transform, "Screen", 200);
            _instanceUIParents[UIType.Popup] = CreateUILayer(canvas.transform, "Popup", 300);
            _instanceUIParents[UIType.Overlay] = CreateUILayer(canvas.transform, "Overlay", 400);
        }


        /// <summary>
        /// MainCanvas를 찾는 코드
        /// </summary>
        /// <returns></returns>
        private Canvas FindOrCreateCanvas() {
            var canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null) {
               
            }
            return canvas;
        }

        private Transform CreateUILayer(Transform parent, string layerName, int baseSortingOrder, bool isRaycast = true) {
            var layerGO = new GameObject($"InstanceUI_{layerName}");
            layerGO.transform.SetParent(parent, false);         
            var layerCanvas = layerGO.AddComponent<Canvas>();
            layerCanvas.overrideSorting = true;
            layerCanvas.sortingOrder = baseSortingOrder;

            if(isRaycast)
                layerGO.AddComponent<GraphicRaycaster>();

            // Resize
            RectTransform rect = layerGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            return layerGO.transform;
        }
        #endregion

        #region InstanceUI 관리
        private async UniTask<T> InstanceUI<T>(UIType type, UIName uiName) where T : Component {
            if (_instanceUIs.ContainsKey(uiName)) {
                GameDebug.LogError($"{uiName.ToString()} 중복 생성 불가능한 UI");
                return null;
            }
            T uiComponent = await _uiService.LoadUIAsync<T>(uiName);
            if (uiComponent != null) {
                uiComponent.transform.SetParent(_instanceUIParents[type]);
                _instanceUIs.Add(uiName, uiComponent.gameObject); // dict에 추가
                EventBus.Publish(new UIOpenedNotificationEvent(uiName, type, uiComponent.gameObject));
            }
            return uiComponent;
        }

        private async UniTask<T> InstanceHudUI<T>(UIName uiName) where T : Component {
            T uiComponent = await _uiService.LoadUIAsync<T>(uiName);
            if (uiComponent != null) {
                if (!_instanceHudUIs.ContainsKey(uiName)) _instanceHudUIs[uiName] = new List<GameObject>();
                _instanceHudUIs[uiName].Add(uiComponent.gameObject); // Object 추가
                uiComponent.transform.SetParent(_instanceUIParents[UIType.HUD]);
                EventBus.Publish(new UIOpenedNotificationEvent(uiName, UIType.HUD, uiComponent.gameObject));
            }
           
            return uiComponent;
        }


        /// <summary>
        /// CloseUI 제거
        /// </summary>
        /// <param name="uiName"></param>

        private void CloseUI(UIName uiName) {
            if (!_instanceUIs.TryGetValue(uiName, out var uiObj)) {
                LogUINotFoundError();
                return;
            }
            GameObject.Destroy(uiObj);
            _instanceUIs.Remove(uiName);
            _uiService.ReleaseUI(uiName);
            EventBus.Publish(new UIClosedNotificationEvent(uiName));
        }

        /// <summary>
        /// HUD ui 제거
        /// </summary>
        private void CloseHudUI(UIName uiName, GameObject hudUiObj) {
            if (!_instanceHudUIs.ContainsKey(uiName)) {
                LogUINotFoundError();
                return;
            }
            _instanceHudUIs[uiName].Remove(hudUiObj);
            GameObject.Destroy(hudUiObj);
            EventBus.Publish(new UIClosedNotificationEvent(uiName));
            if (_instanceHudUIs[uiName].Count <= 0) {
                _uiService.ReleaseUI(uiName);
            }
        }

        private void LogUINotFoundError() {
            GameDebug.LogError("존재 하지 않는 UI 삭제 시도");
        }
        #endregion

        #region 편의 메서드
        /// <summary>
        /// UI 생성 HUD 일경우 여러개 생성 가능
        /// </summary>
        public async UniTask<T> CreateUIAsync<T>(UIName uiName) where T : Component {
            UIType type = _uiTypeMappingDict[uiName];
            switch (type) {
                case UIType.HUD:
                return await InstanceHudUI<T>(uiName);
                default:
                return await InstanceUI<T>(type, uiName);
            }
        }

        /// <summary>
        /// UI 제거 hudUIObj가 존재할경우 찾아서 제거
        /// </summary>
        public void CloseUI(UIName uiName, GameObject hudUIObj = null) {
            if (hudUIObj != null) {
                // HUD UI 처리
                CloseHudUI(uiName, hudUIObj);
            } else {
                // 일반 UI 처리
                CloseUI(uiName);
            }
        }
        #endregion

       
    }
}
