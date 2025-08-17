using Game.Core;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Game.Data;
using System;
namespace Game.Systems {
    /// <summary>
    /// 플레이어 추적 액션
    /// </summary>
    [Serializable]
    public class ChasePlayerAction : ActionNodeBase {
        [SerializeField]
        private CharacterMoveData _moveData;
        public ChasePlayerAction(CharacterMoveData moveData) {
            _moveData = moveData;
        }
        public void SetMoveData(CharacterMoveData moveData) {
            _moveData = moveData;
        }

        public CharacterMoveData MoveData => _moveData;

        public override NodeState Evaluate(int id) {

            Transform playerTr = AIDataProvider.GetPlayerTransform();
            if (playerTr == null) {
                _nodeState = NodeState.Failure;
                return _nodeState;
            }

            Transform ownerTr = AIDataProvider.GetEnemyTransform(id);
            if (ownerTr == null) {
                GameDebug.Log($"적 Transform 찾을 수 없음 ID: {id}");
                _nodeState = NodeState.Failure;
                return _nodeState;
            }

            // 플레이어 쪽으로 이동
            Vector3 dir = (playerTr.position - ownerTr.position).normalized;
            Vector3 newPos = ownerTr.position + (dir * _moveData.moveSpeed * Time.deltaTime);
            Quaternion newRota = ownerTr.ToRotateQuaternion(dir, _moveData.rotationSpeed, Time.deltaTime);

            ownerTr.SetPositionAndRotation(newPos, newRota);

            return _nodeState;
        }

        /// <summary>
        /// 액션 노드는 그대로 활용해도 문제 없음
        /// </summary>
        public override BehaviourNodeBase DeepCopy() {
            return this;
        }
    }
}
