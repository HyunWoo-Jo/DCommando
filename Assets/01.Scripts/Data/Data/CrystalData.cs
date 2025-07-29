using UnityEngine;
using System;
namespace Game.Data
{
    /// <summary>
    /// Firebase�� ����� ũ����Ż ������ ����
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
