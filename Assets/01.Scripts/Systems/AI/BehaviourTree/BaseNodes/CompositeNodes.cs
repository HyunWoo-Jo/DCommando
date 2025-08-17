using Game.Core;
using System;
using UnityEngine;
using UnityEngine.XR;
namespace Game.Systems
{
    /// <summary>
    /// 하나라도 성공하면 성공
    /// </summary>
    [Serializable]
    public class Selector : CompositeNodeBase {
        public override NodeState Evaluate(int id) {
            foreach (var child in _children) {
                switch (child.Evaluate(id)) {
                    case NodeState.Success:
                    _nodeState = NodeState.Success;
                    return _nodeState;
                    case NodeState.Running:
                    _nodeState = NodeState.Running;
                    return _nodeState;
                    case NodeState.Failure:
                    continue;
                }
            }

            _nodeState = NodeState.Failure;
            return _nodeState;
        }
        public override BehaviourNodeBase DeepCopy() {
            var node = new Selector();
            foreach (var child in _children) {
                node._children.Add(child.DeepCopy());
            }
            return node;
        }

    }
    /// <summary>
    /// 모두 성공해야 성공
    /// </summary>
    [Serializable]
    public class Sequence : CompositeNodeBase {
        private bool _isCompleted = false;
        public override NodeState Evaluate(int id) {
            bool anyChildRunning = false;
            if (_isCompleted && _nodeState == NodeState.Success) {
                ResetAllChildren();
                _isCompleted = false;
            }
            foreach (var child in _children) {
                switch (child.Evaluate(id)) {
                    case NodeState.Failure:
                    _nodeState = NodeState.Failure;
                    return _nodeState;
                    case NodeState.Running:
                    anyChildRunning = true;
                    _nodeState = NodeState.Running;
                    return _nodeState;
                    case NodeState.Success:
                    break;
                }
            }
            // Success로 완료되었음을 표시
            if (_nodeState == NodeState.Success && !_isCompleted) {
                _isCompleted = true;
            }
            _nodeState = anyChildRunning ? NodeState.Running : NodeState.Success;
            return _nodeState;
        }
        public override BehaviourNodeBase DeepCopy() {
            var node = new Sequence();
            foreach (var child in _children) {

                node._children.Add(child.DeepCopy());
            }
            return node;
        }

        private void ResetAllChildren() {
            foreach (var child in _children) {
                child.Reset();
            }
        }

        public override void Reset() {
            _isCompleted = false;
            base.Reset();
        }

    }

    /// <summary>
    /// 모두 동시 실행
    /// </summary>
    [Serializable]
    public class Parallel : CompositeNodeBase {
        private int _successCount;
        private int _failureCount;

        public override NodeState Evaluate(int id) {
            _successCount = 0;
            _failureCount = 0;

            foreach (var child in _children) {
                switch (child.Evaluate(id)) {
                    case NodeState.Success:
                    _successCount++;
                    break;
                    case NodeState.Failure:
                    _failureCount++;
                    break;
                    case NodeState.Running:
                    break;
                }
            }

            // 모든 자식이 완료됨
            if (_successCount + _failureCount == _children.Count) {
                _nodeState = _successCount > _failureCount ? NodeState.Success : NodeState.Failure;
            } else {
                _nodeState = NodeState.Running;
            }

            return _nodeState;
        }
        public override BehaviourNodeBase DeepCopy() {
            var node = new Parallel();
            foreach (var child in _children) {
                node._children.Add(child.DeepCopy());
            }
            return node;
        }
    }
}
