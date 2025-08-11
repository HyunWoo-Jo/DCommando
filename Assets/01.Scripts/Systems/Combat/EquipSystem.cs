using Cysharp.Threading.Tasks;
using Game.Models;
using Game.Services;
using UnityEngine;
using Zenject;
using Game.Data;
using Game.Core;
using System;
namespace Game.Systems
{
    public class EquipSystem
    {
        [Inject] private readonly EquipModel _equipModel;
        [Inject] private readonly IEquipService _equipService;

        private bool _isInit = false;
        public async UniTask InitializeAsync() {
            try {
                var data = await _equipService.LoadEquipDataAsync();
                _equipModel.UpdateFromEquipData(data); // 데이터 갱신;
                _isInit = true;
            } catch {
                throw;
            }
        }


        public void EquipWeapon(EquipName equipName) {
            GameDebug.Log(equipName.ToString() + " 장비 장착");
            _equipModel.EquipWeapon(equipName);
            _equipService.SaveEquipDataAsync(_equipModel.ToEquipData());
        }
        public void UnEquipWeapon() {
            _equipModel.UnequipWeapon();
            _equipService.SaveEquipDataAsync(_equipModel.ToEquipData());
        }



        public async UniTask SaveData() {
            try {
                await _equipService.SaveEquipDataAsync(_equipModel.ToEquipData());
            } catch {
                throw;
            }
        }

        public async UniTask<IWeapon> InstanceWeapon() {
            await UniTask.WaitUntil(() => _isInit); // 초기화 완료 까지 대기
            var weapon = (await _equipService.LoadEquipInstancePrefabAsync(_equipModel.EquippedWeapon.CurrentValue)).GetComponent<IWeapon>();
            if (weapon == null) {
                GameDebug.LogError($"{_equipModel.EquippedWeapon.CurrentValue.ToString()}WeaponComponent가 존재하지 않음");
            }
            weapon.SetEquipName(_equipModel.EquippedWeapon.CurrentValue);
            return weapon;
        }

        public void UnLoadWeapon(IWeapon weapon) {
            _equipService.UnloadEquip(weapon.EquipName);
        }

        public Sprite GetLoadSprite(EquipName equipName) {
           return _equipService.LoadEquipSprite(equipName);
        }

        public void UnloadSprite(EquipName equipName) {
            _equipService.UnloadEquipSprite(equipName);
        }

        public void UnloadSpriteAll() {
            _equipService.UnloadSpriteAll();
        }
    }
}
