using UnityEngine;
using Game.Core;
namespace Game.Data
{
    [CreateAssetMenu(fileName = "New_SkillData", menuName = "Game/Data/SkillData")]
    public class SO_SkillData : ScriptableObject {
        [Header("스킬 이름")]
        [SerializeField] private SkillName _skillName;

        [Header("스킬 범위 타입")]
        [SerializeField] private SkillRangeType _rangeType;

        [Header("스킬 범위")]
        [SerializeField] private float _range;

        [Header("부채꼴/사각형 추가 설정")]
        [SerializeField] private float _widthOrAngle; // 부채꼴 각도 또는 사각형 너비

        [Header("데미지 배율")]
        [SerializeField] private float _damageMultiplier;

        [Header("추가 데미지")]
        [SerializeField] private int _additionalDamage;

        // 프로퍼티
        public SkillName SkillName => _skillName;

        public SkillRangeType RangeType => _rangeType;
        public float Range => _range;
        public float Width => _widthOrAngle;

        public float Angle => _widthOrAngle;
        public float DamageMultiplier => _damageMultiplier;
        public int AdditionalDamage => _additionalDamage;
    }
}
