using UnityEngine;
using UnityEngine.UI;
namespace Game.Core {
    public static class UIUtills {
        public static void Resize(this GameObject obj) {
            RectTransform rect = obj.GetComponent<RectTransform>();
            // 앵커를 이용해 부모 전체 영역을 차지
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
        }
        /// <summary>
        /// Sprite 비율을 확인해 사이즈 정함
        /// </summary>
        /// <param name="targetRect"></param>
        /// <param name="sprite"></param>
        public static void SetSizeSlot(this RectTransform targetRect, Sprite sprite, float height) {
            if (sprite == null) {
                GameDebug.LogError("Sprite 가 존재하지 않습니다");
                return;
            }
            Rect rect = sprite.rect;
            float spriteWidth = rect.width;
            float spriteHeight = rect.height;

            // Style에서 고정 높이 가져와서 x를 비율에 맞춰 계산
            float aspectRatio = spriteWidth / spriteHeight;
            float calculatedWidth = height * aspectRatio;

            targetRect.sizeDelta = new Vector2(calculatedWidth, height);
        }
    }
}
