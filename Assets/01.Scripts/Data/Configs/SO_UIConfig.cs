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
        [SerializeField] private List<SO_UIInfo> _uiInfos = new();
        
        public Dictionary<UIName, SO_UIInfo> GetUIDictionary()
        {
            var dict = new Dictionary<UIName, SO_UIInfo>();
            foreach (var info in _uiInfos)
            {
                dict[info.uiName] = info;
            }
            return dict;
        }
        
        public SO_UIInfo GetUIInfo(UIName uiName)
        {
            return _uiInfos.Find(info => info.uiName == uiName);
        }
    }
   
}