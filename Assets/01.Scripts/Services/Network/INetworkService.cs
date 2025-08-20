using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Services {
    public interface INetworkService {
        bool IsInitialized { get; }
        bool IsConnected { get; }
        string CurrentUserId { get; }

        UniTask InitializeAsync();
        UniTask<bool> IsConnectedAsync();
        void SignOut();


        #region 크리스탈 관련 메서드
        UniTask<CrystalData> LoadCrystalDataAsync();
        UniTask SaveCrystalDataAsync(CrystalData crystalData);
        UniTask<bool> UpdateFreeCrystalAsync(int amount);
        UniTask<bool> UpdatePaidCrystalAsync(int amount);
        #endregion

        #region 장비 관련 메서드
        /// <summary>
        /// 장비 데이터를 Firebase에서 로드
        /// </summary>
        UniTask<EquipData> LoadEquipDataAsync();

        /// <summary>
        /// 장비 데이터를 Firebase에 저장
        /// </summary>
        UniTask SaveEquipDataAsync(EquipData equipData);

        /// <summary>
        /// 무기 장착 상태 업데이트
        /// </summary>
        UniTask<bool> UpdateEquippedWeaponAsync(EquipName equipName);

        /// <summary>
        /// 방어구 장착 상태 업데이트
        /// </summary>
        UniTask<bool> UpdateEquippedArmorAsync(EquipName equipName);

        /// <summary>
        /// 악세사리 장착 상태 업데이트
        /// </summary>
        UniTask<bool> UpdateEquippedAccessoryAsync(EquipName equipName);

        /// <summary>
        /// 장비 획득 (보유 목록에 추가)
        /// </summary>
        UniTask<bool> AddOwnedEquipmentAsync(EquipName equipName);

        /// <summary>
        /// 장비 제거 (보유 목록에서 제거 및 장착 해제)
        /// </summary>
        UniTask<bool> RemoveOwnedEquipmentAsync(EquipName equipName);
        #endregion
    }
}