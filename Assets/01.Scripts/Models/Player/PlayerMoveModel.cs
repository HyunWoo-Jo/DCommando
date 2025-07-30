using UnityEngine;
using R3;
using Core.Utilis;

namespace Game.Models
{
    /// <summary>
    /// 플레이어 이동 데이터 모델
    /// </summary>
    public class PlayerMoveModel
    {
        private readonly ReactiveProperty<Vector2> RP_moveDirection = new(Vector2.zero, AlwaysFalseComparer<Vector2>.Instance);
        private readonly ReactiveProperty<float> RP_moveSpeed = new(5f);
        private readonly ReactiveProperty<bool> RP_isMoving = new(false);
        
        public ReadOnlyReactiveProperty<Vector2> RORP_MoveDirection => RP_moveDirection;
        public ReadOnlyReactiveProperty<float> RORP_MoveSpeed => RP_moveSpeed;
        public ReadOnlyReactiveProperty<bool> RORP_IsMoving => RP_isMoving;
        
        public void SetMoveDirection(Vector2 direction)
        {
            RP_moveDirection.Value = direction;
            RP_isMoving.Value = direction.magnitude > 0.01f;
        }
        
        public void SetMoveSpeed(float speed)
        {
            RP_moveSpeed.Value = speed;
        }
        
        public void StopMoving()
        {
            RP_moveDirection.Value = Vector2.zero;
            RP_isMoving.Value = false;
        }
    }
}