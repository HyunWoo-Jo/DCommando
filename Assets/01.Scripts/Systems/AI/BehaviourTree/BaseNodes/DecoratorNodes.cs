using Game.Core;
using UnityEngine;
using UnityEngine.XR;

namespace Game.Systems
{
    /// <summary>
    /// 결과 반전
    /// </summary>
    public class Inverter : DecoratorNodeBase {
        public override NodeState Evaluate() {
            switch (_child.Evaluate()) {
                case NodeState.Success:
                _nodeState = NodeState.Failure;
                break;
                case NodeState.Failure:
                _nodeState = NodeState.Success;
                break;
                case NodeState.Running:
                _nodeState = NodeState.Running;
                break;
            }

            return _nodeState;
        }
    }

    /// <summary>
    /// 반복 실행
    /// </summary>
    public class Repeater : DecoratorNodeBase {
        private int _repeatCount;
        private int _currentCount;

        public Repeater(int repeatCount = -1) // -1은 무한 반복
        {
            _repeatCount = repeatCount;
            _currentCount = 0;
        }

        public override NodeState Evaluate() {
            if (_repeatCount != -1 && _currentCount >= _repeatCount) {
                _nodeState = NodeState.Success;
                return _nodeState;
            }

            switch (_child.Evaluate()) {
                case NodeState.Success:
                case NodeState.Failure:
                _currentCount++;
                _child.Reset();

                if (_repeatCount != -1 && _currentCount >= _repeatCount) {
                    _nodeState = NodeState.Success;
                } else {
                    _nodeState = NodeState.Running;
                }
                break;
                case NodeState.Running:
                _nodeState = NodeState.Running;
                break;
            }

            return _nodeState;
        }

        public override void Reset() {
            base.Reset();
            _currentCount = 0;
        }
    }

    /// <summary>
    /// 특정 시간동안 제한
    /// </summary>
    public class Cooldown : DecoratorNodeBase {
        private float _cooldownTime;
        private float _lastExecuteTime;

        public Cooldown(float cooldownTime) {
            _cooldownTime = cooldownTime;
            _lastExecuteTime = -cooldownTime; // 첫 실행 허용
        }

        public override NodeState Evaluate() {
            float currentTime = Time.time;

            if (currentTime - _lastExecuteTime < _cooldownTime) {
                _nodeState = NodeState.Failure;
                return _nodeState;
            }

            var result = _child.Evaluate();

            if (result == NodeState.Success || result == NodeState.Failure) {
                _lastExecuteTime = currentTime;
            }

            _nodeState = result;
            return _nodeState;
        }
    }

    /// <summary>
    /// 항상 성공
    /// </summary>
    public class Succeeder : DecoratorNodeBase {
        public override NodeState Evaluate() {
            _child.Evaluate();
            _nodeState = NodeState.Success;
            return _nodeState;
        }
    }
}
