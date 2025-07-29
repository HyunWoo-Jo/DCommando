using UnityEngine;
using Game.Core;
namespace Game.Data {
    [CreateAssetMenu(fileName = "SO_UIInfo", menuName = "Scriptable Objects/SO_UIInfo")]
    public class SO_UIInfo : ScriptableObject {
        [Header("UI ����")]
        public UIName uiName;           // UI �̸� (Key)
        public string addressableKey;   // Addressable Asset Key
        public UIType uiType;          // UI Ÿ��
        public int sortOrder;          // ���� ����

    }

}