using Game.Core;
using UnityEngine;

namespace Game.Systems {
    /// <summary>
    /// BehaviourTree ���� Ŭ����
    /// </summary>
    public class BehaviourTree {
        private BehaviourNodeBase _rootNode;

        private int _id;

        public int ID => _id;

        public BehaviourTree(int ownerID) {
            _id = ownerID;
        }

        public void SetRootNode(BehaviourNodeBase rootNode) {
            _rootNode = rootNode;
        }

        public NodeState Update() {
            if (_rootNode == null) {
                GameDebug.Log("BehaviourTree RootNode�� �������� ����");
                return NodeState.Failure;
            }

            return _rootNode.Evaluate();
        }

        public void Reset() {
            _rootNode?.Reset();
        }
    }
}