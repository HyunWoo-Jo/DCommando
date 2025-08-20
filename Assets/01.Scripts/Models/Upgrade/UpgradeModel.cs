using System.Collections.Generic;
using System.Linq;
using R3;
using Game.Data;
using Game.Core;
using System;

namespace Game.Models {
    public class UpgradeModel: IDisposable {
        private List<UpgradeData> _allUpgradeDataList; // 모든 업그레이드 목록
        private ReactiveProperty<int> RP_rerollCount = new(3); // 리롤 가능한 횟수
        private ReactiveProperty<List<UpgradeData>> RP_selectedUpgradeList = new(new List<UpgradeData>()); // 선택된 업그레이드 목록
        private int _selectAblesCount = 2; // 선택 가능한 업그레이드 개수
        private List<ReactiveProperty<UpgradeData>> RP_selectAblesUpgradeList = new(); // 선택 가능한 업그레이드

        // R3
        public ReadOnlyReactiveProperty<int> RORP_rerollCount => RP_rerollCount;
        public ReadOnlyReactiveProperty<List<UpgradeData>> RORP_selectedUpgradeList => RP_selectedUpgradeList;
        public int SelectAblesCount => _selectAblesCount;
        public IReadOnlyList<ReactiveProperty<UpgradeData>> SelectAblesUpgradeList => RP_selectAblesUpgradeList.AsReadOnly();

        // UI용 변환된 데이터 (추가)
        public ReadOnlyReactiveProperty<List<UpgradeOptionData>> RORP_upgradeOptions { get; private set; }

        private CompositeDisposable _disposables = new();

        /// <summary>
        /// 업그레이드 모델 초기화 System을 통해 주입
        /// </summary>
        public void Initialize(List<UpgradeData> allUpgradeData) {
            _allUpgradeDataList = allUpgradeData;

            // 선택 가능한 업그레이드 리스트 초기화
            RP_selectAblesUpgradeList.Clear();
            for (int i = 0; i < _selectAblesCount; i++) {
                RP_selectAblesUpgradeList.Add(new ReactiveProperty<UpgradeData>(null));
            }

            // UI용 변환된 데이터 스트림 생성
            RORP_upgradeOptions = Observable.CombineLatest(
                    RP_selectAblesUpgradeList[0],
                    RP_selectAblesUpgradeList[1],
                    (slot0, slot1) => new List<UpgradeData> { slot0, slot1 }
                )
                .Select(ConvertToUpgradeOptionData)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }
        public void Dispose() {
            _disposables?.Dispose();
        }

        /// <summary>
        /// 리롤 횟수 설정
        /// </summary>
        public void SetRerollCount(int count) {
            RP_rerollCount.Value = count;
        }

        /// <summary>
        /// 리롤 횟수 감소
        /// </summary>
        public bool TryDecreaseRerollCount() {
            if (RP_rerollCount.Value > 0) {
                RP_rerollCount.Value--;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 업그레이드 선택
        /// </summary>
        public void SelectUpgrade(UpgradeData upgradeData) {
            if (upgradeData == null) return;
            var currentList = RP_selectedUpgradeList.Value;
            var newList = new List<UpgradeData>(currentList) { upgradeData };
            RP_selectedUpgradeList.Value = newList;
        }

        /// <summary>
        /// 선택 가능한 업그레이드 설정
        /// </summary>
        public void SetSelectableUpgrades(List<UpgradeData> upgrades) {
            for (int i = 0; i < _selectAblesCount; i++) {
                if (i < upgrades.Count) {
                    RP_selectAblesUpgradeList[i].Value = upgrades[i];
                } else {
                    RP_selectAblesUpgradeList[i].Value = null;
                }
            }
        }

        /// <summary>
        /// 인덱스로 업그레이드 데이터 조회 (System에서 사용)
        /// </summary>
        public UpgradeData GetUpgradeDataByIndex(int index) {
            if (index < 0 || index >= RP_selectAblesUpgradeList.Count)
                return null;

            return RP_selectAblesUpgradeList[index].CurrentValue;
        }

        /// <summary>
        /// UpgradeData를 UpgradeOptionData로 변환 (Model 내부에서 처리)
        /// </summary>
        private List<UpgradeOptionData> ConvertToUpgradeOptionData(List<UpgradeData> upgradeDataList) {
            var result = new List<UpgradeOptionData>();

            for (int i = 0; i < upgradeDataList.Count; i++) {
                var upgradeData = upgradeDataList[i];

                if (upgradeData == null) {
                    result.Add(new UpgradeOptionData {
                        index = i,
                        upgradeName = string.Empty,
                        description = string.Empty,
                    });
                } else {
                    result.Add(new UpgradeOptionData {
                        index = i,
                        id = upgradeData.upgradeID,
                        upgradeName = upgradeData.upgradeName,
                        description = upgradeData.discription,
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// 모든 데이터 초기화
        /// </summary>
        public void Reset() {
            RP_rerollCount.Value = 0;
            RP_selectedUpgradeList.Value = new List<UpgradeData>();

            foreach (var upgrade in RP_selectAblesUpgradeList) {
                upgrade.Value = null;
            }
        }
    }

    /// <summary>
    /// UI에서 사용할 업그레이드 옵션 데이터 (Data 영역 참조 방지)
    /// </summary>
    [System.Serializable]
    public class UpgradeOptionData {
        public int index;
        public int id;
        public string upgradeName;
        public string description;
    }
}