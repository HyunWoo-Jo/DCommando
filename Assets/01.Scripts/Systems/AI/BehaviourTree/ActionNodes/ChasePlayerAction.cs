using Game.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game.Systems {
    /// <summary>
    /// 플레이어 추적 액션
    /// </summary>
    public class ChasePlayerAction : ActionNodeBase {
        public override NodeState Evaluate() {


            return NodeState.Failure;
        }
    }
}
