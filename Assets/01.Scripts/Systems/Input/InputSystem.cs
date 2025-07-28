using Game.Models;
using Game.Services;
using Game.Core;
using Game.Data;
using R3;
using UnityEngine;
using Game.Policies;
using Zenject;
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

        [Inject]
        public InputSystem(InputModel inputModel, IInputStrategy inputStrategy, 
                          IInputPolicy inputPolicy, SO_InputConfig config)
        {
            _inputModel = inputModel;
            _inputStrategy = inputStrategy;
            _inputPolicy = inputPolicy;
            _config = config;
        }
        
        // Input 갱신
        public void UpdateInput()
        {
            // Input 갱신
            _inputStrategy.UpdateInput();
            
            // 인풋 확인
            var currentPosition = _inputStrategy.GetCurrentPosition();
            _inputModel.SetCurrentPosition(currentPosition);
            
            // Input Event 발생
            ProcessInputEvents();
        }
        
        private void ProcessInputEvents()
        {
            // Input Event
            HandleInputStart();
            HandleInputActive();
            HandleInputEnd();
        }
        
        /// <summary>
        /// Input First
        /// </summary>
        private void HandleInputStart()
        {
            if (_inputStrategy.GetInputType() == InputType.First)
            {
                var startPosition = _inputStrategy.GetCurrentPosition();
                _inputModel.SetInputType(InputType.First);
                _inputModel.SetStartPosition(startPosition);
                _inputModel.SetClickStartTime(Time.time);
                _isDragging = false;
            }
        }
        
        /// <summary>
        /// Push 상태
        /// </summary>
        private void HandleInputActive()
        {
            if (_inputStrategy.GetInputType() == InputType.Push)
            {
                var dragDistance = _inputModel.DragDistance.CurrentValue;

                // 아직 드래그가 아니고, 정책에 따라 드래그로 판정될 때
                if (!_isDragging && _inputPolicy.IsValidDrag(dragDistance, _config.dragThreshold))
                {
                    _isDragging = true;
                    _inputModel.SetInputType(InputType.Push);
                    // 드래그 시작 이벤트
                    OnDragStartEvent.OnNext(_inputModel.StartPosition.CurrentValue);
                }

                // 드래그 상태에서는 매 프레임 드래그 이벤트 발생
                if (_isDragging)
                {
                    OnDragEvent.OnNext(_inputModel.CurrentPosition.CurrentValue);
                }
            }
        }
        
        /// <summary>
        /// Input End
        /// </summary>
        private void HandleInputEnd()
        {
            if (_inputStrategy.GetInputType() == InputType.End)
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
                // 입력 상태 초기화
                _isDragging = false;
                _inputModel.SetInputType(InputType.None);
            }
        }
        
        /// <summary>
        /// click 이벤트
        /// </summary>
        private void HandleClick()
        {
            // 눌러다 땐 간격에 따라 클릭 이벤트 발생
            var clickTime = _inputModel.GetClickDuration();
            if (_inputPolicy.IsValidClick(clickTime, _config.clickThreshold))
            {
                OnClickEvent.OnNext(_inputModel.CurrentPosition.CurrentValue);
            }
        }
        
        public InputModel GetInputModel() => _inputModel;
    }
}