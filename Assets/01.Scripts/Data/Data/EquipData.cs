using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Core;
namespace Game.Data {
    /// <summary>
    /// Firebase�� ����� ��� ������ ����
    /// </summary>
    [Serializable]
    public class EquipData {
        [Header("������ ���")]
        public string equippedWeapon;    // ������ ����
        public string equippedArmor;     // ������ ��
        public string equippedAccessory; // ������ �Ǽ��縮

        [Header("���� ��� ���")]
        public List<string> ownedEquipments; // ������ ��� ���

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
        /// ��� �����Ǿ� �ִ��� Ȯ��
        /// </summary>
        public bool IsEquipped(EquipName equipName) {
            string equipStr = equipName.ToString();
            return equippedWeapon == equipStr ||
                   equippedArmor == equipStr ||
                   equippedAccessory == equipStr;
        }

        /// <summary>
        /// ��� �����ϰ� �ִ��� Ȯ��
        /// </summary>
        public bool HasEquipment(EquipName equipName) {
            return ownedEquipments.Contains(equipName.ToString());
        }

        /// <summary>
        /// ��� �߰� (�ߺ� ����)
        /// </summary>
        public void AddEquipment(EquipName equipName) {
            string equipStr = equipName.ToString();
            if (!ownedEquipments.Contains(equipStr)) {
                ownedEquipments.Add(equipStr);
            }
        }

        /// <summary>
        /// ��� ����
        /// </summary>
        public bool RemoveEquipment(EquipName equipName) {
            string equipStr = equipName.ToString();

            // ������ ���� ����
            if (equippedWeapon == equipStr) equippedWeapon = EquipName.None.ToString();
            if (equippedArmor == equipStr) equippedArmor = EquipName.None.ToString();
            if (equippedAccessory == equipStr) equippedAccessory = EquipName.None.ToString();

            // ���� ��Ͽ��� ����
            return ownedEquipments.Remove(equipStr);
        }

        /// <summary>
        /// ���� ����
        /// </summary>
        public void EquipWeapon(EquipName equipName) {
            if (HasEquipment(equipName)) {
                equippedWeapon = equipName.ToString();
            }
        }

        /// <summary>
        /// �� ����
        /// </summary>
        public void EquipArmor(EquipName equipName) {
            if (HasEquipment(equipName)) {
                equippedArmor = equipName.ToString();
            }
        }

        /// <summary>
        /// �Ǽ��縮 ����
        /// </summary>
        public void EquipAccessory(EquipName equipName) {
            if (HasEquipment(equipName)) {
                equippedAccessory = equipName.ToString();
            }
        }

        /// <summary>
        /// ������ ���� ��������
        /// </summary>
        public EquipName GetEquippedWeapon() {
            return System.Enum.TryParse<EquipName>(equippedWeapon, out var weapon) ? weapon : EquipName.None;
        }

        /// <summary>
        /// ������ �� ��������
        /// </summary>
        public EquipName GetEquippedArmor() {
            return System.Enum.TryParse<EquipName>(equippedArmor, out var armor) ? armor : EquipName.None;
        }

        /// <summary>
        /// ������ �Ǽ��縮 ��������
        /// </summary>
        public EquipName GetEquippedAccessory() {
            return System.Enum.TryParse<EquipName>(equippedAccessory, out var accessory) ? accessory : EquipName.None;
        }

        /// <summary>
        /// ���� ��� ����� EquipName ����Ʈ�� ��ȯ
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