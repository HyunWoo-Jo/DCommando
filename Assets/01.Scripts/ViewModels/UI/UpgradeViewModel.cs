using R3;
using System;
using System.Collections.Generic;
using Zenject;
using UnityEngine;
using Game.Core;
using Game.Models;
using Game.Systems;
using Game.Core.Event;

namespace Game.ViewModels {
    public class UpgradeViewModel : IInitializable, IDisposable {
        [Inject] private UpgradeModel _upgradeModel;
        [Inject] private UpgradeSystem _upgradeSystem;

        // 읽기 전용 프로퍼티
        public ReadOnlyReactiveProperty<int> RORP_RerollCount { get; private set; }
        public ReadOnlyReactiveProperty<List<UpgradeOptionDataWithSprite>> RORP_UpgradeOptions { get; private set; }

        // 해제용
        private CompositeDisposable _disposable = new();

        #region Zenject에서 관리

        public void Initialize() {
            // 리롤 횟수 변경
            RORP_RerollCount = _upgradeModel.RORP_rerollCount
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);

            // 업그레이드 선택지 변경 (스프라이트 추가)
            RORP_UpgradeOptions = _upgradeModel.RORP_upgradeOptions
                .Select(AddSpritesToUpgradeOptions)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);
        }

        public void Dispose() {
            RORP_RerollCount?.Dispose();
            RORP_UpgradeOptions?.Dispose();
            _disposable?.Dispose();
        }
        #endregion

        /// <summary>
        /// 업그레이드 선택 (인덱스 기반)
        /// </summary>
        public void SelectUpgrade(int index) {
            _upgradeSystem.OnUpgradeSelectedByIndex(index);
        }

        /// <summary>
        /// 리롤 요청 (인덱스 기반)
        /// </summary>
        public void RequestReroll(int index) {
            if (index < 0 || index >= _upgradeModel.SelectAblesCount) {
                GameDebug.LogError($"잘못된 리롤 인덱스: {index}");
                return;
            }

            EventBus.Publish(new UpgradeRerollEvent(index));
        }

        /// <summary>
        /// 업그레이드 옵션에 스프라이트 추가
        /// </summary>
        private List<UpgradeOptionDataWithSprite> AddSpritesToUpgradeOptions(List<UpgradeOptionData> options) {
            var result = new List<UpgradeOptionDataWithSprite>();

            foreach (var option in options) {
                Sprite sprite = _upgradeSystem.GetSprite(option.id);
                result.Add(new UpgradeOptionDataWithSprite {
                    index = option.index,
                    upgradeName = option.upgradeName,
                    description = option.description,
                    sprite = sprite
                });
            }

            return result;
        }
    }

    /// <summary>
    /// UI에서 사용할 스프라이트 포함 업그레이드 옵션 데이터
    /// </summary>
    [System.Serializable]
    public class UpgradeOptionDataWithSprite {
        public int index;
        public int id;
        public string upgradeName;
        public string description;
        public Sprite sprite;
    }
}