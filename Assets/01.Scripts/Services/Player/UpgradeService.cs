using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Game.Data;
using Game.Core;
using Game.Services;
using System;

namespace Game.Services {
    public class UpgradeService : IUpgradeService, IInitializable, IDisposable {
        [Inject] private IAddressableService<int, Sprite> _spriteAddressables;
        private List<UpgradeData> _upgradeData;
        private const char _SPRITE_CHAR = '/';

        #region 초기화 Zenject에서 관리
        public void Initialize() {
            Dictionary<string, List<string>> tableDict = CSVReader.ReadToMultiColumnDictionary("UpgradeTable/UpgradeTable.csv");
            Dictionary<int, string> spriteKeyDict = new();
            _upgradeData = new List<UpgradeData>();

            foreach (var table in tableDict) {
                string upgradeName = table.Key;
                List<string> columns = table.Value;

                // CSV 컬럼 순서:
                // upgradeName,
                // discription,
                // UpgradeID,
                // upgradeCount,
                // upgradeTypes,

                // values,
                // upgradeConditionTypes,
                // conditionOperators,
                // conditionValues,
                // spriteAddressableKey
                if (columns.Count < 10) {
                    GameDebug.LogError($"업그레이드 데이터 부족: {upgradeName}, 컬럼 수: {columns.Count}");
                    continue;
                }

                var upgradeData = new UpgradeData {
                    upgradeName = columns[0],
                    discription = columns[1],
                    upgradeID = ParseInt(columns[2]),
                    upgradeCount = ParseInt(columns[3]),
                    upgradeTypes = ParseUpgradeTypes(columns[4]),
                    values = ParseFloatArray(columns[5]),
                    upgradeConditionTypes = ParseUpgradeConditionTypes(columns[6]),
                    conditionOperators = ParseConditionOperators(columns[7]),
                    conditionValues = ParseFloatArray(columns[8]),
                    spriteAddressableKey = columns[9] // string 그대로 사용
                };

                _upgradeData.Add(upgradeData);

                // 스프라이트 키 등록 (첫 번째 upgradeType 사용)
                if (upgradeData.upgradeTypes != null && upgradeData.upgradeTypes.Length > 0) {
                    spriteKeyDict[upgradeData.upgradeID] = upgradeData.spriteAddressableKey;
                }
            }

            _spriteAddressables.RegisterAddressKeys(spriteKeyDict);
            GameDebug.Log($"업그레이드 데이터 로드 완료: {_upgradeData.Count}개");
        }

        public void Dispose() {
            _spriteAddressables.UnloadAll();
        }
        #endregion

        public List<UpgradeData> GetUpgradeData() => _upgradeData;

        public Sprite GetSprite(int upgradeId) {
            if (!_spriteAddressables.HasAddressKey(upgradeId)) return null;
            return _spriteAddressables.LoadAsset(upgradeId);
        }

        public void UnloadSprite(int upgradeId) {
            _spriteAddressables.UnloadAsset(upgradeId);
        }

        /// <summary>
        /// 특정 이름의 업그레이드 데이터 조회
        /// </summary>
        public UpgradeData GetUpgradeDataByName(string upgradeName) {
            return _upgradeData.FirstOrDefault(data => data.upgradeName == upgradeName);
        }

        /// <summary>
        /// 특정 타입의 업그레이드 데이터 리스트 조회
        /// </summary>
        public List<UpgradeData> GetUpgradeDataByType(UpgradeType upgradeType) {
            return _upgradeData.Where(data => data.upgradeTypes != null && data.upgradeTypes.Contains(upgradeType)).ToList();
        }

        #region CSV 파싱 헬퍼 메서드
        private int ParseInt(string value) {
            return int.TryParse(value, out int result) ? result : 0;
        }

        private float[] ParseFloatArray(string value) {
            if (string.IsNullOrEmpty(value)) return new float[0];

            string[] parts = value.Split(_SPRITE_CHAR);
            float[] result = new float[parts.Length];

            for (int i = 0; i < parts.Length; i++) {
                result[i] = float.TryParse(parts[i], out float f) ? f : 0f;
            }

            return result;
        }

        private UpgradeType[] ParseUpgradeTypes(string value) {
            if (string.IsNullOrEmpty(value)) return new UpgradeType[0];

            string[] parts = value.Split(_SPRITE_CHAR);
            UpgradeType[] result = new UpgradeType[parts.Length];

            for (int i = 0; i < parts.Length; i++) {
                if (System.Enum.TryParse<UpgradeType>(parts[i], out UpgradeType type)) {
                    result[i] = type;
                }
            }

            return result;
        }

        private UpgradeConditionType[] ParseUpgradeConditionTypes(string value) {
            if (string.IsNullOrEmpty(value)) return new UpgradeConditionType[0];

            string[] parts = value.Split(_SPRITE_CHAR);
            UpgradeConditionType[] result = new UpgradeConditionType[parts.Length];

            for (int i = 0; i < parts.Length; i++) {
                if (System.Enum.TryParse<UpgradeConditionType>(parts[i], out UpgradeConditionType type)) {
                    result[i] = type;
                }
            }

            return result;
        }

        private ConditionOperator[] ParseConditionOperators(string value) {
            if (string.IsNullOrEmpty(value)) return new ConditionOperator[0];

            string[] parts = value.Split(_SPRITE_CHAR);
            ConditionOperator[] result = new ConditionOperator[parts.Length];

            for (int i = 0; i < parts.Length; i++) {
                if (System.Enum.TryParse<ConditionOperator>(parts[i], out ConditionOperator op)) {
                    result[i] = op;
                }
            }

            return result;
        }
        #endregion
    }
}