using UnityEngine;
using R3;
using System;
using Core.Utilis;
using Game.Core;

namespace Game.Models
{
    /// <summary>
    /// Input 상태 모델
    /// </summary>
    public class InputModel
    {
        private readonly ReactiveProperty<InputType> _inputType;
        private readonly ReactiveProperty<Vector2> _currentPosition;
        private readonly ReactiveProperty<Vector2> _startPosition;
        private readonly ReactiveProperty<float> _clickStartTime;
        
        public ReadOnlyReactiveProperty<InputType> InputType { get; }
        public ReadOnlyReactiveProperty<Vector2> CurrentPosition { get; }
        public ReadOnlyReactiveProperty<Vector2> StartPosition { get; }
        public ReadOnlyReactiveProperty<float> ClickStartTime { get; }
        
        // 계산된 프로퍼티
        public ReadOnlyReactiveProperty<Vector2> DragDirection { get; private set; }
        public ReadOnlyReactiveProperty<float> DragDistance { get; private set; }
        
        private CompositeDisposable _disposables = new();
        
        public InputModel()
        {
            _inputType = new ReactiveProperty<InputType>(Game.Core.InputType.None, 
                AlwaysFalseComparer<InputType>.Instance);
            _currentPosition = new ReactiveProperty<Vector2>(Vector2.zero);
            _startPosition = new ReactiveProperty<Vector2>(Vector2.zero);
            _clickStartTime = new ReactiveProperty<float>(0f);
            
            InputType = _inputType;
            CurrentPosition = _currentPosition;
            StartPosition = _startPosition;
            ClickStartTime = _clickStartTime;
            
            // 드래그 방향 계산
            DragDirection = Observable.CombineLatest(_currentPosition, _startPosition)
                .Select(positions => (positions[0] - positions[1]).normalized)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            
            // 드래그 거리 계산
            DragDistance = Observable.CombineLatest(_currentPosition, _startPosition)
                .Select(positions => Vector2.Distance(positions[0], positions[1]))
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }
        
        public void SetInputType(InputType inputType)
        {
            _inputType.Value = inputType;
        }
        
        public void SetCurrentPosition(Vector2 position)
        {
            _currentPosition.Value = position;
        }
        
        public void SetStartPosition(Vector2 position)
        {
            _startPosition.Value = position;
        }
        
        public void SetClickStartTime(float time)
        {
            _clickStartTime.Value = time;
        }
        
        public float GetClickDuration()
        {
            return _clickStartTime.Value > 0 ? Time.time - _clickStartTime.Value : 0f;
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}