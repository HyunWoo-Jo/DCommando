using UnityEngine;
using System;
using System.Collections.Generic;

namespace Game.Core.Styles {
    /// <summary>
    /// 크리스탈 스타일 설정
    /// </summary>
    [CreateAssetMenu(fileName = "CrystalStyle", menuName = "Styles/CrystalStyle")]
    public class SO_CrystalStyle : ScriptableObject {
        [Header("값에 따른 색상 설정")]
        [SerializeField] private List<ThresholdColor> _totalThresholdColorList;
        [SerializeField] private List<ThresholdColor> _paidThresholdColorList;
        [SerializeField] private List<ThresholdColor> _freeThresholdColorList;

        /// <summary>
        /// 크리스탈 값에 따라 색상 반환
        /// </summary>
        public Color GetColorForTotalAmount(int amount) {
            foreach(var thresholdColor in _totalThresholdColorList) {
                if(thresholdColor.threshold> amount) return thresholdColor.color;
            }
            return Color.white;
        }

        public Color GetColorForFreeAmount(int amount) {
            foreach (var thresholdColor in _freeThresholdColorList) {
                if (thresholdColor.threshold > amount) return thresholdColor.color;
            }
            return Color.white;
        }
        public Color GetColorForPaidAmount(int amount) {
            foreach (var thresholdColor in _paidThresholdColorList) {
                if (thresholdColor.threshold > amount) return thresholdColor.color;
            }
            return Color.white;
        }


        /// <summary>
        /// 크리스탈 값에 따라 string 형식으로 반환
        /// </summary>
        public string FormatCrystalAmount(int amount) {
            return amount.ToString();   
        }
    }
}