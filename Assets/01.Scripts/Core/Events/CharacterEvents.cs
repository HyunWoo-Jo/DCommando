using UnityEngine;

namespace Game.Core.Event
{
    #region 캐릭터 생명주기 이벤트
    /// <summary>
    /// 캐릭터 등록 이벤트 (등록 후 발생)
    /// </summary>
    public readonly struct CharacterRegisteredEvent {
        public readonly int characterID;
        public readonly int maxHp;

        public CharacterRegisteredEvent(int characterID, int maxHp) {
            this.characterID = characterID;
            this.maxHp = maxHp;
        }
    }

    /// <summary>
    /// 캐릭터 제거 이벤트
    /// </summary>
    public readonly struct CharacterUnregisteredEvent {
        public readonly int characterID;

        public CharacterUnregisteredEvent(int characterID) {
            this.characterID = characterID;
        }
    }
    #endregion

    #region 데미지 관련 이벤트
    /// <summary>
    /// 데미지 받음 이벤트
    /// </summary>
    public readonly struct DamageTakenEvent {
        public readonly int characterID;
        public readonly int damage;
        public readonly int currentHp;

        public DamageTakenEvent(int characterID, int damage, int currentHp) {
            this.characterID = characterID;
            this.damage = damage;
            this.currentHp = currentHp;
        }
    }

    /// <summary>
    /// 캐릭터 사망 이벤트
    /// </summary>
    public readonly struct CharacterDeathEvent {
        public readonly int characterID;

        public CharacterDeathEvent(int characterID) {
            this.characterID = characterID;
        }
    }
    #endregion

    #region 치료 관련 이벤트  
    /// <summary>
    /// 치료 받음 이벤트
    /// </summary>
    public readonly struct HealedEvent {
        public readonly int characterID;
        public readonly int healAmount;
        public readonly int currentHp;

        public HealedEvent(int characterID, int healAmount, int currentHp) {
            this.characterID = characterID;
            this.healAmount = healAmount;
            this.currentHp = currentHp;
        }
    }

    /// <summary>
    /// 부활 이벤트
    /// </summary>
    public readonly struct RevivedEvent {
        public readonly int characterID;
        public readonly int reviveHp;

        public RevivedEvent(int characterID, int reviveHp) {
            this.characterID = characterID;
            this.reviveHp = reviveHp;
        }
    }
    #endregion

    #region 체력 설정 이벤트
    /// <summary>
    /// 최대 체력 변경 이벤트
    /// </summary>
    public readonly struct MaxHpChangedEvent {
        public readonly int characterID;
        public readonly int newMaxHp;
        public readonly int prevMaxHp;

        public MaxHpChangedEvent(int characterID, int newMaxHp, int prevMaxHp) {
            this.characterID = characterID;
            this.newMaxHp = newMaxHp;
            this.prevMaxHp = prevMaxHp;
        }
    }

    /// <summary>
    /// 현재 체력 설정 이벤트
    /// </summary>
    public readonly struct HpSetEvent {
        public readonly int characterID;
        public readonly int newHp;
        public readonly int prevHp;

        public HpSetEvent(int characterID, int newHp, int prevHp) {
            this.characterID = characterID;
            this.newHp = newHp;
            this.prevHp = prevHp;
        }
    }
    #endregion
}
