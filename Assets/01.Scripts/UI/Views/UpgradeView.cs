using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;
using Game.ViewModels;
using Game.Core;
using Zenject;
using System.Collections.Generic;

namespace Game.UI {
    public class UpgradeView : MonoBehaviour {
        [Inject] private UpgradeViewModel _viewModel;

        [Header("Unity 레퍼")]
        private UpgradeSlot[] _upgradeSlots; // 업그레이드 슬롯들
        [SerializeField] private TextMeshProUGUI _rerollCountText; // 리롤 횟수 텍스트

        private void Awake() {
            _upgradeSlots = GetComponentsInChildren<UpgradeSlot>();
#if UNITY_EDITOR
            RefAssert();
#endif

        }
        private void Start() {
            gameObject.Resize();
            Bind();
        }

#if UNITY_EDITOR
        private void RefAssert() {
            if (_upgradeSlots == null || _upgradeSlots.Length != 2) {
                GameDebug.LogError("UpgradeView: _upgradeSlots 배열이 2개가 아닙니다.");
            }

            if (_rerollCountText == null) {
                GameDebug.LogError("UpgradeView: _rerollCountText가 null입니다.");
            }
        }
#endif

        private void Bind() {
            // 슬롯 초기화
            InitializeSlots();

            // 리롤 횟수 바인딩
            if (_rerollCountText != null) {
                _viewModel.RORP_RerollCount
                    .Subscribe(count => _rerollCountText.text = $"Reroll: {count}")
                    .AddTo(this);
            }

            // 업그레이드 옵션들 바인딩
            _viewModel.RORP_UpgradeOptions
                .Subscribe(UpdateUpgradeSlots)
                .AddTo(this);
        }

        /// <summary>
        /// 슬롯 초기화
        /// </summary>
        private void InitializeSlots() {
            for (int i = 0; i < _upgradeSlots.Length; i++) {
                if (_upgradeSlots[i] != null) {
                    _upgradeSlots[i].Initialize(
                        i,
                        _viewModel.SelectUpgrade,
                        _viewModel.RequestReroll
                    );
                }
            }
        }

        /// <summary>
        /// 업그레이드 슬롯들 업데이트
        /// </summary>
        private void UpdateUpgradeSlots(List<UpgradeOptionDataWithSprite> options) {
            bool canReroll = _viewModel.RORP_RerollCount.CurrentValue > 0;

            for (int i = 0; i < _upgradeSlots.Length; i++) {
                if (_upgradeSlots[i] != null) {
                    UpgradeOptionDataWithSprite option;

                    if (i < options.Count) {
                        option = options[i];
                    } else {
                        // 빈 옵션 생성
                        option = new UpgradeOptionDataWithSprite {
                            index = i,
                            id = -1,
                            upgradeName = string.Empty,
                            description = string.Empty,
                            sprite = null
                        };
                    }

                    _upgradeSlots[i].SetUpgradeOption(option, canReroll);
                }
            }
        }


#if UNITY_EDITOR
        [ContextMenu("Test All Reroll")]
        private void TestAllReroll() {
            for (int i = 0; i < _upgradeSlots.Length; i++) {
                if (_upgradeSlots[i] != null) {
                    _viewModel.RequestReroll(i);
                }
            }
            GameDebug.Log("모든 슬롯 리롤 테스트");
        }

        [ContextMenu("Test All Select")]
        private void TestAllSelect() {
            if (_upgradeSlots[0] != null) {
                _viewModel.SelectUpgrade(0);
                GameDebug.Log("첫 번째 슬롯 선택 테스트");
            }
        }
#endif
    }
}