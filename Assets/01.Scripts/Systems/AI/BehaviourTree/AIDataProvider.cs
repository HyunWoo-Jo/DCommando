using Game.Models;
using UnityEngine;
using Zenject;
using System;
using Game.Core;
using System.Collections.Generic;
using System.Linq;

namespace Game.Systems
{
    /// <summary>
    /// AI�� �ʿ��� �����͸� �������ִ� Ŭ���� ���Ǹ� ���� (Zenject���� static �ۻ��� ����)
    /// </summary>
    public class AIDataProvider : IDisposable {
        private static AIDataProvider _instance;

        [Inject] private StageModel _stageModel;
        [Inject] private CombatModel _combatModel;
        [Inject] private HealthModel _healthModel;

        #region �ʱ�ȭ Zenject���� ����
        [Inject]
        public void Initialize(AIDataProvider provider) {
            _instance = provider;
            GameDebug.Log("AIDataProvider Static Facade �ʱ�ȭ �Ϸ�");
        }

        public void Dispose() {
            _instance = null;
            GameDebug.Log("AIDataProvider Static Facade ����");
        }
        #endregion

        #region static facade ����
        // �÷��̾� ����
        public static GameObject GetPlayer() => _instance?.GetPlayerInternal();
        public static Transform GetPlayerTransform() => _instance?.GetPlayerTransformInternal();
        public static Vector3 GetPlayerPosition() => _instance?.GetPlayerPositionInternal() ?? Vector3.zero;
        public static float GetPlayerHealth() => _instance?.GetPlayerHealthInternal() ?? -1;
        public static float GetPlayerMaxHealth() => _instance?.GetPlayerMaxHealthInternal() ?? -1;
        public static float GetPlayerHealthRatio() => _instance?.GetPlayerHealthRatioInternal() ?? -1;

        // �� ����
        public static Transform GetEnemyTransform(int enemyId) => _instance?.GetEnemyTransformInternal(enemyId);
        public static Vector3 GetEnemyPosition(int enemyId) => _instance?.GetEnemyPositionInternal(enemyId) ?? Vector3.zero;
        public static bool IsEnemyAlive(int enemyId) => _instance?.IsEnemyAliveInternal(enemyId) ?? false;
        public static IDictionary<int, GameObject> GetAllEnemies() => _instance.GetAllEnemiesInternal();
        public static Transform GetNearestEnemy(Vector3 position) => _instance?.GetNearestEnemyInternal(position);

        // �Ÿ� ���
        public static float GetDistanceBetween(Vector3 pos1, Vector3 pos2) => Vector3.Distance(pos1, pos2);
        public static float GetDistanceToPlayer(Vector3 position) => _instance?.GetDistanceToPlayerInternal(position) ?? float.MaxValue;
        public static bool IsInRange(Vector3 from, Vector3 to, float range) => Vector3.Distance(from, to) <= range;


        // ���� ����
        public static bool CanDealDamage(int attackerId, int targetId) => _instance?.CanDealDamageInternal(attackerId, targetId) ?? false;
        public static void DealDamage(int attackerId, int targetId, float damage) => _instance?.DealDamageInternal(attackerId, targetId, damage);

        // ���� ����
        public static bool IsGamePaused() => _instance.IsGamePausedInternal();
      
        #endregion

        #region ���� ����

        // �÷��̾� ���� ����
        private GameObject GetPlayerInternal() {
            return GetPlayerTransformInternal().gameObject;
        }

        private Transform GetPlayerTransformInternal() {
            return _stageModel.PlayerTransform;
        }

        private Vector3 GetPlayerPositionInternal() {
            var playerTransform = GetPlayerTransformInternal();
            return _stageModel.PlayerTransform.position;
        }
        private int GetPlayerInstanceID() {
            return GetPlayerInternal().GetInstanceID();
        }

        private float GetPlayerHealthInternal() {
            return _healthModel.GetCurrentHp(GetPlayerInstanceID());
        }

        private float GetPlayerMaxHealthInternal() {
            return _healthModel.GetMaxHp(GetPlayerInstanceID());
        }

        private float GetPlayerHealthRatioInternal() {
            float max = GetPlayerMaxHealthInternal();
            return _healthModel.GetHpRatio(GetPlayerInstanceID());
        }

        // �� ���� ����
        private Transform GetEnemyTransformInternal(int enemyId) {
            return _stageModel.GetEnemyObject(enemyId).transform;
        }

        private Vector3 GetEnemyPositionInternal(int enemyId) {
            var transform = GetEnemyTransformInternal(enemyId);
            return transform?.position ?? Vector3.zero;
        }

        private bool IsEnemyAliveInternal(int enemyId) {
            var enemy = GetEnemyTransformInternal(enemyId);
            return enemy != null && enemy.gameObject.activeInHierarchy;
        }

        private IDictionary<int, GameObject> GetAllEnemiesInternal() {
             return _stageModel.GetAllEnemies();
        }

        private Transform GetNearestEnemyInternal(Vector3 position) {
            var enemies = GetAllEnemiesInternal();
            Transform nearest = null;
            float minDistance = float.MaxValue;

            foreach (var enemy in enemies.Values) {
                if (enemy == null) continue;

                float distance = Vector3.Distance(position, enemy.transform.position);
                if (distance < minDistance) {
                    minDistance = distance;
                    nearest = enemy.transform;
                }
            }

            return nearest;
        }

        // �Ÿ� ��� ����
        private float GetDistanceToPlayerInternal(Vector3 position) {
            var playerPos = GetPlayerPositionInternal();
            return Vector3.Distance(position, playerPos);
        }


        // ���� ���� ����
        private bool CanDealDamageInternal(int attackerId, int targetId) {
            // CombatModel�� ���� ���� ���� ���� Ȯ��
            return _combatModel != null && IsEnemyAliveInternal(targetId);
        }

        private void DealDamageInternal(int attackerId, int targetId, float damage) {
            // CombatModel�� ���� ���� ������ ó��
            if (_combatModel != null) {
                GameDebug.Log($"������ ó�� Attacker {attackerId} -> Target {targetId}: {damage} damage");
                // _combatModel.ProcessDamage(attackerId, targetId, damage);
            }
        }

        // ���� ���� ����
        private bool IsGamePausedInternal() {
            return GameTime.IsPaused;
        }
        #endregion
    }
}
