using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using R3;
using Game.Models;
using Game.Services;
using Game.Data;
using Game.Core;
using Game.Core.Event;
using System;
using Random = UnityEngine.Random;
using UnityEditor.Graphs;
namespace Game.Systems {
    public class UpgradeSystem : IInitializable, IDisposable {
        [Inject] private UpgradeModel _upgradeModel;
        [Inject] private IUpgradeService _upgradeService;
        [Inject] private ExpModel _expModel;
        [Inject] private StageModel _stageModel;
        [Inject] private GoldModel _goldModel;

        private CompositeDisposable _disposables = new();

        private int _remainingUpgradeCount = 0;

        #region 초기화
        public void Initialize() {
            var allUpgradeData = _upgradeService.GetUpgradeData();
            _upgradeModel.Initialize(allUpgradeData);

            // 이벤트 구독
            SubscribeToEvents();

            GameDebug.Log("UpgradeSystem 초기화 완료");
        }

        public void Dispose() {
            _disposables?.Dispose();
        }
        #endregion

        /// <summary>
        /// 이벤트 구독
        /// </summary>
        private void SubscribeToEvents() {
            // 레벨업 시 새로운 업그레이드 선택지 제공
            EventBus.Subscribe<LevelUpEvent>(OnLevelUp);

            // 리롤 요청 이벤트
            EventBus.Subscribe<UpgradeRerollEvent>(OnUpgradeReroll);
        }

        #region Event
        /// <summary>
        /// 레벨업 시 업그레이드 선택지 생성
        /// </summary>
        private void OnLevelUp(LevelUpEvent evt) {
            GameTime.Pause();
            if (_remainingUpgradeCount == 0) { // 최초 1회 표시
                GenerateUpgradeOptions();
                // UI에 업그레이드 선택 패널 표시 요청
                EventBus.Publish(new UICreationEvent(-1, UIName.UpgradePanel_UI));
            }
            _remainingUpgradeCount += evt.newLevel - evt.previousLevel;
        }

        #endregion

        /// <summary>
        /// 리롤 처리
        /// </summary>
        public void OnUpgradeReroll(UpgradeRerollEvent evt) {
            if (!_upgradeModel.TryDecreaseRerollCount()) {
                GameDebug.Log("리롤 횟수 부족");
                return;
            }

            // 선택된 인덱스의 업그레이드만 재생성
            RegenerateUpgradeOption(evt.selectedIndex);

            GameDebug.Log($"업그레이드 리롤 완료: 인덱스 {evt.selectedIndex}");
        }
        /// <summary>
        /// 인덱스로 업그레이드 선택 (ViewModel에서 호출)
        /// </summary>
        public void OnUpgradeSelectedByIndex(int index) {
            if (index < 0 || index >= _upgradeModel.SelectAblesCount) {
                GameDebug.LogError($"잘못된 업그레이드 인덱스: {index}");
                return;
            }

            var upgradeData = _upgradeModel.GetUpgradeDataByIndex(index);
            if (upgradeData == null) {
                GameDebug.LogError($"업그레이드 데이터가 null: 인덱스 {index}");
                return;
            }
            // Sprite 모두 Unload
            foreach (var upgrade in _upgradeModel.SelectAblesUpgradeList) {
                _upgradeService.UnloadSprite(upgrade.Value.upgradeID);
            }
            GameTime.Resume();
            OnUpgradeSelected(upgradeData);
        }
        /// <summary>
        /// 업그레이드 선택 처리
        /// </summary>
        public void OnUpgradeSelected(UpgradeData upgradeData) {
            // 업그레이드 적용
            ApplyUpgrade(upgradeData);
            // 모델에 선택된 업그레이드 추가
            _upgradeModel.SelectUpgrade(upgradeData);
            // UI 패널 닫기
            EventBus.Publish(new UICloseEvent(-1, UIName.UpgradePanel_UI));
            --_remainingUpgradeCount;
            GameDebug.Log($"업그레이드 선택됨: {upgradeData.upgradeName}");

            if (_remainingUpgradeCount > 0) { // 남은 카운트가 존재하면 다시 출력
                GenerateUpgradeOptions();
                // UI에 업그레이드 선택 패널 표시 요청
                EventBus.Publish(new UICreationEvent(-1, UIName.UpgradePanel_UI));
            }
        }

        public Sprite GetSprite(int upgradeID) {
            return _upgradeService.GetSprite(upgradeID);
        }


