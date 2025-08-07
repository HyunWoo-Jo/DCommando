using UnityEngine;

namespace Game.Core
{
    public enum DamageType {
        Physical,   // 물리 데미지
        Magic,      // 마법 데미지
        Fire,       // 화염 데미지
        Ice,        // 얼음 데미지
        Poison,     // 독 데미지
        Lightning,  // 번개 데미지
        Pure,        // 순수 데미지 (방어 무시)
        Heal        // 체력 회복
    }

    /// <summary>
    /// Damage UI 타입 enum
    /// </summary>
    public enum DamageUIType {
        Normal,    // 일반 표시
        Critical,  // 크리티컬 (크게, 반짝임)
        Miss       // 빗나감
    }
}
