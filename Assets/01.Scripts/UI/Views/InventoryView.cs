using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using Zenject;
using Game.ViewModels;
using Game.Core;
using R3;
using UnityEngine.UI;
using System.Collections.ObjectModel;

namespace Game.UI {
    public class InventoryView : MonoBehaviour {
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private Transform _inventoryContent;
        [SerializeField] private Transform _weaponParent;
        [SerializeField] private SO_InventoryStyle _style;
        [Inject] private InventoryViewModel _viewModel;

        private readonly List<GameObject> _slots = new();
        private readonly Dictionary<EquipName, GameObject> _equipDict = new();
        private EquipName _curWeapon = EquipName.None;
        private EquipName _curArmor = EquipName.None;
        private EquipName _curAccessory = EquipName.None;

        private void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(_slotPrefab);
            Assert.IsNotNull(_inventoryContent);
            Assert.IsNotNull(_weaponParent);
#endif
            InstanceSlot();
            Bind();
        }

        private void OnDestroy() {
            _viewModel.UnLoadSpriteAll();
        }

        private void Bind() {
            // ���� ���� ������Ʈ ����
            Image weaponImage = GameObject.Instantiate(_slotPrefab, _weaponParent).GetComponent<Image>();
            weaponImage.transform.localPosition = Vector3.zero;

            // ������ ���� ������Ʈ
            _viewModel.RORP_EquippedWeapon
                .ThrottleLastFrame(1)
                .Subscribe(equipName => {
                    _curWeapon = equipName;
                    if (equipName != EquipName.None) {
                        weaponImage.sprite = _viewModel.GetSprite(equipName);
                        SetSizeSlot(weaponImage.rectTransform, weaponImage.sprite);
                        weaponImage.gameObject.SetActive(true);
                    } else {
                        weaponImage.sprite = null;
                        weaponImage.gameObject.SetActive(false);
                    }
                }).AddTo(this);

            // ���� ��� ��� ������Ʈ
            _viewModel.Ob_OwnedEquipments
                .ThrottleLastFrame(1)
                .Subscribe(equipNameList => {
                    GameDebug.Log("��� ����");
                    // ���ο� ��� ������� ������Ʈ
                    foreach (var equipName in equipNameList) {

                        if (equipName != EquipName.None && !_equipDict.ContainsKey(equipName)) {
                            CreateEquipObject(equipName);
                        }
                    }
                }).AddTo(this);
        }

        // ���� ����
        private void InstanceSlot() {
            for (int i = 0; i < _style.slotRows; i++) {
                for (int j = 0; j < _style.slotColumns; j++) {
                    GameObject obj = GameObject.Instantiate(_slotPrefab, _inventoryContent);
                    obj.transform.localPosition = new Vector3(
                        j * _style.slotSpacing.x + _style.slotOffset.x,
                        i * -_style.slotSpacing.y + _style.slotOffset.y,
                        0
                    );
                    _slots.Add(obj);
                }
            }
        }

        // ���� ��� ������Ʈ�� ����
        private void ClearEquipObjects() {
            foreach (var kvp in _equipDict) {
                if (kvp.Value != null) {
                    DestroyImmediate(kvp.Value);
                }
            }
            _equipDict.Clear();
        }

        // ��� ������Ʈ ����
        private void CreateEquipObject(EquipName equipName) {
            // �� ���� ã��
            int availableSlotIndex = GetAvailableSlotIndex();
            if (availableSlotIndex == -1) {
                GameDebug.LogWarning("�κ��丮 ������ ������");
                return;
            }

            // ��� ������Ʈ ����
            GameObject equipObj = GameObject.Instantiate(_slotPrefab, _slots[availableSlotIndex].transform);
            equipObj.transform.localPosition = Vector3.zero;

            // ��������Ʈ ����
            Image equipImage = equipObj.GetComponent<Image>();
            if (equipImage != null) {
                equipImage.sprite = _viewModel.GetSprite(equipName);
                SetSizeSlot(equipImage.rectTransform, equipImage.sprite);
                equipImage.gameObject.SetActive(true);
            }

            // Ŭ�� �̺�Ʈ �߰� (����/���� ���)
            Button equipButton = equipObj.GetComponent<Button>();
            if (equipButton == null) {
                equipButton = equipObj.AddComponent<Button>();
            }

            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(() => OnEquipClicked(equipName));

            // �Ҵ�
            _equipDict[equipName] = equipObj;
        }

        /// <summary>
        /// Sprite ������ Ȯ���� ������ ����
        /// </summary>
        /// <param name="targetRect"></param>
        /// <param name="sprite"></param>
        private void SetSizeSlot(RectTransform targetRect, Sprite sprite) {
            Rect rect = sprite.rect;
            float spriteWidth = rect.width;
            float spriteHeight = rect.height;

            // Style���� ���� ���� �����ͼ� x�� ������ ���� ���
            float aspectRatio = spriteWidth / spriteHeight;
            float calculatedWidth = _style.fixedHeight * aspectRatio;

            targetRect.sizeDelta = new Vector2(calculatedWidth, _style.fixedHeight);
        }

        // ��� ������ ���� �ε��� ã��
        private int GetAvailableSlotIndex() {
            for (int i = 0; i < _slots.Count; i++) {
                // ���Կ� �ڽ��� ���� ���
                if (_slots[i].transform.childCount == 0) {
                    return i;
                }
            }
            return -1; // ��� ������ ������ ����
        }

        // ��� Ŭ�� �̺�Ʈ ó��
        private void OnEquipClicked(EquipName equipName) {
            if (_viewModel.RORP_EquippedWeapon.CurrentValue != equipName) {
                // �������� ���� ��� ����
                GameObject.DestroyImmediate(_equipDict[equipName]);
                _equipDict.Remove(equipName);
                _viewModel.EquipItem(equipName);
                GameDebug.Log($"��� ����: {equipName}");
            }
        }
    }
}