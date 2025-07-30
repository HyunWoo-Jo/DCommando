using UnityEngine;
using R3;
using System;
using Game.Core;

namespace Game.Models
{
    /// <summary>
    /// Input 상태 모델
    /// </summary>
    public class InputModel
    {
        private readonly ReactiveProperty<InputType> RP_inputType = new(InputType.None, AlwaysFalseComparer<InputType>.Instance);
        private readonly ReactiveProperty<Vector2> RP_currentPosition = new(Vector2.zero);
        private readonly ReactiveProperty<Vector2> RP_startPosition = new(Vector2.zero);
        private readonly ReactiveProperty<float> RP_clickStartTime = new(0f);
        
        public ReadOnlyReactiveProperty<InputType> RORP_InputType => RP_inputType;
        public ReadOnlyReactiveProperty<Vector2> RORP_CurrentPosition => RP_currentPosition;
        public ReadOnlyReactiveProperty<Vector2> RORP_StartPosition => RP_startPosition;
        public ReadOnlyReactiveProperty<float> RORP_ClickStartTime => RP_clickStartTime;

        // 계산된 프로퍼티
        public ReadOnlyReactiveProperty<Vector2> RORP_DragDirection { get; private set; }
        public ReadOnlyReactiveProperty<float> RORP_DragDistance { get; private set; }
        
        private CompositeDisposable _disposables = new();
        
        public InputModel()
        {
            // 드래그 방향 계산
             RORP_DragDirection = Observable.CombineLatest(RP_currentPosition, RP_startPosition)
                .Select(positions => (positions[0] - positions[1]).normalized)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            
            // 드래그 거리 계산
            RORP_DragDistance = Observable.CombineLatest(RP_currentPosition, RP_startPosition)
                .Select(positions => Vector2.Distance(positions[0], positions[1]))
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }
        
        public void SetInputType(InputType inputType)
        {
            RP_inputType.Value = inputType;
        }
        
        public void SetCurrentPosition(Vector2 position)
        {
            RP_currentPosition.Value = position;
        }
        
        public void SetStartPosition(Vector2 position)
        {
            RP_startPosition.Value = position;
        }
        
        public void SetClickStartTime(float time)
        {
            RP_clickStartTime.Value = time;
        }
        
        public float GetClickDuration()
        {
            return RP_clickStartTime.Value > 0 ? Time.time - RP_clickStartTime.Value : 0f;
        }
        
        public void Dispose()
        {
            _disposables?.Dispose();
        }
    }
}