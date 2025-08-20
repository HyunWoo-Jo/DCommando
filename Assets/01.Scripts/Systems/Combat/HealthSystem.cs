using UnityEngine;
using Zenject;
using Game.Models;
using Game.Core;
using Game.Core.Event;
using System;
using R3;
using Game.Policies;

namespace Game.Systems {
    public class HealthSystem : IInitializable, IDisposable {
        [Inject] private HealthModel _healthModel;
        [Inject] private IHealthPolicy _healthPolicy;

        private CompositeDisposable _disposables = new();

        #region 초기화 Zenject 관리
        public void Initialize() {
            EventBus.Subscribe<StatChangeEvent>(OnUpgradeEvent).AddTo(_disposables);
        }

        public void Dispose() {
            _disposables?.Dispose();
        }

        /// <summary>
        /// Upgrade 처리
        /// </summary>
        private void OnUpgradeEvent(StatChangeEvent e) {
            if (e.upgradeType == UpgradeType.MaxHealth) {
                int id = AIDataProvider.GetPlayer().GetInstanceID();
                int value = (int)e.value;
                SetMaxHp(id, _healthModel.GetMaxHp(id) + value);
                Heal(id, value);
            } else if (e.upgradeType == UpgradeType.Heal) {
                int id = AIDataProvider.GetPlayer().GetInstanceID();
                int value = (int)e.value;
                Heal(id, value);
            }
        }

        #endregion

        #region 캐릭터 관리
        // 캐릭터 등록
        public bool RegisterCharacter(int characterID, int maxHp = 100) {
            if (_healthModel.HasHealth(characterID)) {
                GameDebug.LogWarning($"이미 등록된 캐릭터 Character {characterID}");
                return false;
            }

            if (!_healthPolicy.IsValidMaxHp(maxHp)) {
                GameDebug.LogError($"최대 체력이 최소값보다 적음 {_healthPolicy.GetMinimumHp()}");
                return false;
            }

            // 모델에 추가
            _healthModel.AddHealth(characterID, maxHp);
            GameDebug.Log($"캐릭터 등록 Character {characterID} with {maxHp} HP");

            // 이벤트 발행
            EventBus.Publish(new CharacterRegisteredEvent(characterID, maxHp));
            return true;
        }

        // 캐릭터 제거
        public void UnregisterCharacter(int characterID) {
            if (!_healthModel.HasHealth(characterID)) {
                GameDebug.LogWarning($"등록 해제할 캐릭터 찾을 수 없음 Character {characterID}");
                return;
            }

            _healthModel.RemoveHealth(characterID);
            GameDebug.Log($"캐릭터 등록 해제 Character {characterID}");

            // 이벤트 발행
            EventBus.Publish(new CharacterUnregisteredEvent(characterID));
        }

        // 캐릭터 존재 확인
        public bool HasCharacter(int characterID) {
            return _healthModel.HasHealth(characterID);
        }
        #endregion

