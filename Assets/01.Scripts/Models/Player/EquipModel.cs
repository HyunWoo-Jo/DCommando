using System.Collections.Generic;
using System.Linq;
using R3;
using Zenject;
using Game.Core;
using Game.Data;

namespace Game.Models {
    public class EquipModel {
        #region 장착된 장비 상태 (씬 간 유지)
        private readonly ReactiveProperty<EquipName> RP_equippedWeapon = new();
        private readonly ReactiveProperty<EquipName> RP_equippedArmor = new();
        private readonly ReactiveProperty<EquipName> RP_equippedAccessory = new();

        public ReadOnlyReactiveProperty<EquipName> EquippedWeapon => RP_equippedWeapon;
        public ReadOnlyReactiveProperty<EquipName> EquippedArmor => RP_equippedArmor;
        public ReadOnlyReactiveProperty<EquipName> EquippedAccessory => RP_equippedAccessory;
        #endregion

        #region 보유 장비 목록
        private readonly ReactiveProperty<List<EquipName>> RP_ownedEquipments = new(new List<EquipName>());
        public ReadOnlyReactiveProperty<List<EquipName>> OwnedEquipments => RP_ownedEquipments;
        #endregion

        #region 초기화
        [Inject]
        public void Initialize() {
            // 초기값 설정
            RP_equippedWeapon.Value = EquipName.None;
            RP_equippedArmor.Value = EquipName.None;
            RP_equippedAccessory.Value = EquipName.None;
        }
        #endregion

        #region 장착 관련 메서드
        /// <summary>
        /// 무기 장착
        /// </summary>
        public void EquipWeapon(EquipName equipName) {
            if (equipName == EquipName.None || RP_ownedEquipments.Value.Contains(equipName)) {
                RP_equippedWeapon.Value = equipName;
            }
        }

        /// <summary>
        /// 방어구 장착
        /// </summary>
        public void EquipArmor(EquipName equipName) {
            if (equipName == EquipName.None || RP_ownedEquipments.Value.Contains(equipName)) {
                RP_equippedArmor.Value = equipName;
            }
        }

        /// <summary>
        /// 악세사리 장착
        /// </summary>
        public void EquipAccessory(EquipName equipName) {
            if (equipName == EquipName.None || RP_ownedEquipments.Value.Contains(equipName)) {
                RP_equippedAccessory.Value = equipName;
            }
        }

        /// <summary>
        /// 무기 해제
        /// </summary>
        public void UnequipWeapon() => RP_equippedWeapon.Value = EquipName.None;

        /// <summary>
        /// 방어구 해제
        /// </summary>
        public void UnequipArmor() => RP_equippedArmor.Value = EquipName.None;

        /// <summary>
        /// 악세사리 해제
        /// </summary>
        public void UnequipAccessory() => RP_equippedAccessory.Value = EquipName.None;
        #endregion

        #region 보유 장비 관리
        /// <summary>
        /// 장비 추가 (중복 방지)
        /// </summary>
        public void AddEquipment(EquipName equipName) {
            if (equipName != EquipName.None && !RP_ownedEquipments.Value.Contains(equipName)) {
                var newList = new List<EquipName>(RP_ownedEquipments.Value) {
                    equipName
                };
                RP_ownedEquipments.Value = newList;
            }
        }

        /// <summary>
        /// 장비 제거 (장착된 경우 자동 해제)
        /// </summary>
        public bool RemoveEquipment(EquipName equipName) {
            if (!RP_ownedEquipments.Value.Contains(equipName))
                return false;

            // 장착된 장비면 해제
            if (RP_equippedWeapon.Value == equipName) UnequipWeapon();
            if (RP_equippedArmor.Value == equipName) UnequipArmor();
            if (RP_equippedAccessory.Value == equipName) UnequipAccessory();

            // 보유 목록에서 제거
            var newList = new List<EquipName>(RP_ownedEquipments.Value);
            bool removed = newList.Remove(equipName);
            if (removed) {
                RP_ownedEquipments.Value = newList;
            }
            return removed;
        }

        /// <summary>
        /// 장비 보유 여부 확인
        /// </summary>
        public bool HasEquipment(EquipName equipName) {
            return RP_ownedEquipments.Value.Contains(equipName);
        }

        /// <summary>
        /// 읽기 전용 반복자
        /// </summary>
        public IEnumerable<EquipName> GetOwnedEquipments() {
            return RP_ownedEquipments.Value;
        }
        #endregion

        #region 상태 조회
        /// <summary>
        /// 장비가 현재 장착되어 있는지 확인
        /// </summary>
        public bool IsEquipped(EquipName equipName) {
            return RP_equippedWeapon.Value == equipName ||
                   RP_equippedArmor.Value == equipName ||
                   RP_equippedAccessory.Value == equipName;
        }

        /// <summary>
        /// 현재 장착된 모든 장비 반환
        /// </summary>
        public (EquipName weapon, EquipName armor, EquipName accessory) GetEquippedItems() {
            return (RP_equippedWeapon.Value, RP_equippedArmor.Value, RP_equippedAccessory.Value);
        }

        /// <summary>
        /// 장착 가능한지 확인 (보유하고 있는지 체크)
        /// </summary>
        public bool CanEquip(EquipName equipName) {
            return equipName == EquipName.None || HasEquipment(equipName);
        }
        #endregion

        #region 데이터 동기화
        /// <summary>
        /// EquipData로부터 Model 상태 업데이트
        /// </summary>
        public void UpdateFromEquipData(EquipData equipData) {
            if (equipData == null) return;

            // 장착된 장비 업데이트
            RP_equippedWeapon.Value = equipData.GetEquippedWeapon();
            RP_equippedArmor.Value = equipData.GetEquippedArmor();
            RP_equippedAccessory.Value = equipData.GetEquippedAccessory();

            // 보유 장비 목록 업데이트
            var ownedList = equipData.GetOwnedEquipments();
            RP_ownedEquipments.Value = new List<EquipName>(ownedList);
        }

        /// <summary>
        /// 현재 Model 상태를 EquipData로 변환
        /// </summary>
        public EquipData ToEquipData() {
            var equipData = new EquipData();

            // 장착된 장비 설정
            if (RP_equippedWeapon.Value != EquipName.None)
                equipData.EquipWeapon(RP_equippedWeapon.Value);

            if (RP_equippedArmor.Value != EquipName.None)
                equipData.EquipArmor(RP_equippedArmor.Value);

            if (RP_equippedAccessory.Value != EquipName.None)
                equipData.EquipAccessory(RP_equippedAccessory.Value);

            // 보유 장비 목록 설정
            foreach (var equipment in RP_ownedEquipments.Value) {
                equipData.AddEquipment(equipment);
            }

            return equipData;
        }
        #endregion

        #region R3 Observable 스트림
        /// <summary>
        /// 장착 상태 변경 스트림 (모든 장비)
        /// </summary>
        public Observable<(EquipName weapon, EquipName armor, EquipName accessory)> OnEquipmentChanged =>
            Observable.CombineLatest(
                RP_equippedWeapon,
                RP_equippedArmor,
                RP_equippedAccessory,
                (weapon, armor, accessory) => (weapon, armor, accessory)
            );

        /// <summary>
        /// 보유 장비 변경 스트림
        /// </summary>
        public Observable<List<EquipName>> OnOwnedEquipmentsChanged => RP_ownedEquipments;
        #endregion
    }
}