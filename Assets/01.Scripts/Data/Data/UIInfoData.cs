using System;
using Game.Core;
namespace Game.Data {
    [Serializable]
    public class UIInfoData {
        public UIName uiName;           // UI 이름 (Key)
        public string addressableKey;   // Addressable Asset Key
        public UIType uiType;          // UI 타입

        public UIInfoData() {
            uiName = UIName.None;
            addressableKey = string.Empty;
            uiType = UIType.Screen;
        }

        public UIInfoData(UIName uiName, string addressableKey, UIType uiType) {
            this.uiName = uiName;
            this.addressableKey = addressableKey;
            this.uiType = uiType;
        }

        public override string ToString() {
            return $"UIInfo[{uiName}] Type: {uiType}, Key: {addressableKey}";
        }
    }
}