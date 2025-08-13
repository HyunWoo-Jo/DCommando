using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core;
using System;
using Game.Data;
namespace Game.Services {
    public class EquipService : IEquipService, IInitializable, IDisposable {
        [Inject] private IAddressableService<EquipName, GameObject> _addressableService;
        [Inject] private IAddressableService<EquipName, Sprite> _spriteAddressableService;
        [Inject] private INetworkService _networkService;
        #region 초기화
        public void Initialize() {
            try {
                var equipAddressMap = CSVReader.ReadToDictionary("AddressKey/EquipAddressKey");
                _addressableService.RegisterAddressKeys(equipAddressMap.ToEnumKey<EquipName>());

                // Sprite
                var equipSpriteAddressMap = CSVReader.ReadToDictionary("AddressKey/EquipSpriteAddressKey");
                _spriteAddressableService.RegisterAddressKeys(equipSpriteAddressMap.ToEnumKey<EquipName>());


            } catch {
                GameDebug.LogError("Equip CSV 파일 로드에 실패했습니다.");
            }
        }
        public void Dispose() {
            _addressableService.UnloadAll();
            _spriteAddressableService.UnloadAll();

        }
        #endregion
        #region 장비 프리펩 로드

        public GameObject LoadEquipInstancePrefab(EquipName equipName) {
            return _addressableService.LoadAsset(equipName);
        }

        public async UniTask<GameObject> LoadEquipInstancePrefabAsync(EquipName equipName) {
            var equipPrefab = await _addressableService.LoadAssetAsync(equipName);


            if (equipPrefab != null) {
                GameDebug.Log($"장비 프리팹 로드 완료: {equipName}");
            } else {
                GameDebug.LogError($"장비 프리팹 로드 실패: {equipName}");
            }
            // DI 주입
            GameObject obj = DIHelper.InstantiateWithInjection(equipPrefab);
            return obj;
        }

        public bool HasEquip(EquipName equipName) {
            return _addressableService.HasAddressKey(equipName);
        }

        public void UnloadEquip(EquipName equipName) {
            _addressableService.UnloadAsset(equipName);
            GameDebug.Log($"장비 프리팹 언로드: {equipName}");
        }
        public GameObject GetLoadedEquipPrefab(EquipName equipName) {
            return _addressableService.GetLoadedAsset(equipName);
        }
        #endregion

        #region Firebase 장비 데이터 관리
        /// <summary>
        /// 장비 데이터를 Firebase에서 로드
        /// </summary>
        public async UniTask<EquipData> LoadEquipDataAsync() {
            return await _networkService.LoadEquipDataAsync();
        }

        /// <summary>
        /// 장비 데이터를 Firebase에 저장
        /// </summary>
        public async UniTask SaveEquipDataAsync(EquipData equipData) {
            await _networkService.SaveEquipDataAsync(equipData);
        }

        /// <summary>
        /// 무기 장착
        /// </summary>
        public async UniTask<bool> EquipWeaponAsync(EquipName equipName) {
            return await _networkService.UpdateEquippedWeaponAsync(equipName);
        }

        /// <summary>
        /// 방어구 장착
        /// </summary>
        public async UniTask<bool> EquipArmorAsync(EquipName equipName) {
            return await _networkService.UpdateEquippedArmorAsync(equipName);
        }

        /// <summary>
        /// 악세사리 장착
        /// </summary>
        public async UniTask<bool> EquipAccessoryAsync(EquipName equipName) {
            return await _networkService.UpdateEquippedAccessoryAsync(equipName);
        }

        /// <summary>
        /// 장비 획득 (보유 목록에 추가)
        /// </summary>
        public async UniTask<bool> AcquireEquipmentAsync(EquipName equipName) {
            return await _networkService.AddOwnedEquipmentAsync(equipName);
        }

        /// <summary>
        /// 장비 제거
        /// </summary>
        public async UniTask<bool> RemoveEquipmentAsync(EquipName equipName) {
            return await _networkService.RemoveOwnedEquipmentAsync(equipName);
        }
        /// <summary>
        /// 현재 장착된 장비 정보 가져오기
        /// </summary>
        public async UniTask<(EquipName weapon, EquipName armor, EquipName accessory)> GetEquippedItemsAsync() {
            try {
                var equipData = await _networkService.LoadEquipDataAsync();
                return (equipData.GetEquippedWeapon(), equipData.GetEquippedArmor(), equipData.GetEquippedAccessory());
            } catch (Exception ex) {
                GameDebug.LogError($"장착된 장비 정보 로드 실패: {ex.Message}");
                return (EquipName.None, EquipName.None, EquipName.None);
            }
        }

        /// <summary>
        /// 보유한 장비 목록 가져오기
        /// </summary>
        public async UniTask<List<EquipName>> GetOwnedEquipmentsAsync() {
            try {
                var equipData = await _networkService.LoadEquipDataAsync();
                return equipData.GetOwnedEquipments();
            } catch (Exception ex) {
                GameDebug.LogError($"보유 장비 목록 로드 실패: {ex.Message}");
                return new List<EquipName>();
            }
        }

        #endregion
        #region Sprite관련 로직
        public Sprite LoadEquipSprite(EquipName equipName) {
            return _spriteAddressableService.LoadAsset(equipName);
        }

        public async UniTask<Sprite> LoadEquipSpriteAsync(EquipName equipName) {
            return await _spriteAddressableService.LoadAssetAsync(equipName);
        }

        public void UnloadEquipSprite(EquipName equipName) {
           _spriteAddressableService.UnloadAsset(equipName);
        }
        public void UnloadSpriteAll() {
            _spriteAddressableService.UnloadAll();
        }

        #endregion

    }
}