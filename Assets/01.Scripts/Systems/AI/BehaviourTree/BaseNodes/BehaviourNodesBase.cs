using UnityEngine;
using Game.Core;
using System.Collections.Generic;
using System;
namespace Game.Systems {
    /// <summary>
    /// 노드
    /// </summary>
    [Serializable]
    public abstract class BehaviourNodeBase {
        [SerializeReference]
        protected BehaviourNodeBase _parentNode;
        protected NodeState _nodeState;
        public NodeState NodeState => _nodeState;

        public BehaviourNodeBase ParentNode => _parentNode;
        public void SetParentNode(BehaviourNodeBase node) {
            _parentNode = node;
        }

        public abstract NodeState Evaluate(int id);

        public virtual void Reset() {
            _nodeState = NodeState.Running;
        }

        public abstract BehaviourNodeBase DeepCopy();
    }

    [Serializable]
    public abstract class CompositeNodeBase : BehaviourNodeBase {
        [SerializeReference]
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
    [Serializable]
    public abstract class DecoratorNodeBase : BehaviourNodeBase {
        [SerializeReference]
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
    [Serializable]
    public abstract class ActionNodeBase : BehaviourNodeBase {
    }
    /// <summary>
    /// 조건을 확인하는 노드
    /// </summary>
    [Serializable]
    public abstract class ConditionNodeBase : BehaviourNodeBase {
        public abstract bool CheckCondition();

        public override NodeState Evaluate(int id) {
            return CheckCondition() ? NodeState.Success : NodeState.Failure;
        }
    }
}
