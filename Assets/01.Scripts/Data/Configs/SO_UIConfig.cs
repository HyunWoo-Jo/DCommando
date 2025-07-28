using Game.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    /// <summary>
    /// UI 관련 설정값과 Addressable 키
    /// </summary>
    [CreateAssetMenu(fileName = "UIConfig", menuName = "Config/UI")]
    public class SO_UIConfig : ScriptableObject
    {
        [Header("UI Addressable Keys")]
        [SerializeField] private List<UIInfo> _uiInfos = new();
        
        public Dictionary<string, UIInfo> GetUIDictionary()
        {
            var dict = new Dictionary<string, UIInfo>();
            foreach (var info in _uiInfos)
            {
                dict[info.uiName] = info;
            }
            return dict;
        }
        
        public UIInfo GetUIInfo(string uiName)
        {
            return _uiInfos.Find(info => info.uiName == uiName);
        }
    }
    
    [System.Serializable]
    public class UIInfo
    {
        [Header("UI 정보")]
        public string uiName;           // UI 이름 (Key)
        public string addressableKey;   // Addressable Asset Key
        public UIType uiType;          // UI 타입
        public bool isModal;           // 모달 여부
        public bool isPersistent;      // 영구 유지 여부
        public int sortOrder;          // 정렬 순서
    }
    
    public enum UIType
    {
        Screen,     // 전체 화면 UI
        Popup,      // 팝업 UI
        HUD,        // HUD UI
        Overlay     // 오버레이 UI
    }
}