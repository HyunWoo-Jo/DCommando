using Game.Core;
using UnityEngine;
using UnityEngine.XR;
namespace Game.Systems
{
    /// <summary>
    /// 하나라도 성공하면 성공
    /// </summary>
    public class Selector : CompositeNodeBase {
        public override NodeState Evaluate() {
            foreach (var child in _children) {
                switch (child.Evaluate()) {
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
    }
    /// <summary>
    /// 모두 성공해야 성공
    /// </summary>
    public class Sequence : CompositeNodeBase {
        public override NodeState Evaluate() {
            bool anyChildRunning = false;

            foreach (var child in _children) {
                switch (child.Evaluate()) {
                    case NodeState.Failure:
                    _nodeState = NodeState.Failure;
                    return _nodeState;
                    case NodeState.Running:
                    anyChildRunning = true;
                    break;
                    case NodeState.Success:
                    break;
                }
            }

            _nodeState = anyChildRunning ? NodeState.Running : NodeState.Success;
            return _nodeState;
        }
    }

    /// <summary>
    /// 모두 동시 실행
    /// </summary>
    public class Parallel : CompositeNodeBase {
        private int _successCount;
        private int _failureCount;

        public override NodeState Evaluate() {
            _successCount = 0;
            _failureCount = 0;

            foreach (var child in _children) {
                switch (child.Evaluate()) {
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
    }
}
