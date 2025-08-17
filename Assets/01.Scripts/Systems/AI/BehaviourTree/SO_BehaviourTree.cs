using Game.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Systems {
    /// <summary>
    /// BehaviourTree 메인 클래스
    /// </summary>
    public class SO_BehaviourTree : ScriptableObject{

        [SerializeReference]
        private BehaviourNodeBase _rootNode;

        private int _id;

        public int ID => _id;

        public BehaviourNodeBase GetRootNode() => _rootNode;


        #region 복사
        /// <summary>
        /// 현재 트리의 Deep Copy를 생성
        /// </summary>
        public SO_BehaviourTree CreateCopy() {
            // 새 SO 인스턴스 생성
            SO_BehaviourTree newTree = ScriptableObject.CreateInstance<SO_BehaviourTree>();

            // ID 복사
            newTree.SetID(_id);

            // 루트 노드가 있다면 Deep Copy
            if (_rootNode != null) {
                BehaviourNodeBase newRoot = _rootNode.DeepCopy();
                newTree.SetRootNode(newRoot);
            }

            return newTree;
        }
#endregion

        public void SetID(int id) {
            _id = id;
        }

        public void SetRootNode(BehaviourNodeBase rootNode) {
            _rootNode = rootNode;
        }

        public NodeState Update() {
            if (_rootNode == null) {
                GameDebug.Log("BehaviourTree RootNode가 설정되지 않음");
                return NodeState.Failure;
            }

            return _rootNode.Evaluate(ID);
        }

        public void Reset() {
            _rootNode?.Reset();
        }
    }
}