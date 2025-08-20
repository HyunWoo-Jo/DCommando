using Game.Core;
using Game.Core.Event;
using System;
using UnityEngine;

namespace Game.Systems {
    /// <summary>
    /// 데미지 처리 요청 
    /// </summary>
    [Serializable]
    public class DamageToPlayerAction : ActionNodeBase {
        [SerializeField] private DamageType _damageType;

        public DamageType DamageType => _damageType;

        public void SetDamageType(DamageType damageType) {
            _damageType = damageType;
        }

        public override NodeState Evaluate(int id) {
            GameObject playerObj = AIDataProvider.GetPlayer();
            Vector3 ownerPos = AIDataProvider.GetEnemyPosition(id);
            EventBus.Publish(new DamageRequestEvent(id, playerObj.GetInstanceID(), _damageType)); // 데미지 처리요청
            _nodeState = NodeState.Success;
            return NodeState.Success;
        }

        public override BehaviourNodeBase DeepCopy() {
            return this;
        }
    }
}
