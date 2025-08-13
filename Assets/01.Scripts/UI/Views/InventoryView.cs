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
            // 무기 슬롯 오브젝트 생성
            Image weaponImage = GameObject.Instantiate(_slotPrefab, _weaponParent).GetComponent<Image>();
            weaponImage.transform.localPosition = Vector3.zero;

            // 장착된 무기 업데이트
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

            // 보유 장비 목록 업데이트
            _viewModel.Ob_OwnedEquipments
                .ThrottleLastFrame(1)
                .Subscribe(equipNameList => {
                    GameDebug.Log("장비 변경");
                    // 새로운 장비 목록으로 업데이트
                    foreach (var equipName in equipNameList) {

                        if (equipName != EquipName.None && !_equipDict.ContainsKey(equipName)) {
                            CreateEquipObject(equipName);
                        }
                    }
                }).AddTo(this);
        }

        // 슬롯 생성
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

        // 기존 장비 오브젝트들 제거
        private void ClearEquipObjects() {
            foreach (var kvp in _equipDict) {
                if (kvp.Value != null) {
                    DestroyImmediate(kvp.Value);
                }
            }
            _equipDict.Clear();
        }

        // 장비 오브젝트 생성
        private void CreateEquipObject(EquipName equipName) {
            // 빈 슬롯 찾기
            int availableSlotIndex = GetAvailableSlotIndex();
            if (availableSlotIndex == -1) {
                GameDebug.LogWarning("인벤토리 슬롯이 가득참");
                return;
            }

            // 장비 오브젝트 생성
            GameObject equipObj = GameObject.Instantiate(_slotPrefab, _slots[availableSlotIndex].transform);
            equipObj.transform.localPosition = Vector3.zero;

            // 스프라이트 설정
            Image equipImage = equipObj.GetComponent<Image>();
            if (equipImage != null) {
                equipImage.sprite = _viewModel.GetSprite(equipName);
                SetSizeSlot(equipImage.rectTransform, equipImage.sprite);
                equipImage.gameObject.SetActive(true);
            }

            // 클릭 이벤트 추가 (장착/해제 기능)
            Button equipButton = equipObj.GetComponent<Button>();
            if (equipButton == null) {
                equipButton = equipObj.AddComponent<Button>();
            }

            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(() => OnEquipClicked(equipName));

            // 할당
            _equipDict[equipName] = equipObj;
        }

        /// <summary>
        /// Sprite 비율을 확인해 사이즈 정함
        /// </summary>
        /// <param name="targetRect"></param>
        /// <param name="sprite"></param>
        private void SetSizeSlot(RectTransform targetRect, Sprite sprite) {
            Rect rect = sprite.rect;
            float spriteWidth = rect.width;
            float spriteHeight = rect.height;

            // Style에서 고정 높이 가져와서 x를 비율에 맞춰 계산
            float aspectRatio = spriteWidth / spriteHeight;
            float calculatedWidth = _style.fixedHeight * aspectRatio;

            targetRect.sizeDelta = new Vector2(calculatedWidth, _style.fixedHeight);
        }

        // 사용 가능한 슬롯 인덱스 찾기
        private int GetAvailableSlotIndex() {
            for (int i = 0; i < _slots.Count; i++) {
                // 슬롯에 자식이 없는 경우
                if (_slots[i].transform.childCount == 0) {
                    return i;
                }
            }
            return -1; // 사용 가능한 슬롯이 없음
        }

        // 장비 클릭 이벤트 처리
        private void OnEquipClicked(EquipName equipName) {
            if (_viewModel.RORP_EquippedWeapon.CurrentValue != equipName) {
                // 장착되지 않은 경우 장착
                GameObject.DestroyImmediate(_equipDict[equipName]);
                _equipDict.Remove(equipName);
                _viewModel.EquipItem(equipName);
                GameDebug.Log($"장비 장착: {equipName}");
            }
        }
    }
}