using UnityEngine;

namespace Game.Core
{
    public enum UpgradeType {
        None,

        Power,        // 공격력 증가
        MaxHealth,      // HP 증가
        Heal,           // HP 회복

        AttackSpeed,    // 공격 속도 증가
        Defense,        // 방어력
        AttackRange,    // 거리 범위 증가
        AttackWidth,    // 너비 범위 증가
        MoveSpeed,      // 이동 속도 증가
    }
}
