using Game.Models;
using Game.Services;
using Game.Core;
using Game.Data;
using R3;
using UnityEngine;
using Game.Policies;
using Zenject;
using System;
namespace Game.Systems
{
    public class InputSystem : IInputProvider, IInitializable, IDisposable  {
        [Inject] private readonly InputModel _inputModel;
        [Inject] private readonly IInputStrategy _inputStrategy;
        [Inject] private readonly IInputPolicy _inputPolicy;
        [Inject] private readonly SO_InputConfig _config;
        [Inject] private readonly IUpdater _updater;
        // Input 이벤트
        private readonly Subject<Vector2> _onClickEvent = new();
        private readonly Subject<Vector2> _onDragStartEvent = new();
        private readonly Subject<Vector2> _onDragEvent = new();
        private readonly Subject<Vector2> _onDragEndEvent = new();

        public Observable<Vector2> OnClickEvent => _onClickEvent;
        public Observable<Vector2> OnDragStartEvent => _onDragStartEvent;
        public Observable<Vector2> OnDragEvent => _onDragEvent;
        public Observable<Vector2> OnDragEndEvent => _onDragEndEvent;

        private bool _isDragging = false;
        public bool IsDragging => _isDragging;

        private CompositeDisposable _disposables = new();

        #region Zenject 관리
        public void Initialize() {
            _updater.OnUpdate
                .Subscribe(Update)
                .AddTo(_disposables);
        }
        public void Dispose() {
            _disposables?.Dispose();
            _disposables = null;
        }
        #endregion

        public void Update(float deltaTime) {
            UpdateInput();
        }

        // Input 갱신
        private void UpdateInput() {
            // Input 갱신
            _inputStrategy.UpdateInput();

            _inputModel.SetCurrentPosition(_inputStrategy.GetCurrentPosition());
            // Input Event 발생
            ProcessInputEvents();   
        }

        private void ProcessInputEvents() {
            // Input Event
            HandleInputStart();
            HandleInputActive();
            HandleInputEnd();
        }

        /// <summary>
        /// Input First
        /// </summary>
        private void HandleInputStart() {
            if (_inputStrategy.GetInputType() == InputType.First) {
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
        private void HandleInputActive() {
            if (_inputStrategy.GetInputType() == InputType.Push) {
                var dragDistance = _inputModel.RORP_DragDistance.CurrentValue;
                // 아직 드래그가 아니고, 정책에 따라 드래그로 판정될 때
                if (!_isDragging && _inputPolicy.IsValidDrag(dragDistance, _config.dragThreshold)) {
                    _isDragging = true;
                    _inputModel.SetInputType(InputType.Push);
                    // 드래그 시작 이벤트
                    _onDragStartEvent.OnNext(_inputModel.RORP_StartPosition.CurrentValue);
                }

                // 드래그 상태에서는 매 프레임 드래그 이벤트 발생
                if (_isDragging) {
                    _onDragEvent.OnNext(_inputModel.RORP_CurrentPosition.CurrentValue);
                }
            }
        }

        /// <summary>
        /// Input End
        /// </summary>
        private void HandleInputEnd() {
            if (_inputStrategy.GetInputType() == InputType.End) {
                _inputModel.SetInputType(InputType.End);

                if (_isDragging) {
                    _onDragEndEvent.OnNext(_inputModel.RORP_CurrentPosition.CurrentValue);
                } else {
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
        private void HandleClick() {
            // 눌러다 땐 간격에 따라 클릭 이벤트 발생
            var clickTime = _inputModel.GetClickDuration();
            if (_inputPolicy.IsValidClick(clickTime, _config.clickThreshold)) {
                _onClickEvent.OnNext(_inputModel.RORP_CurrentPosition.CurrentValue);
            }
        }

        public InputModel GetInputModel() => _inputModel;


    }
}