        /// <summary>
        /// 업그레이드 선택지 생성
        /// </summary>
        private void GenerateUpgradeOptions() {
            var allUpgrades = _upgradeService.GetUpgradeData();
            var availableUpgrades = GetAvailableUpgrades(allUpgrades);

            if (availableUpgrades.Count == 0) {
                GameDebug.LogWarning("사용 가능한 업그레이드가 없음");
                return;
            }

            // 랜덤으로 선택지 생성
            var selectedUpgrades = SelectRandomUpgrades(availableUpgrades, _upgradeModel.SelectAblesCount);
            _upgradeModel.SetSelectableUpgrades(selectedUpgrades);
        }

        /// <summary>
        /// 특정 인덱스의 업그레이드 선택지 재생성 (리롤용)
        /// </summary>
        private void RegenerateUpgradeOption(int index) {
            if (index < 0 || index >= _upgradeModel.SelectAblesCount) {
                GameDebug.LogError($"잘못된 리롤 인덱스: {index}");
                return;
            }

            var allUpgrades = _upgradeService.GetUpgradeData();
            var availableUpgrades = GetAvailableUpgrades(allUpgrades);

            if (availableUpgrades.Count == 0) {
                GameDebug.LogWarning("사용 가능한 업그레이드가 없음");
                return;
            }

            // 기존 Sprite Unload
            var curUpgradeData = _upgradeModel.GetUpgradeDataByIndex(index);
            _upgradeService.UnloadSprite(curUpgradeData.upgradeID);
            // 기존 선택지들 제외하고 새로운 업그레이드 선택
            var currentUpgrades = GetCurrentUpgradeOptions();
            var filteredUpgrades = availableUpgrades.Where(upgrade =>
                !currentUpgrades.Any(current => current?.upgradeID == upgrade.upgradeID)).ToList();

            if (filteredUpgrades.Count == 0) {
                // 필터링된 업그레이드가 없으면 전체에서 선택
                filteredUpgrades = availableUpgrades;
            }
            

            // 새로운 업그레이드 선택
            var newUpgrade = filteredUpgrades[Random.Range(0, filteredUpgrades.Count)];

            // 특정 인덱스만 업데이트
            var currentOptions = GetCurrentUpgradeOptions();
            currentOptions[index] = newUpgrade;
            _upgradeModel.SetSelectableUpgrades(currentOptions);
        }

        /// <summary>
        /// 현재 업그레이드 선택지 조회
        /// </summary>
        private List<UpgradeData> GetCurrentUpgradeOptions() {
            var options = new List<UpgradeData>();
            for (int i = 0; i < _upgradeModel.SelectAblesUpgradeList.Count; i++) {
                var upgrade = _upgradeModel.SelectAblesUpgradeList[i].CurrentValue;
                options.Add(upgrade);
            }
            return options;
        }

        /// <summary>
        /// 사용 가능한 업그레이드 필터링
        /// </summary>
        private List<UpgradeData> GetAvailableUpgrades(List<UpgradeData> allUpgrades) {
            return allUpgrades.Where(upgrade => IsUpgradeUnlocked(upgrade)).ToList();
        }

