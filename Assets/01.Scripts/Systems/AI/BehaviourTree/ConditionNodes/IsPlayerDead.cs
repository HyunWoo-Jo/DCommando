using UnityEngine;
using Game.Core;
namespace Game.Systems
{
    public class IsPlayerDead : ConditionNodeBase
    {
        public override bool CheckCondition() {
            return AIDataProvider.GetPlayerIsDie();
        }


        public override NodeState Evaluate(int id) {
            _nodeState = base.Evaluate(id);
            return _nodeState;
        }

        public override BehaviourNodeBase DeepCopy() {
            return this;
        }
    }
}
