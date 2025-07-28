using Zenject;
using System;
using Game.Models;
using Game.Core;
using R3;
using UnityEngine;

namespace Game.ViewModels
{
    public class ControllerViewModel
    {
        [Inject] private PlayerMoveModel _playerMoveModel;
        [Inject] private InputModel _inputModel;

        // UI 바인딩용 프로퍼티
        public ReadOnlyReactiveProperty<Vector2> MoveDirection => _playerMoveModel.MoveDirection;
        public ReadOnlyReactiveProperty<bool> IsMoving => _playerMoveModel.IsMoving;
        public ReadOnlyReactiveProperty<InputType> InputType => _inputModel.InputType;
        
        // Input 상태 프로퍼티들
        public ReadOnlyReactiveProperty<Vector2> CurrentPosition => _inputModel.CurrentPosition;
        public ReadOnlyReactiveProperty<Vector2> StartPosition => _inputModel.StartPosition;
        public ReadOnlyReactiveProperty<Vector2> DragDirection => _inputModel.DragDirection;
        public ReadOnlyReactiveProperty<float> DragDistance => _inputModel.DragDistance;
        
        // 편의 프로퍼티들
        public Vector2 FirstFramePointScreenPosition => _inputModel.StartPosition.CurrentValue;
        public Vector2 CurrentPointScreenPosition => _inputModel.CurrentPosition.CurrentValue;
        public bool IsInputActive => _inputModel.DragDistance.CurrentValue > 10f; // 드래그 임계값
        
        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() {
            // 컨트롤러 초기화 완료 알림
        }
    }
}