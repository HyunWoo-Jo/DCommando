using System.Collections.Generic;
using UnityEngine;
using R3;
namespace Game.Models {
    public class StageModel {
        private Transform _playerTr;
        private Dictionary<int, GameObject> _spawnedEnemies = new();

        private ReactiveProperty<int> RP_remainingEnemyCount = new();
        private ReactiveProperty<int> RP_stageLevel = new();
        


        #region 프로퍼티
        public Transform PlayerTransform => _playerTr;

        public ReadOnlyReactiveProperty<int> RORP_RemainingEnemyCount => RP_remainingEnemyCount;
        public ReadOnlyReactiveProperty<int> RORP_StageLevel => RP_stageLevel;
        public int StageLevel => RP_stageLevel.Value;
        public int RemainingEnemyCount => RP_remainingEnemyCount.Value;
        #endregion
        #region Enemy 관련

        public GameObject GetEnemyObject(int id) {
            return _spawnedEnemies[id];
        }

        public void AddEnemy(GameObject enemy) {
            _spawnedEnemies.Add(enemy.GetInstanceID(), enemy);
            ++RP_remainingEnemyCount.Value;
        }

        public void RemoveEnemy(GameObject enemy) {
            _spawnedEnemies.Remove(enemy.GetInstanceID());
            --RP_remainingEnemyCount.Value;
        }
        public void RemoveEnemy(int enemyID) {
            _spawnedEnemies.Remove(enemyID);
            --RP_remainingEnemyCount.Value;
        }

        public void ClearDestroyEnemy() {
            foreach (var enemy in _spawnedEnemies) {
                if (enemy.Value != null) {
                    GameObject.Destroy(enemy.Value);
                }
            }
            _spawnedEnemies.Clear();
            RP_remainingEnemyCount.Value = 0;
        }

        public IDictionary<int, GameObject> GetAllEnemies() {
            return _spawnedEnemies;
        }
        #endregion

        #region Stage 관련
        public void SetStageLevel(int level) {
            RP_stageLevel.Value = level;
        }

        public void AddStageLevel() {
            ++RP_stageLevel.Value;
        }
        #endregion

        #region Player 관련
        public void SetPlayerTransform(Transform playerTransform) {
            _playerTr = playerTransform;
        }

        #endregion
    }
}
