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
        private readonly ReactiveProperty<Vector2> _moveDirection = new(Vector2.zero, AlwaysFalseComparer<Vector2>.Instance);
        private readonly ReactiveProperty<float> _moveSpeed = new(5f);
        private readonly ReactiveProperty<bool> _isMoving = new(false);
        
        public ReadOnlyReactiveProperty<Vector2> MoveDirection => _moveDirection;
        public ReadOnlyReactiveProperty<float> MoveSpeed => _moveSpeed;
        public ReadOnlyReactiveProperty<bool> IsMoving => _isMoving;
        
        public void SetMoveDirection(Vector2 direction)
        {
            _moveDirection.Value = direction;
            _isMoving.Value = direction.magnitude > 0.01f;
        }
        
        public void SetMoveSpeed(float speed)
        {
            _moveSpeed.Value = speed;
        }
        
        public void StopMoving()
        {
            _moveDirection.Value = Vector2.zero;
            _isMoving.Value = false;
        }
    }
}