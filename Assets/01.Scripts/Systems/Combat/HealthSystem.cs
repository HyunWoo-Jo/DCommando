using UnityEngine;
using Zenject;
using Game.Models;
using Game.Core;
using Game.Core.Event;
using System;

namespace Game.Systems {
    public class HealthSystem {
        [Inject] private HealthModel _healthModel;

        // 상수
        private const int MIN_DAMAGE = 0; // 최소 데미지
        private const int MIN_HEAL = 1; // 최소 힐량
        private const int MIN_HP = 1; // 최소 등록 HP

        #region 캐릭터 관리
        // 캐릭터 등록
        public bool RegisterCharacter(int characterID, int maxHp = 100) {
            if (_healthModel.HasHealth(characterID)) {
                GameDebug.LogWarning($"이미 등록된 캐릭터 Character {characterID}");
                return false;
            }

            if (maxHp < MIN_HP) {
                GameDebug.LogError($"최대 체력이 최소값보다 적음 {MIN_HP}");
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
        public bool TakeDamage(int characterID, int damage) {
            // 유효성 검사
            if (!ValidateCharacterExists(characterID)) return false;
            if (!ValidateDamageAmount(damage)) return false;
            if (!CanTakeDamage(characterID)) return false;

            var prevHp = _healthModel.GetCurrentHp(characterID);
            _healthModel.TakeDamage(characterID, damage);
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var actualDamage = prevHp - currentHp;

            GameDebug.Log($"캐릭터 데미지 Character {characterID} took {actualDamage} damage ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new DamageTakenEvent(characterID, DamageType.Physical, actualDamage, currentHp));

            // 사망 체크
            if (_healthModel.IsDead(characterID)) {
                OnCharacterDeath(characterID);
            }

            return true;
        }

        // 데미지 가능 여부 확인
        public bool CanTakeDamage(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            return !_healthModel.IsDead(characterID);
        }

        // 즉사 처리
        public bool InstantKill(int characterID) {
            if (!ValidateCharacterExists(characterID)) return false;
            if (!CanTakeDamage(characterID)) return false;

            var currentHp = _healthModel.GetCurrentHp(characterID);
            return TakeDamage(characterID, currentHp);
        }
        #endregion

        #region 치료 처리
        // 치료 처리
        public bool Heal(int characterID, int healAmount) {
            // 유효성 검사
            if (!ValidateCharacterExists(characterID)) return false;
            if (!ValidateHealAmount(healAmount)) return false;
            if (!CanHeal(characterID)) return false;

            var prevHp = _healthModel.GetCurrentHp(characterID);
            _healthModel.Heal(characterID, healAmount);
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var actualHeal = currentHp - prevHp;

            GameDebug.Log($"캐릭터 치료 Character {characterID} healed {actualHeal} HP ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new HealedEvent(characterID, actualHeal, currentHp));
            return true;
        }

        // 치료 가능 여부 확인
        public bool CanHeal(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            if (_healthModel.IsDead(characterID)) return false;

            var currentHp = _healthModel.GetCurrentHp(characterID);
            var maxHp = _healthModel.GetMaxHp(characterID);
            return currentHp < maxHp;
        }

        // 완전 회복
        public bool FullHeal(int characterID) {
            if (!ValidateCharacterExists(characterID)) return false;
            if (_healthModel.IsDead(characterID)) return false;

            var maxHp = _healthModel.GetMaxHp(characterID);
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var healAmount = maxHp - currentHp;

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
            var finalReviveHp = reviveHp == -1 ? maxHp : reviveHp;

            if (finalReviveHp < MIN_HP || finalReviveHp > maxHp) {
                GameDebug.LogError($"부활 체력 값이 유효하지 않음 reviveHp: {finalReviveHp} (must be between {MIN_HP} and {maxHp})");
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
            return _healthModel.IsDead(characterID);
        }
        #endregion

        #region 체력 설정
        // 최대 체력 설정
        public bool SetMaxHp(int characterID, int maxHp) {
            if (!ValidateCharacterExists(characterID)) return false;

            if (maxHp < MIN_HP) {
                GameDebug.LogError($"최대 체력이 최소값보다 적음 {MIN_HP}");
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

            if (hp < 0) {
                GameDebug.LogError("체력은 음수가 될 수 없음");
                return false;
            }

            var prevHp = _healthModel.GetCurrentHp(characterID);
            _healthModel.SetCurrentHp(characterID, hp);
            var currentHp = _healthModel.GetCurrentHp(characterID);

            GameDebug.Log($"캐릭터 체력 설정 Character {characterID} HP set ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new HpSetEvent(characterID, currentHp, prevHp));

            // 사망/부활 체크
            if (prevHp > 0 && currentHp <= 0) {
                OnCharacterDeath(characterID);
            } else if (prevHp <= 0 && currentHp > 0) {
                EventBus.Publish(new RevivedEvent(characterID, currentHp));
            }

            return true;
        }
        #endregion

        #region 상태 조회
        // 사망 상태 확인
        public bool IsDead(int characterID) => _healthModel?.IsDead(characterID) ?? false;

        // 체력 비율 조회
        public float GetHpRatio(int characterID) => _healthModel?.GetHpRatio(characterID) ?? 0f;

        // 현재 체력 조회
        public int GetCurrentHp(int characterID) => _healthModel?.GetCurrentHp(characterID) ?? 0;

        // 최대 체력 조회
        public int GetMaxHp(int characterID) => _healthModel?.GetMaxHp(characterID) ?? 0;

        // 전체 캐릭터 수 조회
        public int GetCharacterCount() => _healthModel?.GetHealthCount() ?? 0;
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

        // 데미지 양 검증
        private bool ValidateDamageAmount(int damage) {
            if (damage < MIN_DAMAGE) {
                GameDebug.LogError($"데미지가 최소값보다 적음 {MIN_DAMAGE}");
                return false;
            }
            return true;
        }

        // 치료 양 검증
        private bool ValidateHealAmount(int healAmount) {
            if (healAmount < MIN_HEAL) {
                GameDebug.LogError($"치료량이 최소값보다 적음 {MIN_HEAL}");
                return false;
            }
            return true;
        }
        #endregion
    }
}