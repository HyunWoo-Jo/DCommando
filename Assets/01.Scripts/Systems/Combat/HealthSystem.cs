using UnityEngine;
using Zenject;
using Game.Models;
using Game.Core;
using Game.Core.Event;
using System;
namespace Game.Systems {
    public class HealthSystem  {
        [Inject] private HealthModel _healthModel;

        // 상수
        private const int MIN_DAMAGE = 0; // 최소 데미지
        private const int MIN_HEAL = 1; // 최소 힐량
        private const int MIN_HP = 1; // 최소 등록 HP


        #region 캐릭터 관리
        /// <summary>
        /// 캐릭터 등록
        /// </summary>
        public bool RegisterCharacter(int characterID, int maxHp = 100) {
            if (_healthModel.HasHealth(characterID)) {
                GameDebug.LogWarning($"Character {characterID} already registered");
                return false;
            }

            if (maxHp < MIN_HP) {
                GameDebug.LogError($"MaxHp must be at least {MIN_HP}");
                return false;
            }
            // 모델에 추가
            _healthModel.AddHealth(characterID, maxHp);
            GameDebug.Log($"Character {characterID} registered with {maxHp} HP");

            // 이벤트 발행
            EventBus.Publish(new CharacterRegisteredEvent(characterID, maxHp));
            return true;
        }

        /// <summary>
        /// 캐릭터 제거
        /// </summary>
        public void UnregisterCharacter(int characterID) {
            if (!_healthModel.HasHealth(characterID)) {
                GameDebug.LogWarning($"Character {characterID} not found for unregistration");
                return;
            }

            _healthModel.RemoveHealth(characterID);
            GameDebug.Log($"Character {characterID} unregistered");

            // 이벤트 발행
            EventBus.Publish(new CharacterUnregisteredEvent(characterID));
        }

        /// <summary>
        /// 캐릭터 존재 확인
        /// </summary>
        public bool HasCharacter(int characterID) {
            return _healthModel.HasHealth(characterID);
        }
        #endregion

