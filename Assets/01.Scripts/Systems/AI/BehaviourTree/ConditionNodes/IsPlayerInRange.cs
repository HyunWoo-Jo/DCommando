using UnityEngine;
using System;
using Game.Core;
namespace Game.Systems {
    /// <summary>
    /// 플레이어가 범위 내에 존재하나 체크
    /// </summary>
    [Serializable]
    public class IsPlayerInRange : ConditionNodeBase {
        [SerializeField]
        private float _range = 2f;
        private int _id;
        
        public float Range => _range;

        public IsPlayerInRange(float range) {
            _range = range;
        }

        public void SetRange(float range) {
            _range = range;
        }

        public override NodeState Evaluate(int id) {
            _id = id;
            _nodeState = base.Evaluate(id);
            return _nodeState;
        }

        public override bool CheckCondition() {
            Vector3 playerPos = AIDataProvider.GetPlayerPosition();
            Vector3 ownerPos = AIDataProvider.GetEnemyPosition(_id);
            if (Vector3.Distance(ownerPos, playerPos) < _range) {
                return true;
            }
            return false;
        }

        public override BehaviourNodeBase DeepCopy() {
            return this;
        }
    }
}
