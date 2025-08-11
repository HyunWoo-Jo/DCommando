using Game.Core;
using UnityEngine;

namespace Game.Systems
{
    public interface IWeapon
    {
        EquipName EquipName { get; }

        GameObject GameObj { get; }

        void SetEquipName(EquipName equipName);

        void PerformAttack();
        /// <summary>
        /// 무기 장착
        /// </summary>
        void Equip(GameObject owner); 

        /// <summary>
        /// 공격 지점 설정
        /// </summary>
        public void SetAttackPoint(Transform newAttackPoint);

        /// <summary>
        /// 무기 제거
        /// </summary>
        void Unequip(); 
    }
}
