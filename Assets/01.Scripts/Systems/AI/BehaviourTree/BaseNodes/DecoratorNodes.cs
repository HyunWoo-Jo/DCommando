using Game.Core;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Game.Systems {
    /// <summary>
    /// 결과 반전
    /// </summary>
    [Serializable]
    public class Inverter : DecoratorNodeBase {
        public override NodeState Evaluate(int id) {
            switch (_child.Evaluate(id)) {
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

        public override BehaviourNodeBase DeepCopy() {
            var node = new Inverter();
            node._child = _child.DeepCopy();
            return node;
        }
    }

    /// <summary>
    /// 반복 실행
    /// </summary>
    [Serializable]
    public class Repeater : DecoratorNodeBase {
        [SerializeField] private int _repeatCount;
        [SerializeField] private int _currentCount;

        public Repeater(int repeatCount = -1) // -1은 무한 반복
        {
            _repeatCount = repeatCount;
            _currentCount = 0;
        }

        public override NodeState Evaluate(int id) {
            if (_repeatCount != -1 && _currentCount >= _repeatCount) {
                _nodeState = NodeState.Success;
                return _nodeState;
            }

            switch (_child.Evaluate(id)) {
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
        public override BehaviourNodeBase DeepCopy() {
            var node = new Repeater(_repeatCount);
            node._child = _child.DeepCopy();
            return node;
        }
    }

    /// <summary>
    /// 특정 시간동안 제한
    /// </summary>
    [Serializable]
    public class Cooldown : DecoratorNodeBase {
        [SerializeField] private float _cooldownTime;
        [SerializeField] private float _lastExecuteTime;

        public float CoolTime => _cooldownTime;

        public void SetCoolTime(float cooldownTime) {
            _cooldownTime = cooldownTime;
            _lastExecuteTime = -cooldownTime; // 첫 실행 허용
        }

        public Cooldown(float cooldownTime) {
            SetCoolTime(cooldownTime);
        }

        public override NodeState Evaluate(int id) {
            float currentTime = Time.time;

            if (currentTime - _lastExecuteTime < _cooldownTime) {
                _nodeState = NodeState.Failure;
                return _nodeState;
            }

            var result = _child.Evaluate(id);

            if (result == NodeState.Success || result == NodeState.Failure) {
                _lastExecuteTime = currentTime;
            }

            _nodeState = result;
            return _nodeState;
        }
        public override BehaviourNodeBase DeepCopy() {
            var node = new Cooldown(_cooldownTime);
            node._child = _child.DeepCopy();
            return node;
        }
    }



    /// <summary>
    /// 항상 성공
    /// </summary>
    [Serializable]
    public class Succeeder : DecoratorNodeBase {
        public override NodeState Evaluate(int id) {
            _child.Evaluate(id);
            _nodeState = NodeState.Success;
            return _nodeState;
        }
        public override BehaviourNodeBase DeepCopy() {
            var node = new Succeeder();
            node._child = _child.DeepCopy();
            return node;
        }
    }
}