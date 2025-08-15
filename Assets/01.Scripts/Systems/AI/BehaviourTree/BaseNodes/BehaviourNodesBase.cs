using UnityEngine;
using Game.Core;
using System.Collections.Generic;
namespace Game.Systems {
    /// <summary>
    /// 노드
    /// </summary>
    public abstract class BehaviourNodeBase {
        protected NodeState _nodeState;
        public NodeState NodeState => _nodeState;

        public abstract NodeState Evaluate();

        public virtual void Reset() {
            _nodeState = NodeState.Running;
        }
    }


    public abstract class CompositeNodeBase : BehaviourNodeBase {
        protected List<BehaviourNodeBase> _children = new List<BehaviourNodeBase>();

        public void AddChild(BehaviourNodeBase child) {
            _children.Add(child);
        }

        public void RemoveChild(BehaviourNodeBase child) {
            _children.Remove(child);
        }

        public override void Reset() {
            base.Reset();
            foreach (var child in _children) {
                child.Reset();
            }
        }
    }

    public abstract class DecoratorNodeBase : BehaviourNodeBase {
        protected BehaviourNodeBase _child;

        public void SetChild(BehaviourNodeBase child) {
            _child = child;
        }

        public override void Reset() {
            base.Reset();
            _child?.Reset();
        }
    }

    /// <summary>
    /// 실제 행동을 수행 하는 노드
    /// </summary>
    public abstract class ActionNodeBase : BehaviourNodeBase {
    }
    /// <summary>
    /// 조건을 확인하는 노드
    /// </summary>
    public abstract class ConditionNodeBase : BehaviourNodeBase {
        public abstract bool CheckCondition();

        public override NodeState Evaluate() {
            return CheckCondition() ? NodeState.Success : NodeState.Failure;
        }
    }
}
