using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "InventoryStyle", menuName = "Styles/InventoryStyle")]
    public class SO_InventoryStyle : ScriptableObject {
        [Header("슬롯 설정")]
        public int slotColumns = 6;
        public int slotRows = 10;
        public Vector2 slotSpacing = new Vector2(200, 200);
        public Vector2 slotOffset = new Vector2(-500, -100);

        [Header("장비 이미지 설정")]
        public float fixedHeight = 150f;
    }
}
