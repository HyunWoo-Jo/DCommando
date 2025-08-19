using UnityEngine;
using R3;
using Game.Data;
namespace Game.Models {
    public class UpgradeModel {
        private ReactiveProperty<int> RP_rerollCount = new(0);
        private ReactiveProperty<UpgradeData> RP_Data;
    }
}