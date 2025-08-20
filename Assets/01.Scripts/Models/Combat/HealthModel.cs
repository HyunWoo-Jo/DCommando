using Game.Core;
using Game.Data;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Models {
    public class HealthModel : IDisposable {
        // 상수
        private const int MIN_HP = 0;
        private const int DEFAULT_MAX_HP = 100;
        private const int REVIVE_MIN_HP = 1;


        // 모든 캐릭터의 체력 데이터를 Dictionary로 관리
        private readonly Dictionary<int, ReactiveProperty<HealthData>> _healthDataDict = new();
        private bool _disposed = false;

        // 데이터 생성
        public void AddHealth(int id, int maxHp = DEFAULT_MAX_HP) {
            if (_healthDataDict.ContainsKey(id)) {
                GameDebug.LogError($"이미 생성된 데이터 ID: {id}");
                return;
            }

            var healthData = new HealthData(maxHp);
            _healthDataDict[id] = new ReactiveProperty<HealthData>(healthData);
        }

        public void AddHealth(int id, HealthData healthData) {
            if (_healthDataDict.ContainsKey(id)) {
                GameDebug.LogError($"이미 생성된 데이터 ID: {id}");
                return;
            }

            _healthDataDict[id] = new ReactiveProperty<HealthData>(healthData);
        }

        // 등록된 ReadOnlyReactiveProperty 가져오기
        public ReadOnlyReactiveProperty<HealthData> GetHealthProperty(int id) {
            if (_healthDataDict.TryGetValue(id, out var property)) {
                return property.ToReadOnlyReactiveProperty();
            }

            GameDebug.LogError($"존재하지 않는 Health ID: {id}");
            return null;
        }

        // 데이터 존재 확인
        public bool HasHealth(int id) {
            return _healthDataDict.ContainsKey(id);
        }

        // 데이터 제거
        public void RemoveHealth(int id) {
            if (_healthDataDict.TryGetValue(id, out var property)) {
                property.Dispose();
                _healthDataDict.Remove(id);
            }
        }

        // 기본 체력 설정
        public void SetMaxHp(int id, int hp) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.maxHp = hp;

            // 현재 체력이 최대 체력을 초과하지 않도록
            if (data.currentHp > hp) {
                data.currentHp = hp;
            }

            CheckDead(ref data);
            property.Value = data;
        }

        public void SetCurrentHp(int id, int hp) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            data.currentHp = Mathf.Clamp(hp, MIN_HP, data.maxHp);
            CheckDead(ref data);
            property.Value = data;
        }

        // 데미지 받기
        public void TakeDamage(int id, int damage) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            if (data.isDead) return;

            data.currentHp = Mathf.Max(MIN_HP, data.currentHp - damage);
            CheckDead(ref data);
            property.Value = data;
        }

        // 치료
        public void Heal(int id, int healAmount) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            if (data.isDead) return;

            data.currentHp = Mathf.Min(data.maxHp, data.currentHp + healAmount);
            CheckDead(ref data);
            property.Value = data;
        }

        // 부활
        public void Revive(int id, int reviveHp = -1) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return;

            var data = property.Value;
            int hp = reviveHp == -1 ? data.maxHp : reviveHp;
            data.currentHp = Mathf.Clamp(hp, REVIVE_MIN_HP, data.maxHp);
            data.isDead = false;
            property.Value = data;
        }

        // 체력 비율
        public float GetHpRatio(int id) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return 0f;

            var data = property.Value;
            return data.maxHp == MIN_HP ? 0f : (float)data.currentHp / data.maxHp;
        }

        // 상태 조회 메서드들
        public bool IsDead(int id) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return false;
            return property.Value.isDead;
        }

        public int GetCurrentHp(int id) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.currentHp;
        }

        public int GetMaxHp(int id) {
            if (!_healthDataDict.TryGetValue(id, out var property)) return 0;
            return property.Value.maxHp;
        }

        public HealthData GetHealthData(int id) {
            if (!_healthDataDict.TryGetValue(id, out var property))
                return new HealthData(0);
            return property.Value;
        }

        // 전체 데이터 관리
        public int GetHealthCount() {
            return _healthDataDict.Count;
        }

        public IEnumerable<int> GetAllHealthIDs() {
            return _healthDataDict.Keys;
        }

        // 사망 체크
        private void CheckDead(ref HealthData data) {
            data.isDead = data.currentHp <= MIN_HP;
        }

        // 전체 초기화
        public void Initialize() {
            foreach (var property in _healthDataDict.Values) {
                property.Dispose();
            }
            _healthDataDict.Clear();
        }

        // 리소스 정리
        public void Dispose() {
            if (_disposed) return;
            _disposed = true;

            foreach (var property in _healthDataDict.Values) {
                property?.Dispose();
            }
            _healthDataDict.Clear();
        }
    }
}