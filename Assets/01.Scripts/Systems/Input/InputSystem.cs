using Game.Models;
using Game.Services;
using Game.Core;
using Game.Data;
using R3;
using UnityEngine;
using Game.Policies;
namespace Game.Systems
{
    public class InputSystem
    {
        private readonly InputModel _inputModel;
        private readonly IInputStrategy _inputStrategy;
        private readonly IInputPolicy _inputPolicy;
        private readonly SO_InputConfig _config;
        
        // Input 이벤트
        public readonly Subject<Vector2> OnClickEvent = new();
        public readonly Subject<Vector2> OnDragStartEvent = new();
        public readonly Subject<Vector2> OnDragEvent = new();
        public readonly Subject<Vector2> OnDragEndEvent = new();
        
        private bool _isDragging = false;
        
        public InputSystem(InputModel inputModel, IInputStrategy inputStrategy, 
                          IInputPolicy inputPolicy, SO_InputConfig config)
        {
            _inputModel = inputModel;
            _inputStrategy = inputStrategy;
            _inputPolicy = inputPolicy;
            _config = config;
        }
        
        public void UpdateInput()
        {
            _inputStrategy.UpdateInput();
            
            var currentPosition = _inputStrategy.GetCurrentPosition();
            _inputModel.SetCurrentPosition(currentPosition);
            
            ProcessInputEvents();
        }
        
        private void ProcessInputEvents()
        {
            HandleInputStart();
            HandleInputActive();
            HandleInputEnd();
        }
        
        private void HandleInputStart()
        {
            if (_inputStrategy.IsInputStarted())
            {
                var startPosition = _inputStrategy.GetCurrentPosition();
                _inputModel.SetInputType(InputType.First);
                _inputModel.SetStartPosition(startPosition);
                _inputModel.SetClickStartTime(Time.time);
                _isDragging = false;
            }
        }
        
        private void HandleInputActive()
        {
            if (_inputStrategy.IsInputActive())
            {
                var dragDistance = _inputModel.DragDistance.CurrentValue;
                
                if (!_isDragging && _inputPolicy.IsValidDrag(dragDistance, _config.dragThreshold))
                {
                    _isDragging = true;
                    _inputModel.SetInputType(InputType.Push);
                    OnDragStartEvent.OnNext(_inputModel.StartPosition.CurrentValue);
                }
                
                if (_isDragging)
                {
                    OnDragEvent.OnNext(_inputModel.CurrentPosition.CurrentValue);
                }
            }
        }
        
        private void HandleInputEnd()
        {
            if (_inputStrategy.IsInputEnded())
            {
                _inputModel.SetInputType(InputType.End);
                
                if (_isDragging)
                {
                    OnDragEndEvent.OnNext(_inputModel.CurrentPosition.CurrentValue);
                }
                else
                {
                    HandleClick();
                }
                
                _isDragging = false;
                _inputModel.SetInputType(InputType.None);
            }
        }
        
        private void HandleClick()
        {
            var clickTime = _inputModel.GetClickDuration();
            if (_inputPolicy.IsValidClick(clickTime, _config.clickThreshold))
            {
                OnClickEvent.OnNext(_inputModel.CurrentPosition.CurrentValue);
            }
        }
        
        public InputModel GetInputModel() => _inputModel;
    }
}