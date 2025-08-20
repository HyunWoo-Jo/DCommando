using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.ViewModels;
using Game.Core;
using System;

namespace Game.UI {
    public class UpgradeSlot : MonoBehaviour {
        [Header("Unity 레퍼")]
        [SerializeField] private Button _upgradeButton; // 업그레이드 선택 버튼
        [SerializeField] private Button _rerollButton; // 리롤 버튼
        [SerializeField] private Image _upgradeIcon; // 업그레이드 아이콘
        [SerializeField] private TextMeshProUGUI _upgradeName; // 업그레이드 이름
        [SerializeField] private TextMeshProUGUI _upgradeDescription; // 업그레이드 설명

        private int _slotIndex;
        private Action<int> _onUpgradeSelected;
        private Action<int> _onRerollRequested;

        /// <summary>
        /// 슬롯 초기화 (View에서 초기화
        /// </summary>
        public void Initialize(int slotIndex, Action<int> onUpgradeSelected, Action<int> onRerollRequested) {
            _slotIndex = slotIndex;
            _onUpgradeSelected = onUpgradeSelected;
            _onRerollRequested = onRerollRequested;

#if UNITY_EDITOR
            RefAssert();
#endif
            BindButtons();
        }

        /// <summary>
        /// 업그레이드 옵션 데이터 설정
        /// </summary>
        public void SetUpgradeOption(UpgradeOptionDataWithSprite option, bool canReroll) {
            bool hasOption = !string.IsNullOrEmpty(option.upgradeName);

            // 버튼 활성화/비활성화
            if (_upgradeButton != null) {
                _upgradeButton.interactable = hasOption;
            }

            if (_rerollButton != null) {
                _rerollButton.gameObject.SetActive(hasOption && canReroll);
            }

            if (hasOption) {
                // 아이콘 설정
                if (_upgradeIcon != null) {
                    _upgradeIcon.sprite = option.sprite;
                    _upgradeIcon.gameObject.SetActive(option.sprite != null);
                    _upgradeIcon.rectTransform.SetSizeSlot(option.sprite, 200);

                }

                // 이름 설정
                if (_upgradeName != null) {
                    _upgradeName.text = option.upgradeName;
                }

                // 설명 설정
                if (_upgradeDescription != null) {
                    _upgradeDescription.text = option.description;
                }
            } else {
                // 빈 슬롯 처리
                SetEmptySlot();
            }
        }

        /// <summary>
        /// 빈 슬롯 설정
        /// </summary>
        private void SetEmptySlot() {
            if (_upgradeIcon != null) {
                _upgradeIcon.gameObject.SetActive(false);
            }

            if (_upgradeName != null) {
                _upgradeName.text = "Empty";
            }

            if (_upgradeDescription != null) {
                _upgradeDescription.text = "";
            }
        }

        /// <summary>
        /// 버튼 이벤트 바인딩
        /// </summary>
        private void BindButtons() {
            if (_upgradeButton != null) {
                _upgradeButton.onClick.AddListener(() => _onUpgradeSelected?.Invoke(_slotIndex));
            }

            if (_rerollButton != null) {
                _rerollButton.onClick.AddListener(() => _onRerollRequested?.Invoke(_slotIndex));
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 검증
        /// </summary>
        private void RefAssert()
        {
            if (_upgradeButton == null)
            {
                GameDebug.LogError($"UpgradeSlot {_slotIndex}: _upgradeButton이 null입니다.");
            }

            if (_rerollButton == null)
            {
                GameDebug.LogError($"UpgradeSlot {_slotIndex}: _rerollButton이 null입니다.");
            }

            if (_upgradeIcon == null)
            {
                GameDebug.LogError($"UpgradeSlot {_slotIndex}: _upgradeIcon이 null입니다.");
            }

            if (_upgradeName == null)
            {
                GameDebug.LogError($"UpgradeSlot {_slotIndex}: _upgradeName이 null입니다.");
            }

            if (_upgradeDescription == null)
            {
                GameDebug.LogError($"UpgradeSlot {_slotIndex}: _upgradeDescription이 null입니다.");
            }
        }
#endif
    }
}