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
        public ReadOnlyReactiveProperty<Vector2> MoveDirection => _playerMoveModel.RORP_MoveDirection;
        public ReadOnlyReactiveProperty<bool> IsMoving => _playerMoveModel.RORP_IsMoving;
        public ReadOnlyReactiveProperty<InputType> InputType => _inputModel.RORP_InputType;
        
        // Input 상태 프로퍼티들
        public ReadOnlyReactiveProperty<Vector2> CurrentPosition => _inputModel.RORP_CurrentPosition;
        public ReadOnlyReactiveProperty<Vector2> StartPosition => _inputModel.RORP_StartPosition;
        public ReadOnlyReactiveProperty<Vector2> DragDirection => _inputModel.RORP_DragDirection;
        public ReadOnlyReactiveProperty<float> DragDistance => _inputModel.RORP_DragDistance;
        

        public Vector2 FirstFramePointScreenPosition => _inputModel.RORP_StartPosition.CurrentValue;
        public Vector2 CurrentPointScreenPosition => _inputModel.RORP_CurrentPosition.CurrentValue;
        

    }
}