using System;
using UnityEngine;

namespace Game.Models {
    [Serializable]
    public struct HealthData {
        public int maxHp;
        public int currentHp;
        public bool isDead;

        public HealthData(int maxHp) {
            this.maxHp = maxHp;
            this.currentHp = maxHp;
            this.isDead = false;
        }
    }
}