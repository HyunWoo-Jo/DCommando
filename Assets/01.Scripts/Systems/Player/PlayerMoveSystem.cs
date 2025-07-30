using Game.Models;
using Game.Systems;
using R3;
using UnityEngine;
using Zenject;

namespace Game.Systems
{
    /// <summary>
    /// 플레이어 이동 시스템
    /// </summary>
    public class PlayerMoveSystem {
        [Inject] private readonly PlayerMoveModel _playerMoveModel;
        [Inject] private readonly InputModel _inputModel;
        [Inject] private readonly IInputProvider _inputProvider;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        private void Initialize() {
            // 드래그 시작 시 이동 시작
            _inputProvider.OnDragStartEvent
                .Subscribe(OnDragStart)
                .AddTo(_disposables);

            // 드래그 중 이동 방향 업데이트
            _inputProvider.OnDragEvent
                .Subscribe(OnDrag)
                .AddTo(_disposables);

            // 드래그 종료 시 이동 중지
            _inputProvider.OnDragEndEvent
                .Subscribe(OnDragEnd)
                .AddTo(_disposables);
        }

        private void OnDragStart(Vector2 position) {
            // 드래그 시작 시 초기 방향 설정
            var direction = _inputModel.RORP_DragDirection.CurrentValue;
            _playerMoveModel.SetMoveDirection(direction);
        }

        private void OnDrag(Vector2 position) {
            // 드래그 중 방향 업데이트
            var direction = _inputModel.RORP_DragDirection.CurrentValue;
            _playerMoveModel.SetMoveDirection(direction);
        }

        private void OnDragEnd(Vector2 position) {
            // 드래그 종료 시 이동 중지
            _playerMoveModel.StopMoving();
        }

        public void Dispose() {
            _disposables?.Dispose();
        }
    }
}