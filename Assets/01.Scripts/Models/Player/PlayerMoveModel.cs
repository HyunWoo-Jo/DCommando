using UnityEngine;
using R3;
using Game.Core;
using Game.Data;
using System;
namespace Game.Models
{
    /// <summary>
    /// 플레이어 이동 데이터 모델
    /// </summary>
    public class PlayerMoveModel : IDisposable {
        private readonly ReactiveProperty<Vector2> RP_moveDirection = new(Vector2.zero, AlwaysFalseComparer<Vector2>.Instance);
        private readonly ReactiveProperty<CharacterMoveData> RP_moveData = new ();
        private readonly ReactiveProperty<bool> RP_isMoving = new(false);

        public ReadOnlyReactiveProperty<Vector2> RORP_MoveDirection => RP_moveDirection;
        public ReadOnlyReactiveProperty<CharacterMoveData> RORP_MoveData => RP_moveData;
        public ReadOnlyReactiveProperty<bool> RORP_IsMoving => RP_isMoving;

        public void Dispose() {
            RP_moveDirection?.Dispose();
            RP_moveData?.Dispose();
            RP_isMoving?.Dispose();
        }

        public void SetMoveDirection(Vector2 direction) {
            RP_moveDirection.Value = direction;
            RP_isMoving.Value = direction.magnitude > 0.01f;
        }

        public void SetMoveData(CharacterMoveData data) {
            RP_moveData.Value = data;
        }

        public void StopMoving() {
            RP_moveDirection.Value = Vector2.zero;
            RP_isMoving.Value = false;
        }

       
    }
}