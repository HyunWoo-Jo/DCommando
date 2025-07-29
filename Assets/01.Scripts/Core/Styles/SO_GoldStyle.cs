using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Styles {
    /// <summary>
    /// 골드 스타일 설정
    /// </summary>
    [CreateAssetMenu(fileName = "GoldStyle", menuName = "Styles/GoldStyle")]
    public class SO_GoldStyle : ScriptableObject {
        [Header("값에 따른 색상 설정")]
        [SerializeField] private List<ThresholdColor> _thresholdColorList;

        /// <summary>
        /// 골드 값에 따라 색상 반환
        /// </summary>
        public Color GetColorForAmount(int amount) {
            foreach (var pair in _thresholdColorList) {
                if (pair.threshold > amount) return pair.color;
            }
            return Color.black;
        }

        /// <summary>
        /// 골드 값에 따라 string 형식으로 반환
        /// </summary>
        public string FormatGoldAmount(int amount) {
            return amount.ToString();
        }
    }
}