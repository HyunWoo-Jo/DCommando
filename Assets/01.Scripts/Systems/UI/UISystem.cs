using Game.Models;
using Game.Services;
using Game.Data;
using R3;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Game.Systems
{
    public class UISystem
    {
        private readonly UIModel _uiModel;
        private readonly IUIService _uiService;
        private readonly SO_UIConfig _uiConfig;
        
        // UI 관리
        private readonly Dictionary<string, GameObject> _activeUIs = new();
        private readonly List<string> _activePopups = new();
        
        // UI 이벤트
        public readonly Subject<string> OnScreenOpenedEvent = new();
        public readonly Subject<string> OnPopupOpenedEvent = new();
        
        // UI 부모 Transform들
        private Transform _screenParent;
        private Transform _popupParent;
        
        public UISystem(UIModel uiModel, IUIService uiService, SO_UIConfig uiConfig)
        {
            _uiModel = uiModel;
            _uiService = uiService;
            _uiConfig = uiConfig;
            
            SetupUIParents();
        }
        
        private void SetupUIParents()
        {
            var canvas = FindOrCreateCanvas();
            _screenParent = CreateUILayer(canvas.transform, "Screens", 0);
            _popupParent = CreateUILayer(canvas.transform, "Popups", 100);
        }
        
        private Canvas FindOrCreateCanvas()
        {
            var canvas = GameObject.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasGO = new GameObject("UI Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            return canvas;
        }
        
        private Transform CreateUILayer(Transform parent, string layerName, int sortOrder)
        {
            var layerGO = new GameObject(layerName);
            layerGO.transform.SetParent(parent, false);
            return layerGO.transform;
        }
        
        public async UniTask<T> OpenScreenAsync<T>(string uiName) where T : Component
        {
            var uiInfo = _uiConfig.GetUIInfo(uiName);
            if (uiInfo == null) return null;
            
            _uiModel.SetLoadingState(true);
            
            var uiComponent = await _uiService.LoadUIAsync<T>(uiInfo.addressableKey);
            if (uiComponent != null)
            {
                uiComponent.transform.SetParent(_screenParent, false);
                _activeUIs[uiName] = uiComponent.gameObject;
                _uiModel.SetCurrentScreen(uiName);
                OnScreenOpenedEvent.OnNext(uiName);
            }
            
            _uiModel.SetLoadingState(false);
            return uiComponent;
        }
        
        public async UniTask<T> OpenPopupAsync<T>(string uiName) where T : Component
        {
            var uiInfo = _uiConfig.GetUIInfo(uiName);
            if (uiInfo == null) return null;
            
            var uiComponent = await _uiService.LoadUIAsync<T>(uiInfo.addressableKey);
            if (uiComponent != null)
            {
                uiComponent.transform.SetParent(_popupParent, false);
                _activeUIs[uiName] = uiComponent.gameObject;
                _activePopups.Add(uiName);
                OnPopupOpenedEvent.OnNext(uiName);
            }
            
            return uiComponent;
        }
        
        public void CloseUI(string uiName)
        {
            if (_activeUIs.TryGetValue(uiName, out var uiObject))
            {
                _uiService.ReleaseUI(uiObject);
                _activeUIs.Remove(uiName);
                _activePopups.Remove(uiName);
            }
        }
    }
}