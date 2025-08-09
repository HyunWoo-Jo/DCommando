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
        [Inject] private readonly EquipService _equipService;

        private bool _isInit = false;
        public async UniTask InitializeAsync() {
            try {
                var data = await _equipService.LoadEquipDataAsync();
                _equipModel.UpdateFromEquipData(data); // 데이터 갱신
                _isInit = true;
            } catch {
                throw;
            }
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
            return weapon;
        }
    }
}
