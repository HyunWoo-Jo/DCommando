using Game.Models;
using Game.Systems;
using UnityEngine;
using Zenject;
using Game.Core;
using System.Collections.Generic;
using R3;
namespace Game.ViewModels
{
    public class InventoryViewModel {
        [Inject] private readonly EquipSystem _equipSystem;
        [Inject] private readonly EquipModel _equipModel;

        public ReadOnlyReactiveProperty<EquipName> RORP_EquippedWeapon => _equipModel.EquippedWeapon;
        public ReadOnlyReactiveProperty<EquipName> RORP_EquippedArmor => _equipModel.EquippedArmor;
        public ReadOnlyReactiveProperty<EquipName> RORP_EquippedAccessory => _equipModel.EquippedAccessory;

        public Observable<List<EquipName>> Ob_OwnedEquipments => _equipModel.OwnedEquipments;


        public Sprite GetSprite(EquipName equipName) {
            return _equipSystem.GetLoadSprite(equipName);
        }

        public void UnLoadSpriteAll() {
            _equipSystem.UnloadSpriteAll();
        }

        public void UnequipItem() {
            _equipSystem.UnEquipWeapon();
        }
        public void EquipItem(EquipName equipName) {
            _equipSystem.EquipWeapon(equipName);
        }
    }
}
