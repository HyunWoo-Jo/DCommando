using UnityEngine;
using System;
using Game.Core;
namespace Game.Systems {
    /// <summary>
    /// 지연 노드
    /// </summary>
    [Serializable]
    public class DelayAction : ActionNodeBase {
        [SerializeField] private float _targetDelay = 0;

        [SerializeField] private float _curTime = 0;

        public float TargetDelay => _targetDelay;

        public DelayAction(float targetDelay) {
            _targetDelay = targetDelay;
        }

        public void SetTargetDelay(float delay) {
            _targetDelay = delay;
        }

        public override void Reset() {
            base.Reset();
            _curTime = 0;
        }

        public override NodeState Evaluate(int id) {
            if(_curTime < _targetDelay) _curTime += Time.deltaTime;
            if (_targetDelay <= _curTime) {
                _nodeState = NodeState.Success;
                return NodeState.Success;
            }
            _nodeState = NodeState.Running;
            return NodeState.Running;
        }
        public override BehaviourNodeBase DeepCopy() {
            return new DelayAction(_targetDelay);
        }

    }
}