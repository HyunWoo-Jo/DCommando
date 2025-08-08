using R3;
using UnityEngine;
using Zenject;
using System;
using Game.Models;
using Game.Systems;
namespace Game.ViewModels {
    public class HealthViewModel : IInitializable, IDisposable {
        [Inject] private HealthModel _healthModel;
        [Inject] private HealthSystem _healthSystem;
        // [Inject] private SO_HealthStyle _healthStyle; // 필요시 추가

        private CompositeDisposable _disposables = new();

        // 알림용 Subject
        public readonly Subject<string> OnHealthNotification = new();

        #region Zenject에서 관리
        public void Initialize() {
            // 초기화 로직
        }

        /// <summary>
        /// Zenject에서 관리
        /// </summary>
        public void Dispose() {
            OnHealthNotification?.Dispose();
            _disposables?.Dispose();
            _disposables = null;
        }
        #endregion
        /// <summary>
        /// 특정 캐릭터의 현재 체력
        /// </summary>
        public ReadOnlyReactiveProperty<int> GetCurrentHpProperty(int characterID) {
            var healthProperty = _healthModel?.GetHealthProperty(characterID);
            if (healthProperty == null) return null;

            return healthProperty
                .Select(data => data.currentHp)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }

        /// <summary>
        /// 특정 캐릭터의 최대 체력
        /// </summary>
        public ReadOnlyReactiveProperty<int> GetMaxHpProperty(int characterID) {
            var healthProperty = _healthModel?.GetHealthProperty(characterID);
            if (healthProperty == null) return null;

            return healthProperty
                .Select(data => data.maxHp)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }

        /// <summary>
        /// 특정 캐릭터의 사망 상태
        /// </summary>
        public ReadOnlyReactiveProperty<bool> GetIsDeadProperty(int characterID) {
            var healthProperty = _healthModel?.GetHealthProperty(characterID);
            if (healthProperty == null) return null;

            return healthProperty
                .Select(data => data.isDead)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }

        /// <summary>
        /// 특정 캐릭터의 체력 텍스트 (내부 변환 데이터)
        /// </summary>
        public ReadOnlyReactiveProperty<string> GetHealthDisplayTextProperty(int characterID) {
            var healthProperty = _healthModel?.GetHealthProperty(characterID);
            if (healthProperty == null) return null;

            return healthProperty
                .Select(data => FormatHealthText(data.currentHp, data.maxHp))
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }

        /// <summary>
        /// 특정 캐릭터의 체력 비율 (내부 변환 데이터)
        /// </summary>
        public ReadOnlyReactiveProperty<float> GetHealthRatioProperty(int characterID) {
            var healthProperty = _healthModel?.GetHealthProperty(characterID);
            if (healthProperty == null) return null;

            return healthProperty
                .Select(data => CalculateHealthRatio(data.currentHp, data.maxHp))
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }

        /// <summary>
        /// 특정 캐릭터의 저체력 상태 (내부 변환 데이터)
        /// </summary>
        public ReadOnlyReactiveProperty<bool> GetLowHealthProperty(int characterID) {
            var healthProperty = _healthModel?.GetHealthProperty(characterID);
            if (healthProperty == null) return null;

            return healthProperty
                .Select(data => CalculateIsLowHealth(data.currentHp, data.maxHp, data.isDead))
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }



        /// <summary>
        /// 체력 텍스트 포맷팅
        /// </summary>
        public string FormatHealthText(int currentHp, int maxHp) {
            return $"{currentHp}/{maxHp}";
        }

        /// <summary>
        /// 체력 비율 계산
        /// </summary>
        public float CalculateHealthRatio(int currentHp, int maxHp) {
            return maxHp <= 0 ? 0f : (float)currentHp / maxHp;
        }

        /// <summary>
        /// 저체력 상태 계산
        /// </summary>
        public bool CalculateIsLowHealth(int currentHp, int maxHp, bool isDead) {
            if (isDead || currentHp <= 0 || maxHp <= 0) return false;
            float ratio = (float)currentHp / maxHp;
            return ratio <= 0.3f; // LOW_HEALTH_THRESHOLD
        }


        /// <summary>
        /// 캐릭터 등록
        /// </summary>
        public bool RegisterCharacter(int characterID, int maxHp = 100) {
            var result = _healthSystem?.RegisterCharacter(characterID, maxHp) ?? false;
            if (result) {
                OnHealthNotification.OnNext($"Character {characterID} registered with {maxHp} HP");
            }
            return result;
        }

        /// <summary>
        /// 캐릭터 제거
        /// </summary>
        public void UnregisterCharacter(int characterID) {
            _healthSystem?.UnregisterCharacter(characterID);
            OnHealthNotification.OnNext($"Character {characterID} unregistered");
        }

        /// <summary>
        /// 데미지 처리
        /// </summary>
        public bool TakeDamage(int characterID, int damage) {
            var result = _healthSystem?.TakeDamage(characterID, damage, Core.DamageType.Physical) ?? false;
            if (result) {
                OnHealthNotification.OnNext($"Character {characterID} took {damage} damage");
            }
            return result;
        }

        /// <summary>
        /// 치료 처리
        /// </summary>
        public bool Heal(int characterID, int healAmount) {
            var result = _healthSystem?.Heal(characterID, healAmount) ?? false;
            if (result) {
                OnHealthNotification.OnNext($"Character {characterID} healed {healAmount} HP");
            }
            return result;
        }

        /// <summary>
        /// 부활 처리
        /// </summary>
        public bool Revive(int characterID, int reviveHp = -1) {
            var result = _healthSystem?.Revive(characterID, reviveHp) ?? false;
            if (result) {
                OnHealthNotification.OnNext($"Character {characterID} revived");
            }
            return result;
        }

        /// <summary>
        /// 데미지 가능 여부 확인
        /// </summary>
        public bool CanTakeDamage(int characterID) {
            return _healthSystem?.CanTakeDamage(characterID) ?? false;
        }

        /// <summary>
        /// 치료 가능 여부 확인
        /// </summary>
        public bool CanHeal(int characterID) {
            return _healthSystem?.CanHeal(characterID) ?? false;
        }

        /// <summary>
        /// 캐릭터 존재 확인
        /// </summary>
        public bool HasCharacter(int characterID) {
            return _healthSystem?.HasCharacter(characterID) ?? false;
        }

        /// <summary>
        /// 최대 체력 설정
        /// </summary>
        public void SetMaxHp(int characterID, int hp) {
            _healthSystem?.SetMaxHp(characterID, hp);
        }
    }
}