using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Core;

namespace Game.Services {
    public interface IEquipService {
        #region 프리팹 로드
        /// <summary>
        /// 장비 인스턴스 프리팹을 로드하고 DI 주입
        /// </summary>
        GameObject LoadEquipInstancePrefab(EquipName equipName);
        /// <summary>
        /// 장비 인스턴스 프리팹을 로드하고 DI 주입 비동기
        /// </summary>
        UniTask<GameObject> LoadEquipInstancePrefabAsync(EquipName equipName);

        /// <summary>
        /// 해당 장비가 존재하는지 확인합니다
        /// </summary>
        bool HasEquip(EquipName equipName);

        /// <summary>
        /// 특정 장비 프리팹을 언로드합니다
        /// </summary>
        void UnloadEquip(EquipName equipName);

        /// <summary>
        /// 로드된 장비 프리팹을 가져옵니다
        /// </summary>
        GameObject GetLoadedEquipPrefab(EquipName equipName);
        #endregion

        #region Firebase 데이터 관리
        /// <summary>
        /// 장비 데이터를 Firebase에서 로드
        /// </summary>
        UniTask<EquipData> LoadEquipDataAsync();

        /// <summary>
        /// 장비 데이터를 Firebase에 저장
        /// </summary>
        UniTask SaveEquipDataAsync(EquipData equipData);
        #endregion

        #region 장비 장착/해제
        /// <summary>
        /// 무기 장착
        /// </summary>
        UniTask<bool> EquipWeaponAsync(EquipName equipName);

        /// <summary>
        /// 방어구 장착
        /// </summary>
        UniTask<bool> EquipArmorAsync(EquipName equipName);

        /// <summary>
        /// 악세사리 장착
        /// </summary>
        UniTask<bool> EquipAccessoryAsync(EquipName equipName);

        /// <summary>
        /// 현재 장착된 장비 정보 가져오기
        /// </summary>
        UniTask<(EquipName weapon, EquipName armor, EquipName accessory)> GetEquippedItemsAsync();
        #endregion

        #region 장비 획득/제거
        /// <summary>
        /// 장비 획득 (보유 목록에 추가)
        /// </summary>
        UniTask<bool> AcquireEquipmentAsync(EquipName equipName);

        /// <summary>
        /// 장비 제거
        /// </summary>
        UniTask<bool> RemoveEquipmentAsync(EquipName equipName);

        /// <summary>
        /// 보유한 장비 목록 가져오기
        /// </summary>
        UniTask<List<EquipName>> GetOwnedEquipmentsAsync();
        #endregion
    }
}