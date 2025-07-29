using UnityEngine;
using System;
namespace Game.Data
{
    /// <summary>
    /// Firebase에 저장될 크리스탈 데이터 구조
    /// </summary>
    [Serializable]
    public class CrystalData {
        public int freeCrystal;
        public int paidCrystal;

        public CrystalData() {
            freeCrystal = 0;
            paidCrystal = 0;
        }

        public CrystalData(int free, int paid) {
            freeCrystal = free;
            paidCrystal = paid;
        }
    }
}
