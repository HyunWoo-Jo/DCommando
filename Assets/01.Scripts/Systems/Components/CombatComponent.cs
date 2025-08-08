using Game.Models;
using UnityEngine;
using Zenject;
using Game.Core;
using static UnityEngine.GraphicsBuffer;
using System.Collections.Generic;
namespace Game.Systems
{
    /// <summary>
    /// 전투를 하기 위한 기본 스텟(공격력, 방어력)을 등록 하는 컴포넌트 + 데미지 처리
    /// </summary>
    public class CombatComponent : MonoBehaviour, IHitReceiver, IAnimAttackReceiver{
        [SerializeField] private CombatData _combatData;

        [Inject] private readonly CombatSystem _combatSystem;

        private bool _isAttackAble = false; // 공격 가능 상태
        private readonly HashSet<int> _targetHashs = new(); // 중복 처리를 방지하기 위한 HashSet

        #region 초기화
        private void OnDestroy() {
            // 해제
            _combatSystem.UnregisterCombatCharacter(gameObject.GetInstanceID());
        }

        private void Start() {
            // 등록
            _combatSystem.RegisterCombatCharacter(gameObject.GetInstanceID(), _combatData.FinalAttack, _combatData.FinalDefense);
        }
        #endregion

        /// <summary>
        /// 공격 가능 상태를 설정
        /// </summary>
        public void SetAttackEnabled(bool enabled) {
            _isAttackAble = enabled;
            if (!enabled) {
                _targetHashs.Clear();
            }
        }

        /// <summary>
        /// AttackComponent에서 레이어에 걸러진 오브젝트가 들어옴
        /// </summary>
        public void OnHit(GameObject hitObject, DamageType type) {
            if (!_isAttackAble) return;
            int id = hitObject.GetInstanceID();
            // 중복 적용 피하기위해 검사
            if (!_targetHashs.Contains(id)) {
                _combatSystem.ProcessAttack(gameObject.GetInstanceID(), id, type);

                _targetHashs.Add(id);
            } 
        }


  

        #region AnimSender에서 전달 받는 영역
        public void OnAttackStart() {
            SetAttackEnabled(true);
        }

        public void OnAttackEnd() {
            SetAttackEnabled(false);
        }

        #endregion
    }
}
