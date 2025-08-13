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
namespace Game.Systems {
    public class StageSystem : IInitializable, IDisposable {
        [Inject] private IStageService _stageService;



        private Transform _enemyContainer;

        private CompositeDisposable _disposables = new();


        private StageName _stageName;
        private StageData _currentStageData;
        private List<GameObject> _spawnedEnemies = new List<GameObject>();
        private int _stageLevel = 0;
        private int _remainingEnemyCount;
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
            ++_stageLevel;
            _stageName = stageName;

            // Load
            var stageSo = await _stageService.GetStageConfigAsync(stageName);
            if (stageSo.stages.Length < _stageLevel) {
                GameDebug.LogWarning("최대 스테이지를 초과했습니다.");
                return;
            }
            _currentStageData = stageSo.stages[_stageLevel - 1];


            ClearEnemies();
            await SpawnEnemiesAsync();

            EventBus.Publish(new StageStartedEvent(_stageLevel, _currentStageData));
            GameDebug.Log($"Stage {_stageLevel} 시작 완료: {_remainingEnemyCount}개 Enemy 생성됨");
        }

        private async UniTask SpawnEnemiesAsync() {
            if (_currentStageData.enemyDatas == null || _currentStageData.enemyDatas.Length == 0) {
                GameDebug.LogWarning("Stage에 Enemy Wave 데이터가 없음");
                return;
            }

            _remainingEnemyCount = 0;

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

            _spawnedEnemies.Add(enemyInstance);
            _remainingEnemyCount++;

            GameDebug.Log($"Enemy 생성: {enemyData.enemyName} at {enemyData.spawnPosition}");
        }




        private void OnEnemyDefeated(EnemyDefeatedEvent evt) {
            _remainingEnemyCount--;
            _spawnedEnemies.RemoveAll(e => e == null || !e.activeInHierarchy);

            GameDebug.Log($"Enemy 처치됨. 남은 Enemy: {_remainingEnemyCount}");

            if (_remainingEnemyCount <= 0) {
                EndStage();
            }
        }

        private void EndStage() {
            GameDebug.Log($"Stage {_stageLevel} 종료");

            EventBus.Publish(new StageEndedEvent(_stageLevel, _currentStageData));
            ClearEnemies();

            if (_currentStageData.autoStartNextStage) {
                EventBus.Publish(new StartStageEvent(_stageName));
            }
        }

        private void ClearEnemies() {
            foreach (var enemy in _spawnedEnemies) {
                if (enemy != null) {
                    GameObject.Destroy(enemy);
                }
            }

            _spawnedEnemies.Clear();
            _remainingEnemyCount = 0;
        }

        public StageData GetCurrentStageData() {
            return _currentStageData;
        }

        public int GetCurrentStageId() {
            return _stageLevel;
        }

        public int GetRemainingEnemyCount() {
            return _remainingEnemyCount;
        }


    }
}