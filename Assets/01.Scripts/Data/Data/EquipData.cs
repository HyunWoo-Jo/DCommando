using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core;
namespace Game.Data {
    /// <summary>
    /// Firebase에 저장될 장비 데이터 구조
    /// </summary>
    [Serializable]
    public class EquipData {
        [Header("장착된 장비")]
        public string equippedWeapon;    // 장착된 무기
        public string equippedArmor;     // 장착된 방어구
        public string equippedAccessory; // 장착된 악세사리

        [Header("보유 장비 목록")]
        public List<string> ownedEquipments; // 보유한 장비 목록

        public EquipData() {
            equippedWeapon = EquipName.None.ToString();
            equippedArmor = EquipName.None.ToString();
            equippedAccessory = EquipName.None.ToString();
            ownedEquipments = new List<string>();
        }

        public EquipData(string weapon, string armor, string accessory, List<string> owned) {
            equippedWeapon = weapon ?? EquipName.None.ToString();
            equippedArmor = armor ?? EquipName.None.ToString();
            equippedAccessory = accessory ?? EquipName.None.ToString();
            ownedEquipments = owned ?? new List<string>();
        }

        /// <summary>
        /// 장비가 장착되어 있는지 확인
        /// </summary>
        public bool IsEquipped(EquipName equipName) {
            string equipStr = equipName.ToString();
            return equippedWeapon == equipStr ||
                   equippedArmor == equipStr ||
                   equippedAccessory == equipStr;
        }

        /// <summary>
        /// 장비를 보유하고 있는지 확인
        /// </summary>
        public bool HasEquipment(EquipName equipName) {
            return ownedEquipments.Contains(equipName.ToString());
        }

        /// <summary>
        /// 장비 추가 (중복 방지)
        /// </summary>
        public void AddEquipment(EquipName equipName) {
            string equipStr = equipName.ToString();
            if (!ownedEquipments.Contains(equipStr)) {
                ownedEquipments.Add(equipStr);
            }
        }

        /// <summary>
        /// 장비 제거
        /// </summary>
        public bool RemoveEquipment(EquipName equipName) {
            string equipStr = equipName.ToString();

            // 장착된 장비면 해제
            if (equippedWeapon == equipStr) equippedWeapon = EquipName.None.ToString();
            if (equippedArmor == equipStr) equippedArmor = EquipName.None.ToString();
            if (equippedAccessory == equipStr) equippedAccessory = EquipName.None.ToString();

            // 보유 목록에서 제거
            return ownedEquipments.Remove(equipStr);
        }

        /// <summary>
        /// 무기 장착
        /// </summary>
        public void EquipWeapon(EquipName equipName) {
            if (HasEquipment(equipName)) {
                equippedWeapon = equipName.ToString();
            }
        }

        /// <summary>
        /// 방어구 장착
        /// </summary>
        public void EquipArmor(EquipName equipName) {
            if (HasEquipment(equipName)) {
                equippedArmor = equipName.ToString();
            }
        }

        /// <summary>
        /// 악세사리 장착
        /// </summary>
        public void EquipAccessory(EquipName equipName) {
            if (HasEquipment(equipName)) {
                equippedAccessory = equipName.ToString();
            }
        }

        /// <summary>
        /// 장착된 무기 가져오기
        /// </summary>
        public EquipName GetEquippedWeapon() {
            return System.Enum.TryParse<EquipName>(equippedWeapon, out var weapon) ? weapon : EquipName.None;
        }

        /// <summary>
        /// 장착된 방어구 가져오기
        /// </summary>
        public EquipName GetEquippedArmor() {
            return System.Enum.TryParse<EquipName>(equippedArmor, out var armor) ? armor : EquipName.None;
        }

        /// <summary>
        /// 장착된 악세사리 가져오기
        /// </summary>
        public EquipName GetEquippedAccessory() {
            return System.Enum.TryParse<EquipName>(equippedAccessory, out var accessory) ? accessory : EquipName.None;
        }

        /// <summary>
        /// 보유 장비 목록을 EquipName 리스트로 반환
        /// </summary>
        public List<EquipName> GetOwnedEquipments() {
            var result = new List<EquipName>();
            foreach (var equipStr in ownedEquipments) {
                if (System.Enum.TryParse<EquipName>(equipStr, out var equipName)) {
                    result.Add(equipName);
                }
            }
            return result;
        }
    }
}