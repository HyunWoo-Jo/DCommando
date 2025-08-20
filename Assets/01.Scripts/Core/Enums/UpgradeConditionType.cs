using UnityEngine;

namespace Game.Core {
    /// <summary>
    /// 업그레이드 조건
    /// </summary>
    public enum UpgradeConditionType {
        PlayerLevel,    // 레벨
        StageCleared,   // 스테이지 단계
        TotalGold,      // 골드
        UpgradeOwned,   // 업그레이드
        ItemOwned,      // 아이템
    }
}
