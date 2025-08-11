using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
namespace Game.UI {
    public class InventoryView : MonoBehaviour {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _inventoryContent;
        [SerializeField] private Transform _weaponParent;

      

        private List<GameObject> _slots = new();
        private List<GameObject> _equips = new();
        
        private void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(_slotPrefab);
            Assert.IsNotNull(_inventoryContent);
            Assert.IsNotNull(_weaponParent);
#endif
            InstanceSlot();

        }

        // ½½·Ô»ý¼º
        private void InstanceSlot() {
            int x = 6;
            int y = 10;
            for(int i =0;i<x; i++) {
                for (int j = 0; j < y; j++) {
                    GameObject obj = GameObject.Instantiate(_slotPrefab, _inventoryContent);
                    obj.transform.localPosition = new Vector3(i * 200 - 500, j * -200 - 100, 0);
                    _slots.Add(obj);
                }
            }
        }
        

        private void AddEquip(GameObject equipObj) {
            int index = _equips.Count;

            equipObj.transform.SetParent(_slots[index].transform);
            equipObj.transform.localPosition = Vector3.zero;
            _equips.Add(equipObj);
        }

        private void RemoveEquip(GameObject equipObj) {
            _equips.Remove(equipObj);
        }



    }
}