        /// <summary>
        /// 업그레이드 해금 조건 확인
        /// </summary>
        private bool IsUpgradeUnlocked(UpgradeData upgradeData) {
            if (upgradeData.upgradeConditionTypes == null || upgradeData.upgradeConditionTypes.Length == 0)
                return true;

            // 모든 조건을 확인
            for (int i = 0; i < upgradeData.upgradeConditionTypes.Length; i++) {
                if (!CheckCondition(upgradeData.upgradeConditionTypes[i],
                                  upgradeData.conditionOperators[i],
                                  upgradeData.conditionValues[i])) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 개별 조건 확인
        /// </summary>
        private bool CheckCondition(UpgradeConditionType conditionType, ConditionOperator conditionOperator, float conditionValue) {
            float currentValue = GetCurrentValueByConditionType(conditionType);

            return conditionOperator switch {
                ConditionOperator.Equal => Mathf.Approximately(currentValue, conditionValue),
                ConditionOperator.NotEqual => !Mathf.Approximately(currentValue, conditionValue),
                ConditionOperator.Greater => currentValue > conditionValue,
                ConditionOperator.GreaterOrEqual => currentValue >= conditionValue,
                ConditionOperator.Less => currentValue < conditionValue,
                ConditionOperator.LessOrEqual => currentValue <= conditionValue,
                ConditionOperator.Has => HasUpgradeById((int)conditionValue),
                ConditionOperator.NotHas => !HasUpgradeById((int)conditionValue),
                _ => false
            };
        }

        /// <summary>
        /// 특정 ID의 업그레이드를 보유하고 있는지 확인
        /// </summary>
        private bool HasUpgradeById(int upgradeId) {
            var selectedUpgrades = _upgradeModel.RORP_selectedUpgradeList.CurrentValue;
            return selectedUpgrades.Any(upgrade => upgrade.upgradeID == upgradeId);
        }

        /// <summary>
        /// 조건 타입에 따른 현재 값 조회
        /// </summary>
        private float GetCurrentValueByConditionType(UpgradeConditionType conditionType) {
            return conditionType switch {
                UpgradeConditionType.PlayerLevel => _expModel.RORP_CurrentLevel.CurrentValue,
                UpgradeConditionType.StageCleared => _stageModel.RORP_StageLevel.CurrentValue,
                UpgradeConditionType.TotalGold => _goldModel.RORP_CurrentGold.CurrentValue,
                UpgradeConditionType.UpgradeOwned => GetUpgradeOwnedCount(),
                UpgradeConditionType.ItemOwned => GetItemOwnedCount(),
                _ => 0f
            };
        }

        /// <summary>
        /// 보유 업그레이드 개수 조회
        /// </summary>
        private float GetUpgradeOwnedCount() {
            return _upgradeModel.RORP_selectedUpgradeList.CurrentValue.Count;
        }

        /// <summary>
        /// 보유 아이템 개수 조회 (구현 필요)
        /// </summary>
        private float GetItemOwnedCount() {
            // TODO: ItemModel과 연동하여 구현
            return 0f;
        }

        /// <summary>
        /// 랜덤 업그레이드 선택 (중복 방지)
        /// </summary>
        private List<UpgradeData> SelectRandomUpgrades(List<UpgradeData> availableUpgrades, int count) {
            if (availableUpgrades.Count <= count) {
                return new List<UpgradeData>(availableUpgrades);
            }

            var result = new List<UpgradeData>();
            var tempList = new List<UpgradeData>(availableUpgrades);

            for (int i = 0; i < count && tempList.Count > 0; i++) {
                int randomIndex = Random.Range(0, tempList.Count);
                result.Add(tempList[randomIndex]);
                tempList.RemoveAt(randomIndex);
            }

            return result;
        }

        /// <summary>
        /// 업그레이드 효과 적용
        /// </summary>
        private void ApplyUpgrade(UpgradeData upgradeData) {
            if (upgradeData.upgradeTypes == null || upgradeData.values == null) {
                GameDebug.LogError($"업그레이드 데이터 불완전: {upgradeData.upgradeName}");
                return;
            }

            for (int i = 0; i < upgradeData.upgradeTypes.Length && i < upgradeData.values.Length; i++) {
                ApplyUpgradeByType(upgradeData.upgradeTypes[i], upgradeData.values[i]);
            }

            // 업그레이드 적용 이벤트 발행
            EventBus.Publish(new UpgradeAppliedEvent(
                upgradeData.upgradeName,
                upgradeData.upgradeTypes,
                upgradeData.values
            ));
        }

        /// <summary>
        /// 업그레이드 타입별 효과 적용
        /// </summary>
        private void ApplyUpgradeByType(UpgradeType upgradeType, float value) {
            switch (upgradeType) {
                case UpgradeType.Power:
                EventBus.Publish(new StatChangeEvent(UpgradeType.Power, value));
                break;

                case UpgradeType.MaxHealth:
                EventBus.Publish(new StatChangeEvent(UpgradeType.MaxHealth, value));
                break;

                case UpgradeType.Heal:
                EventBus.Publish(new StatChangeEvent(UpgradeType.Heal, value));
                break;

                case UpgradeType.AttackSpeed:
                EventBus.Publish(new StatChangeEvent(UpgradeType.AttackSpeed, value));
                break;

                case UpgradeType.Defense:
                EventBus.Publish(new StatChangeEvent(UpgradeType.Defense, value));
                break;

                case UpgradeType.AttackRange:
                EventBus.Publish(new StatChangeEvent(UpgradeType.AttackRange, value));
                break;

                case UpgradeType.AttackWidth:
                EventBus.Publish(new StatChangeEvent(UpgradeType.AttackWidth,value));
                break;

                case UpgradeType.MoveSpeed:
                EventBus.Publish(new StatChangeEvent(UpgradeType.MoveSpeed, value));
                break;

                default:
                GameDebug.LogWarning($"처리되지 않은 업그레이드 타입: {upgradeType}");
                break;
            }

            GameDebug.Log($"업그레이드 적용: {upgradeType} +{value}");
        }

        /// <summary>
        /// 리롤 횟수 설정
        /// </summary>
        public void SetRerollCount(int count) {
            _upgradeModel.SetRerollCount(count);
        }

        /// <summary>
        /// 업그레이드 시스템 리셋
        /// </summary>
        public void Reset() {
            _upgradeModel.Reset();
            GameDebug.Log("UpgradeSystem 리셋 완료");
        }
    }
}