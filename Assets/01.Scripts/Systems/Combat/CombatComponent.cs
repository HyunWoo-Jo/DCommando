using Game.Models;
using UnityEngine;
using Zenject;

namespace Game.Systems
{
    /// <summary>
    /// 전투를 하기 위한 기본 스텟(공격력, 방어력)을 등록 하는 컴포넌트
    /// </summary>
    public class CombatComponent : MonoBehaviour {
        [SerializeField] private CombatData _combatData;

        [Inject] private CombatSystem _combatSystem;

        public void OnHitLayer(int targetId) {
            _combatSystem.ProcessAttack(gameObject.GetInstanceID(), targetId);
        }


        private void OnDestroy() {
            // 해제
            _combatSystem.UnregisterCombatCharacter(gameObject.GetInstanceID());
        }

        private void Start() {
            // 등록
            _combatSystem.RegisterCombatCharacter(gameObject.GetInstanceID(), _combatData.FinalAttack, _combatData.FinalDefense);
        }
    }
}
