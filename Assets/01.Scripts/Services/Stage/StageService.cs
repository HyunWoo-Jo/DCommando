using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Core;
using Zenject;
using Game.Core.Event;
using System;
namespace Game.Services
{
    public class StageService : IStageService, IInitializable, IDisposable
    {
        [Inject] private IAddressableService<EnemyName, GameObject> _enemyAddressableService;
        [Inject] private IAddressableService<StageName, SO_StageConfig> _stageConfigAddressableService;

        private SO_StageConfig _stageConfig;

        #region 초기화 Zenject에서 관리
        public void Initialize() {
            var enemyKeyDict = CSVReader.ReadToDictionary("AddressKey/EnemyAddressKey");
            _enemyAddressableService.RegisterAddressKeys(enemyKeyDict.ToEnumKey<EnemyName>());

            var stageKeyDict = CSVReader.ReadToDictionary("AddressKey/StageConfigAddressKey");
            _stageConfigAddressableService.RegisterAddressKeys(stageKeyDict.ToEnumKey<StageName>());
        }

        public void Dispose() {
            _enemyAddressableService.UnloadAll();
            _stageConfigAddressableService.UnloadAll();
        }
        #endregion

        public async UniTask<GameObject> GetEnemyObjectAsync(EnemyName enemyName) {
            return await _enemyAddressableService.LoadAssetAsync(enemyName);
        }

        public async UniTask<SO_StageConfig> GetStageConfigAsync(StageName stageName) {
            // 캐시에 있으면 반환
            if (_stageConfig != null) {
                return _stageConfig;
            }
            return await _stageConfigAddressableService.LoadAssetAsync(stageName);
        }

        public StageData[] GetAllStageData() {
            if (_stageConfig != null && _stageConfig.stages != null) {
                return _stageConfig.stages;
            }
            return new StageData[0];
        }

    }
}