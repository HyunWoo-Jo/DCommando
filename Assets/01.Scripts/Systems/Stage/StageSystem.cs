using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Game.Core;
using Game.Services;
using Game.Data;
using Game.Core.Event;
using R3;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;
using Game.Models;
namespace Game.Systems {
    public class StageSystem : IInitializable, IDisposable {
        [Inject] private IStageService _stageService;
        [Inject] private StageModel _stageModel;


        private Transform _enemyContainer;

        private CompositeDisposable _disposables = new();


        private StageName _stageName;
        private StageData _currentStageData;

        #region 초기화 Zenject에서 관리
        public void Initialize() {
            _enemyContainer = new GameObject("Enemy Container").transform;
            _enemyContainer.transform.position = Vector3.zero;
            EventBus.Subscribe<StartStageEvent>(OnStartStage).AddTo(_disposables);
            EventBus.Subscribe<EnemyDefeatedEvent>(OnEnemyDefeated).AddTo(_disposables);
        }

        public void Dispose() {
            _disposables?.Dispose();
        }
        #endregion
        private async void OnStartStage(StartStageEvent evt) {
            await StartStageAsync(evt.stageNage);
        }

        /// <summary>
        /// 스테이지 시작
        /// </summary>
        public async UniTask StartStageAsync(StageName stageName) {
            _stageModel.AddStageLevel();
            _stageName = stageName;
            int stageLevel = _stageModel.StageLevel;
            // Load
            var stageSo = await _stageService.GetStageConfigAsync(stageName);
            if (stageSo.stages.Length < stageLevel) {
                EventBus.Publish(new GameWinEvent(_stageName, stageLevel - 1));
                GameTime.Pause();
                return;
            }
            _currentStageData = stageSo.stages[stageLevel - 1];


            ClearEnemies();
            await SpawnEnemiesAsync();

            EventBus.Publish(new StageStartedEvent(stageLevel, _currentStageData));
            GameDebug.Log($"Stage {stageLevel} 시작 완료: {_stageModel.RemainingEnemyCount}개 Enemy 생성됨");
        }

        private async UniTask SpawnEnemiesAsync() {
            if (_currentStageData.enemyDatas == null || _currentStageData.enemyDatas.Length == 0) {
                GameDebug.LogWarning("Stage에 Enemy Wave 데이터가 없음");
                return;
            }

            // 모든 Enemy 스폰
            foreach (var enemyData in _currentStageData.enemyDatas) {
                await SpawnEnemyAsync(enemyData);
            }
        }

        private async UniTask SpawnEnemyAsync(EnemyData enemyData) {
            var enemyPrefab = await _stageService.GetEnemyObjectAsync(enemyData.enemyName);

            if (enemyPrefab == null) {
                GameDebug.LogError($"Enemy 프리팹 로드 실패: {enemyData.enemyName}");
                return;
            }

            var enemyInstance = DIHelper.InstantiateWithInjection(enemyPrefab, _enemyContainer);
            enemyInstance.transform.position = enemyData.spawnPosition;

            var healthComponent = enemyInstance.GetComponent<HealthComponent>();
            if (healthComponent != null) {
                healthComponent.Initialize(enemyData.enemyHealth);
            }
            _stageModel.AddEnemy(enemyInstance);

            GameDebug.Log($"Enemy 생성: {enemyData.enemyName} at {enemyData.spawnPosition}");
        }




        private void OnEnemyDefeated(EnemyDefeatedEvent evt) {
            _stageModel.RemoveEnemy(evt.enemyId);
            GameDebug.Log($"Enemy 처치됨. 남은 Enemy: {_stageModel.RemainingEnemyCount}");

            if (_stageModel.RemainingEnemyCount <= 0) {
                EndStage();
            }
        }

        private void EndStage() {
            GameDebug.Log($"Stage {_stageModel.StageLevel} 종료");

            EventBus.Publish(new StageEndedEvent(_stageModel.StageLevel, _currentStageData));
            ClearEnemies();

            if (_currentStageData.autoStartNextStage) {
                EventBus.Publish(new StartStageEvent(_stageName));
            }
        }

        private void ClearEnemies() {
            _stageModel.ClearDestroyEnemy();
        }



        public StageData GetCurrentStageData() {
            return _currentStageData;
        }

        public int GetCurrentStageId() {
            return _stageModel.StageLevel;
        }

        public int GetRemainingEnemyCount() {
            return _stageModel.RemainingEnemyCount;
        }


    }
}