        #region 데미지 처리
        /// <summary>
        /// 데미지 처리
        /// </summary>
        public bool TakeDamage(int characterID, int damage) {
            // 유효성 검사
            if (!ValidateCharacterExists(characterID)) return false;
            if (!ValidateDamageAmount(damage)) return false;
            if (!CanTakeDamage(characterID)) return false;

            var prevHp = _healthModel.GetCurrentHp(characterID);
            _healthModel.TakeDamage(characterID, damage);
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var actualDamage = prevHp - currentHp;

            GameDebug.Log($"Character {characterID} took {actualDamage} damage ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new DamageTakenEvent(characterID, actualDamage, currentHp));

            // 사망 체크
            if (_healthModel.IsDead(characterID)) {
                OnCharacterDeath(characterID);
            }

            return true;
        }

        /// <summary>
        /// 데미지 가능 여부 확인
        /// </summary>
        public bool CanTakeDamage(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            return !_healthModel.IsDead(characterID);
        }

        /// <summary>
        /// 즉사 처리
        /// </summary>
        public bool InstantKill(int characterID) {
            if (!ValidateCharacterExists(characterID)) return false;
            if (!CanTakeDamage(characterID)) return false;

            var currentHp = _healthModel.GetCurrentHp(characterID);
            return TakeDamage(characterID, currentHp);
        }
        #endregion

        #region 치료 처리
        /// <summary>
        /// 치료 처리
        /// </summary>
        public bool Heal(int characterID, int healAmount) {
            // 유효성 검사
            if (!ValidateCharacterExists(characterID)) return false;
            if (!ValidateHealAmount(healAmount)) return false;
            if (!CanHeal(characterID)) return false;

            var prevHp = _healthModel.GetCurrentHp(characterID);
            _healthModel.Heal(characterID, healAmount);
            var currentHp = _healthModel.GetCurrentHp(characterID);
            var actualHeal = currentHp - prevHp;

            GameDebug.Log($"Character {characterID} healed {actualHeal} HP ({prevHp} → {currentHp})");

            // 이벤트 발행
            EventBus.Publish(new HealedEvent(characterID, actualHeal, currentHp));
            return true;
        }

        /// <summary>
        /// 치료 가능 여부 확인
        /// </summary>
        public bool CanHeal(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            if (_healthModel.IsDead(characterID)) return false;

            var currentHp = _healthModel.GetCurrentHp(characterID);
            var maxHp = _healthModel.GetMaxHp(characterID);
            return currentHp < maxHp;
        }

        /// <summary>
        /// 완전 회복
        /// </summary>
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
        /// <summary>
        /// 부활 처리
        /// </summary>
        public bool Revive(int characterID, int reviveHp = -1) {
            if (!ValidateCharacterExists(characterID)) return false;
            if (!CanRevive(characterID)) return false;

            var maxHp = _healthModel.GetMaxHp(characterID);
            var finalReviveHp = reviveHp == -1 ? maxHp : reviveHp;

            if (finalReviveHp < MIN_HP || finalReviveHp > maxHp) {
                GameDebug.LogError($"Invalid revive HP: {finalReviveHp} (must be between {MIN_HP} and {maxHp})");
                return false;
            }

            _healthModel.Revive(characterID, finalReviveHp);
            GameDebug.Log($"Character {characterID} revived with {finalReviveHp} HP");

            // 이벤트 발행
            EventBus.Publish(new RevivedEvent(characterID, finalReviveHp));
            return true;
        }

        /// <summary>
        /// 부활 가능 여부 확인
        /// </summary>
        public bool CanRevive(int characterID) {
            if (!_healthModel.HasHealth(characterID)) return false;
            return _healthModel.IsDead(characterID);
        }
        #endregion

        #region 체력 설정
        /// <summary>
        /// 최대 체력 설정
        /// </summary>
        public bool SetMaxHp(int characterID, int maxHp) {
            if (!ValidateCharacterExists(characterID)) return false;

            if (maxHp < MIN_HP) {
                GameDebug.LogError($"MaxHp must be at least {MIN_HP}");
                return false;
            }

            var prevMaxHp = _healthModel.GetMaxHp(characterID);
            _healthModel.SetMaxHp(characterID, maxHp);

            GameDebug.Log($"Character {characterID} max HP changed ({prevMaxHp} → {maxHp})");

            // 이벤트 발행
            EventBus.Publish(new MaxHpChangedEvent(characterID, maxHp, prevMaxHp));
            return true;
        }

        /// <summary>
        /// 현재 체력 설정
        /// </summary>
        public bool SetCurrentHp(int characterID, int hp) {
            if (!ValidateCharacterExists(characterID)) return false;

            if (hp < 0) {
                GameDebug.LogError("HP cannot be negative");
                return false;
            }

            var prevHp = _healthModel.GetCurrentHp(characterID);
            _healthModel.SetCurrentHp(characterID, hp);
            var currentHp = _healthModel.GetCurrentHp(characterID);

            GameDebug.Log($"Character {characterID} HP set ({prevHp} → {currentHp})");

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
        /// <summary>
        /// 사망 상태 확인
        /// </summary>
        public bool IsDead(int characterID) => _healthModel?.IsDead(characterID) ?? false;

        /// <summary>
        /// 체력 비율 조회
        /// </summary>
        public float GetHpRatio(int characterID) => _healthModel?.GetHpRatio(characterID) ?? 0f;

        /// <summary>
        /// 현재 체력 조회
        /// </summary>
        public int GetCurrentHp(int characterID) => _healthModel?.GetCurrentHp(characterID) ?? 0;

        /// <summary>
        /// 최대 체력 조회
        /// </summary>
        public int GetMaxHp(int characterID) => _healthModel?.GetMaxHp(characterID) ?? 0;

        /// <summary>
        /// 전체 캐릭터 수 조회
        /// </summary>
        public int GetCharacterCount() => _healthModel?.GetHealthCount() ?? 0;
        #endregion

        #region 내부 메서드
        /// <summary>
        /// 캐릭터 사망 처리
        /// </summary>
        private void OnCharacterDeath(int characterID) {
            GameDebug.Log($"Character {characterID} has died");
            EventBus.Publish(new CharacterDeathEvent(characterID));
        }

        /// <summary>
        /// 캐릭터 존재 검증
        /// </summary>
        private bool ValidateCharacterExists(int characterID) {
            if (!_healthModel.HasHealth(characterID)) {
                GameDebug.LogError($"Character {characterID} not found");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 데미지 양 검증
        /// </summary>
        private bool ValidateDamageAmount(int damage) {
            if (damage < MIN_DAMAGE) {
                GameDebug.LogError($"Damage must be at least {MIN_DAMAGE}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 치료 양 검증
        /// </summary>
        private bool ValidateHealAmount(int healAmount) {
            if (healAmount < MIN_HEAL) {
                GameDebug.LogError($"Heal amount must be at least {MIN_HEAL}");
                return false;
            }
            return true;
        }
        #endregion
    }
}