using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Game.Core;
using R3;

namespace Game.Models {
    /// <summary>
    /// 공격력과 방어력을 관리하는 모델
    /// </summary>
    public class CombatModel : IInitializable, IDisposable {
        // 상수
        private const int MIN_ATTACK = 0; // 최소
        private const int MIN_DEFENSE = 0;

        private const int DEFAULT_BASE_ATTACK = 5; // 기본 
        private const int DEFAULT_BASE_DEFENSE = 0;
        private const float DEFAULT_ATTACK_SPEED = 1;

        private const float DEFAULT_MULTIPLIER = 1.0f; // 기본 배율

        // 모든 캐릭터의 전투 데이터를 Dictionary로 관리
        private readonly Dictionary<int, CombatModel> _weaponDataDict = new();
        private readonly Dictionary<int, ReactiveProperty<CombatData>> _combatDataDict = new();
        private bool _disposed = false;


        public ReadOnlyReactiveProperty<CombatData> GetRORP_CombatData(int id) {
             if (!_combatDataDict.TryGetValue(id, out var property)) return null;
            return property;
        }

        #region 초기화, 등록
        // 초기화
        public void Initialize() {
            // 필요시 기본 설정
        }

        // 데이터 생성
        public void RegisterCombat(int id, int baseAttack = DEFAULT_BASE_ATTACK, int baseDefense = DEFAULT_BASE_DEFENSE, float attackSpeed = DEFAULT_ATTACK_SPEED) {
            if (_combatDataDict.ContainsKey(id)) {
                GameDebug.LogError($"이미 생성된 전투 데이터 ID: {id}");
                return;
            }

            var combatData = new CombatData(baseAttack, baseDefense, attackSpeed);
            _combatDataDict[id] = new ReactiveProperty<CombatData>(combatData);
        }

        public void RegisterCombat(int id, CombatData combatData) {
            if (_combatDataDict.ContainsKey(id)) {
                GameDebug.LogError($"이미 생성된 전투 데이터 ID: {id}");
                return;
            }

            _combatDataDict[id] = new ReactiveProperty<CombatData>(combatData);
        }

        // 등록된 ReadOnlyReactiveProperty 가져오기
        public ReadOnlyReactiveProperty<CombatData> GetCombatProperty(int id) {
            if (_combatDataDict.TryGetValue(id, out var property)) {
                return property.ToReadOnlyReactiveProperty();
            }

            GameDebug.LogError($"존재하지 않는 Combat ID: {id}");
            return null;
        }

        // 데이터 존재 확인
        public bool HasCombat(int id) {
            return _combatDataDict.ContainsKey(id);
        }

        // 데이터 제거
        public void RemoveCombat(int id) {
            if (_combatDataDict.TryGetValue(id, out var property)) {
                property.Dispose();
                _combatDataDict.Remove(id);
            }
        }
        #endregion
        #region 공격력 관련 메서드

        // 기본 공격력 설정
        public void SetBaseAttack(int id, int attack) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.baseAttack = Mathf.Max(MIN_ATTACK, attack);
            property.Value = data;
        }

        // 추가 공격력 설정 (장비, 버프 등)
        public void SetBonusAttack(int id, int bonusAttack) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.bonusAttack = Mathf.Max(0, bonusAttack);
            property.Value = data;
        }

        // 공격력 배율 설정
        public void SetAttackMultiplier(int id, float multiplier) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.attackMultiplier = Mathf.Max(0f, multiplier);
            property.Value = data;
        }

        // 공격력 증가/감소
        public void AddBaseAttack(int id, int amount) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.baseAttack = Mathf.Max(MIN_ATTACK, data.baseAttack + amount);
            property.Value = data;
        }

        public void AddBonusAttack(int id, int amount) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.bonusAttack = Mathf.Max(0, data.bonusAttack + amount);
            property.Value = data;
        }

        public void MultiplyAttack(int id, float multiplier) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.attackMultiplier = Mathf.Max(0f, data.attackMultiplier * multiplier);
            property.Value = data;
        }

        #endregion

        #region 방어력 관련 메서드

        // 기본 방어력 설정
        public void SetBaseDefense(int id, int defense) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.baseDefense = Mathf.Max(MIN_DEFENSE, defense);
            property.Value = data;
        }

        // 추가 방어력 설정 (장비, 버프 등)
        public void SetBonusDefense(int id, int bonusDefense) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.bonusDefense = Mathf.Max(0, bonusDefense);
            property.Value = data;
        }

        // 방어력 배율 설정
        public void SetDefenseMultiplier(int id, float multiplier) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.defenseMultiplier = Mathf.Max(0f, multiplier);
            property.Value = data;
        }
        // 공격속도 배율 설정
        public void SetAttackSpeedMultiplier(int id, float multiplier) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;
            var data = property.Value;
            data.attackMultiplier = Mathf.Max(0f, multiplier);
            property.Value = data;
        }

        // 방어력 증가/감소
        public void AddBaseDefense(int id, int amount) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.baseDefense = Mathf.Max(MIN_DEFENSE, data.baseDefense + amount);
            property.Value = data;
        }

        public void AddBonusDefense(int id, int amount) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.bonusDefense = Mathf.Max(0, data.bonusDefense + amount);
            property.Value = data;
        }

        public void AddBonusAttackSpeed(int id, float amount) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.bonusAttackSpeed = Mathf.Max(0, data.bonusAttackSpeed + amount);
            property.Value = data;
        }
       

        public void MultiplyDefense(int id, float multiplier) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.defenseMultiplier = Mathf.Max(0f, data.defenseMultiplier * multiplier);
            property.Value = data;
        }

        #endregion

        #region 상태 조회 메서드들

        // 최종 공격력 조회
        public int GetFinalAttack(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.FinalAttack;
        }

        public int GetBaseAttack(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.baseAttack;
        }

        public int GetBonusAttack(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.bonusAttack;
        }

        public float GetAttackMultiplier(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 1.0f;
            return property.Value.attackMultiplier;
        }

        // 최종 방어력 조회
        public int GetFinalDefense(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.FinalDefense;
        }

        public int GetBaseDefense(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.baseDefense;
        }

        public int GetBonusDefense(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.bonusDefense;
        }

        public float GetDefenseMultiplier(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 1.0f;
            return property.Value.defenseMultiplier;
        }

        public float GetFinalAttackSpeed(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property)) return 0.0f;
            return property.Value.FinalAttackSpeed;
        }

        // 전체 전투 데이터 조회
        public CombatData GetCombatData(int id) {
            if (!_combatDataDict.TryGetValue(id, out var property))
                return new CombatData(0, 0, 0);
            return property.Value;
        }

        #endregion

        #region 전체 데이터 관리

        public int GetCombatCount() {
            return _combatDataDict.Count;
        }

        public IEnumerable<int> GetAllCombatIDs() {
            return _combatDataDict.Keys;
        }

        // 전체 초기화 (Initialize와 다름 - 런타임 데이터 리셋)
        public void ClearAllData() {
            foreach (var property in _combatDataDict.Values) {
                property.Dispose();
            }
            _combatDataDict.Clear();
        }

        #endregion

        #region 리소스 정리

        public void Dispose() {
            if (_disposed) return;
            _disposed = true;

            foreach (var property in _combatDataDict.Values) {
                property?.Dispose();
            }
            _combatDataDict.Clear();
        }

        #endregion
    }
}