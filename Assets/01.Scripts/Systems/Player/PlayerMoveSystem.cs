using Game.Models;
using Game.Systems;
using R3;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// 플레이어 이동 시스템
    /// </summary>
    public class PlayerMoveSystem
    {
        private readonly PlayerMoveModel _playerMoveModel;
        private readonly InputSystem _inputSystem;
        private readonly CompositeDisposable _disposables = new();
        
        public PlayerMoveSystem(PlayerMoveModel playerMoveModel, InputSystem inputSystem)
        {
            _playerMoveModel = playerMoveModel;
            _inputSystem = inputSystem;
            
            Initialize();
        }
        
        private void Initialize()
        {
            // 드래그 시작 시 이동 시작
            _inputSystem.OnDragStartEvent
                .Subscribe(OnDragStart)
                .AddTo(_disposables);
            
            // 드래그 중 이동 방향 업데이트
            _inputSystem.OnDragEvent
                .Subscribe(OnDrag)
                .AddTo(_disposables);
            
            // 드래그 종료 시 이동 중지
            _inputSystem.OnDragEndEvent
                .Subscribe(OnDragEnd)
                .AddTo(_disposables);
        }
        
        private void OnDragStart(Vector2 position)
        {
            // 드래그 시작 시 초기 방향 설정
            var inputModel = _inputSystem.GetInputModel();
            var direction = inputModel.DragDirection.CurrentValue;
            _playerMoveModel.SetMoveDirection(direction);
        }
        
        private void OnDrag(Vector2 position)
        {
            // 드래그 중 방향 업데이트
            var inputModel = _inputSystem.GetInputModel();
            var direction = inputModel.DragDirection.CurrentValue;
            _playerMoveModel.SetMoveDirection(direction);
        }
        
        private void OnDragEnd(Vector2 position)
        {
            // 드래그 종료 시 이동 중지
            _playerMoveModel.StopMoving();
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}