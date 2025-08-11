using UnityEngine;

namespace Game.Models
{
    /// <summary>
    /// 전투 데이터 구조체
    /// </summary>
    [System.Serializable]
    public struct CombatData {
        [Header("공격력")]
        public int baseAttack;          // 기본 공격력
        public int bonusAttack;         // 추가 공격력 (장비, 버프 등)
        public float attackMultiplier;  // 공격력 배율

        [Header("방어력")]
        public int baseDefense;         // 기본 방어력
        public int bonusDefense;        // 추가 방어력 (장비, 버프 등)
        public float defenseMultiplier; // 방어력 배율

        [Header("공격 속도")]
        public float baseAttackSpeed;
        public float bonusAttackSpeed;
        public float attackSpeedMultiplier;
        public CombatData(int baseAttack, int baseDefense, float baseAttackSpeed) {
            this.baseAttack = baseAttack;
            this.baseDefense = baseDefense;
            this.bonusAttack = 0;
            this.bonusDefense = 0;
            this.attackMultiplier = 1.0f;
            this.defenseMultiplier = 1.0f;
            this.baseAttackSpeed = 0f;
            this.bonusAttackSpeed = 0f;
            this.attackSpeedMultiplier = baseAttackSpeed;
        }

        // 최종 공격력 계산
        public int FinalAttack => Mathf.RoundToInt((baseAttack + bonusAttack) * attackMultiplier);

        // 최종 방어력 계산
        public int FinalDefense => Mathf.RoundToInt((baseDefense + bonusDefense) * defenseMultiplier);

        // 최종 공격 속도 계산
        public float FinalAttackSpeed => Mathf.Max(0, (baseAttackSpeed + bonusAttackSpeed) * attackSpeedMultiplier);
    }
}
