using UnityEngine;
using Game.Core;
namespace Game.Data {
    [CreateAssetMenu(fileName = "SO_UIInfo", menuName = "Scriptable Objects/SO_UIInfo")]
    public class SO_UIInfo : ScriptableObject {
        [Header("UI 정보")]
        public UIName uiName;           // UI 이름 (Key)
        public string addressableKey;   // Addressable Asset Key
        public UIType uiType;          // UI 타입
        public int sortOrder;          // 정렬 순서

    }
}