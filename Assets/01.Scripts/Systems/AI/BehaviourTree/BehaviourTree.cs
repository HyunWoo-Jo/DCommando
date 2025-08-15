using Game.Core;
using UnityEngine;

namespace Game.Systems {
    /// <summary>
    /// BehaviourTree 메인 클래스
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
                GameDebug.Log("BehaviourTree RootNode가 설정되지 않음");
                return NodeState.Failure;
            }

            return _rootNode.Evaluate();
        }

        public void Reset() {
            _rootNode?.Reset();
        }
    }
}