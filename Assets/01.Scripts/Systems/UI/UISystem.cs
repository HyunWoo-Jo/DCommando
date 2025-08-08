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
    public class UISystem : IDisposable {
        [Inject] private readonly IUIService _uiService;

        private readonly Dictionary<UIName, GameObject> _prefabs = new(); // Prefabs UI
        private readonly Dictionary<UIName, GameObject> _instanceUIs = new(); // 일반 단일 생성 UI
        private readonly Dictionary<UIName, List<GameObject>> _instanceHudUIs = new(); // 다중 생성 HUD UI
        private readonly Dictionary<UIType, Transform> _instanceUIParents = new(); // UI 부모 

        #region 초기화 Zenject 관리
        [Inject]
        public void Initialize() {
            SetupInstanceUIParents();
        }
       
        public void Dispose() {
            // 일반 단일 생성 UI 제거
            foreach (var kvp in _instanceUIs) {
                if (kvp.Value != null) {
                    UnityEngine.Object.Destroy(kvp.Value);
                }
            }
            _instanceUIs.Clear();

            // 다중 생성 HUD UI 제거
            foreach (var kvp in _instanceHudUIs) {
                if (kvp.Value != null) {
                    foreach (var gameObject in kvp.Value) {
                        if (gameObject != null) {
                            UnityEngine.Object.Destroy(gameObject);
                        }
                    }
                    kvp.Value.Clear();
                }
            }
            _instanceHudUIs.Clear();
            _uiService.ReleaseAll();
        }

        /// <summary>
        /// InstanceUI Type별 부모 Transform 생성
        /// </summary>
        private void SetupInstanceUIParents() {
            var canvas = FindOrCreateCanvas();
            _instanceUIParents[UIType.HUD] = CreateUILayer(canvas.transform, "HUD", 10, false);
            _instanceUIParents[UIType.HUD1] = CreateUILayer(canvas.transform, "HUD1", 100, false);
            _instanceUIParents[UIType.HUD2] = CreateUILayer(canvas.transform, "HUD2", 200, false);
            _instanceUIParents[UIType.Screen] = CreateUILayer(canvas.transform, "Screen", 300);
            _instanceUIParents[UIType.DynamicScreen] = CreateUILayer(canvas.transform, "DynamicScreen", 400);
            _instanceUIParents[UIType.Popup] = CreateUILayer(canvas.transform, "Popup", 500);
            _instanceUIParents[UIType.Overlay] = CreateUILayer(canvas.transform, "Overlay", 600);
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
        private async UniTask<T> InstanceUI<T>(int id, UIType type, UIName uiName) where T : Component {
            if (_instanceUIs.ContainsKey(uiName)) {
                GameDebug.LogError($"{uiName.ToString()} 중복 생성 불가능한 UI");
                return null;
            }
            T uiComponent = await _uiService.LoadUIAsync<T>(uiName);
            if (uiComponent != null) {
                uiComponent.transform.SetParent(_instanceUIParents[type]);
                _instanceUIs.Add(uiName, uiComponent.gameObject); // dict에 추가
                EventBus.Publish(new UIOpenedNotificationEvent(id, uiName, type, uiComponent.gameObject));
            }
            return uiComponent;
        }

        private async UniTask<T> InstanceHudUI<T>(int id, UIName uiName) where T : Component {
            T uiComponent = await _uiService.LoadUIAsync<T>(uiName);
            if (uiComponent != null) {
                if (!_instanceHudUIs.ContainsKey(uiName)) _instanceHudUIs[uiName] = new List<GameObject>();
                _instanceHudUIs[uiName].Add(uiComponent.gameObject); // Object 추가
                uiComponent.transform.SetParent(_instanceUIParents[UIType.HUD]);
                EventBus.Publish(new UIOpenedNotificationEvent(id, uiName, UIType.HUD, uiComponent.gameObject));
            }
           
            return uiComponent;
        }


        /// <summary>
        /// CloseUI 제거
        /// </summary>
        /// <param name="uiName"></param>

        private void CloseUI(int id, UIName uiName) {
            if (!_instanceUIs.TryGetValue(uiName, out var uiObj)) {
                LogUINotFoundError();
                return;
            }
            GameObject.Destroy(uiObj);
            _instanceUIs.Remove(uiName);
            _uiService.ReleaseUI(uiName);
            EventBus.Publish(new UIClosedNotificationEvent(id, uiName));
        }

        /// <summary>
        /// HUD ui 제거
        /// </summary>
        private void CloseHudUI(int id, UIName uiName, GameObject hudUiObj) {
            if (!_instanceHudUIs.ContainsKey(uiName)) {
                LogUINotFoundError();
                return;
            }
            _instanceHudUIs[uiName].Remove(hudUiObj);
            GameObject.Destroy(hudUiObj);
            EventBus.Publish(new UIClosedNotificationEvent(id, uiName));
            if (_instanceHudUIs[uiName].Count <= 0) {
                _uiService.ReleaseUI(uiName);
            }
        }

        private void LogUINotFoundError() {
            GameDebug.LogError("존재 하지 않는 UI 삭제 시도");
        }
        #endregion

        #region Prefab 관리
        public async UniTask<GameObject> LoadPrefabsAsync(UIName name) {
            if(_prefabs.TryGetValue(name, out var obj)) {
                return obj;
            }
            var prefab = await _uiService.LoadUIPrefabAsync(name);
            _prefabs[name] = prefab;
            return _prefabs[name];
        }
        public void ReleasePrefab(UIName name)  {
            _prefabs.Remove(name);
            _uiService.ReleaseUI(name);
        }
        #endregion


        #region 편의 메서드
        /// <summary>
        /// UI 생성 HUD 일경우 여러개 생성 가능
        /// </summary>
        public async UniTask<T> CreateUIAsync<T>(int id, UIName uiName) where T : Component {
            UIType type = _uiService.GetUIType(uiName);
            switch (type) {
                case UIType.HUD:
                return await InstanceHudUI<T>(id, uiName);
                default:
                return await InstanceUI<T>(id, type, uiName);
            }
        }

        /// <summary>
        /// UI 제거 hudUIObj가 존재할경우 찾아서 제거
        /// </summary>
        public void CloseUI(int id, UIName uiName, GameObject hudUIObj = null) {
            if (hudUIObj != null) {
                // HUD UI 처리
                CloseHudUI(id, uiName, hudUIObj);
            } else {
                // 일반 UI 처리
                CloseUI(id, uiName);
            }
        }

        public Transform GetUIParent(UIType type) {
            return _instanceUIParents[type];
        }
        #endregion


    }
}