        #region 데미지 처리
        // 데미지 처리
        public bool TakeDamage(int characterID, int damage, DamageType damageType) {
            // 유효성 검사
            if (!ValidateCharacterExists(characterID)) return false;
            if (!_healthPolicy.IsValidDamageAmount(damage)) {
                GameDebug.LogError($"데미지가 최소값보다 적음 {_healthPolicy.GetMinimumDamage()}");
                return false;
            }
            if (!CanTakeDamage(characterID)) return false;

            var prevHp = _healthModel.GetCurrentHp(characterID);

            // Policy를 통한 데미지 계산
            var newHp = _healthPolicy.CalculateHpAfterDamage(prevHp, damage);
            _healthModel.SetCurrentHp(characterID, newHp);

            var currentHp = _healthModel.GetCurrentHp(characterID);
            var actualDamage = _healthPolicy.CalculateActualDamage(prevHp, currentHp);

            GameDebug.Log($"캐릭터 데미지 Character {characterID} took {actualDamage} damage ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new DamageTakenEvent(characterID, damageType, actualDamage, currentHp));

            // 사망 체크
            if (_healthPolicy.IsDead(currentHp)) {
                OnCharacterDeath(characterID);
            }

            return true;
        }

        // 데미지 가능 여부 확인
        public bool CanTakeDamage(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            var currentHp = _healthModel.GetCurrentHp(characterID);
            return _healthPolicy.CanTakeDamage(currentHp);
        }

        // 즉사 처리
        public bool InstantKill(int characterID) {
            if (!ValidateCharacterExists(characterID)) return false;
            if (!CanTakeDamage(characterID)) return false;

            var currentHp = _healthModel.GetCurrentHp(characterID);
            var killDamage = _healthPolicy.CalculateInstantKillDamage(currentHp);
            return TakeDamage(characterID, killDamage, DamageType.Pure);
        }
        #endregion

        #region 치료 처리
        // 치료 처리
        public bool Heal(int characterID, int healAmount) {
            // 유효성 검사
            if (!ValidateCharacterExists(characterID)) return false;
            if (!_healthPolicy.IsValidHealAmount(healAmount)) {
                GameDebug.LogError($"치료량이 최소값보다 적음 {_healthPolicy.GetMinimumHeal()}");
                return false;
            }
            if (!CanHeal(characterID)) return false;

            var prevHp = _healthModel.GetCurrentHp(characterID);
            var maxHp = _healthModel.GetMaxHp(characterID);

            // Policy를 통한 치료 계산
            var newHp = _healthPolicy.CalculateHpAfterHeal(prevHp, healAmount, maxHp);
            _healthModel.SetCurrentHp(characterID, newHp);

            var currentHp = _healthModel.GetCurrentHp(characterID);
            var actualHeal = _healthPolicy.CalculateActualHeal(prevHp, currentHp);

            GameDebug.Log($"캐릭터 치료 Character {characterID} healed {actualHeal} HP ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new HealedEvent(characterID, actualHeal, currentHp));
            return true;
        }

        // 치료 가능 여부 확인
        public bool CanHeal(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var maxHp = _healthModel.GetMaxHp(characterID);
            return _healthPolicy.CanHeal(currentHp, maxHp);
        }

        // 완전 회복
        public bool FullHeal(int characterID) {
            if (!ValidateCharacterExists(characterID)) return false;

            var currentHp = _healthModel.GetCurrentHp(characterID);
            if (_healthPolicy.IsDead(currentHp)) return false;

            var maxHp = _healthModel.GetMaxHp(characterID);
            var healAmount = _healthPolicy.CalculateFullHealAmount(currentHp, maxHp);

            if (healAmount <= 0) return false; // 이미 풀피

            return Heal(characterID, healAmount);
        }
        #endregion

        #region 부활 처리
        // 부활 처리
        public bool Revive(int characterID, int reviveHp = -1) {
            if (!ValidateCharacterExists(characterID)) return false;
            if (!CanRevive(characterID)) return false;

            var maxHp = _healthModel.GetMaxHp(characterID);
            var finalReviveHp = reviveHp == -1 ? _healthPolicy.GetDefaultReviveHp(maxHp) : reviveHp;

            if (!_healthPolicy.IsValidReviveHp(finalReviveHp, maxHp)) {
                GameDebug.LogError($"부활 체력 값이 유효하지 않음 reviveHp: {finalReviveHp} (must be between {_healthPolicy.GetMinimumHp()} and {maxHp})");
                return false;
            }

            _healthModel.Revive(characterID, finalReviveHp);
            GameDebug.Log($"캐릭터 부활 Character {characterID} revived with {finalReviveHp} HP");

            // 이벤트 발행
            EventBus.Publish(new RevivedEvent(characterID, finalReviveHp));
            return true;
        }

        // 부활 가능 여부 확인
        public bool CanRevive(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            var currentHp = _healthModel.GetCurrentHp(characterID);
            return _healthPolicy.CanRevive(currentHp);
        }
        #endregion

        #region 체력 설정
        // 최대 체력 설정
        public bool SetMaxHp(int characterID, int maxHp) {
            if (!ValidateCharacterExists(characterID)) return false;

            if (!_healthPolicy.IsValidMaxHp(maxHp)) {
                GameDebug.LogError($"최대 체력이 최소값보다 적음 {_healthPolicy.GetMinimumHp()}");
                return false;
            }

            var prevMaxHp = _healthModel.GetMaxHp(characterID);
            _healthModel.SetMaxHp(characterID, maxHp);

            GameDebug.Log($"캐릭터 최대 체력 변경 Character {characterID} max HP changed ({prevMaxHp} → {maxHp})");

            // 이벤트 발행
            EventBus.Publish(new MaxHpChangedEvent(characterID, maxHp, prevMaxHp));
            return true;
        }

        // 현재 체력 설정
        public bool SetCurrentHp(int characterID, int hp) {
            if (!ValidateCharacterExists(characterID)) return false;

            if (!_healthPolicy.IsValidCurrentHp(hp)) {
                GameDebug.LogError("체력은 음수가 될 수 없음");
                return false;
            }

            var prevHp = _healthModel.GetCurrentHp(characterID);
            var maxHp = _healthModel.GetMaxHp(characterID);

            // Policy를 통한 체력 제한
            var clampedHp = _healthPolicy.ClampHp(hp, maxHp);
            _healthModel.SetCurrentHp(characterID, clampedHp);

            var currentHp = _healthModel.GetCurrentHp(characterID);

            GameDebug.Log($"캐릭터 체력 설정 Character {characterID} HP set ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new HpSetEvent(characterID, currentHp, prevHp));

            // 사망/부활 체크
            if (prevHp > 0 && _healthPolicy.IsDead(currentHp)) {
                OnCharacterDeath(characterID);
            } else if (_healthPolicy.IsDead(prevHp) && currentHp > 0) {
                EventBus.Publish(new RevivedEvent(characterID, currentHp));
            }

            return true;
        }
        #endregion

        #region 상태 조회
        // 사망 상태 확인
        public bool IsDead(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            var currentHp = _healthModel.GetCurrentHp(characterID);
            return _healthPolicy.IsDead(currentHp);
        }

        // 체력 비율 조회
        public float GetHpRatio(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return 0f;
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var maxHp = _healthModel.GetMaxHp(characterID);
            return _healthPolicy.CalculateHpRatio(currentHp, maxHp);
        }

        // 현재 체력 조회
        public int GetCurrentHp(int characterID) => _healthModel?.GetCurrentHp(characterID) ?? 0;

        // 최대 체력 조회
        public int GetMaxHp(int characterID) => _healthModel?.GetMaxHp(characterID) ?? 0;

        // 전체 캐릭터 수 조회
        public int GetCharacterCount() => _healthModel?.GetHealthCount() ?? 0;

        public ReadOnlyReactiveProperty<HealthData> GetRORP_HealthProperty(int id) => _healthModel.GetHealthProperty(id);
        #endregion

        #region 내부 메서드
        // 캐릭터 사망 처리
        private void OnCharacterDeath(int characterID) {
            GameDebug.Log($"캐릭터 사망 Character {characterID} has died");
            EventBus.Publish(new CharacterDeathEvent(characterID));
        }

        // 캐릭터 존재 검증
        private bool ValidateCharacterExists(int characterID) {
            if (!_healthModel.HasHealth(characterID)) {
                GameDebug.LogError($"캐릭터 찾을 수 없음 Character {characterID}");
                return false;
            }
            return true;
        }
        #endregion
    }
}