using UnityEngine;
using Game.Policies;
using Game.Data;
using Game.Core;
namespace Game.Systems {
    public class MobileInputStrategy : InputStrategyBase
    {
        private Touch? _currentTouch;
        
        public MobileInputStrategy(IInputPolicy inputPolicy, SO_InputConfig config) 
            : base(inputPolicy, config)
        {
        }
        
        protected override void ProcessInput()
        {
            _currentTouch = null;

            // 터치가 하나만 있을 때
            if (Input.touchCount == 1) {
                Touch touch = Input.touches[0];

                // UI 위 터치 무시
                if (!_inputPolicy.ShouldIgnoreUIClick(touch.fingerId)) {
                    _currentTouch = touch;

                    switch (touch.phase) {
                        case TouchPhase.Began:
                        _inputType = InputType.First; // 터치 시작
                        break;

                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                        if (_inputType == InputType.First || _inputType == InputType.Push)
                            _inputType = InputType.Push; // 터치 중
                        break;

                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                        if (_inputType == InputType.First || _inputType == InputType.Push)
                            _inputType = InputType.End; // 터치 종료
                        break;
                    }
                }
            }
            // 터치가 없거나 여러 개일 때 입력 종료 처리
            else {
                if (_inputType == InputType.Push || _inputType == InputType.First)
                    _inputType = InputType.End;
                else if (_inputType == InputType.End)
                    _inputType = InputType.None; // 한 프레임 후 초기화
            }
        }
        
        public override Vector2 GetCurrentPosition()
        {
            return _currentTouch?.position ?? Vector2.zero;
        }
    }
}