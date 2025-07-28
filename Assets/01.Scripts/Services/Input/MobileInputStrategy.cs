using UnityEngine;
using Game.Policies;
using Game.Data;
namespace Game.Services
{
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
            if (Input.touchCount == 1)
            {
                Touch touch = Input.touches[0];
                
                // UI 위에 있지 않은 터치만 처리
                if (!_inputPolicy.ShouldIgnoreUIClick(touch.fingerId))
                {
                    _currentTouch = touch;
                    
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            _isInputActive = true;
                            _isInputStarted = true;
                            break;
                            
                        case TouchPhase.Moved:
                        case TouchPhase.Stationary:
                            // 터치 중이므로 특별한 처리 없음
                            break;
                            
                        case TouchPhase.Ended:
                        case TouchPhase.Canceled:
                            if (_isInputActive)
                            {
                                _isInputActive = false;
                                _isInputEnded = true;
                            }
                            break;
                    }
                }
            }
            // 터치가 없거나 여러 개일 때
            else
            {
                if (_isInputActive)
                {
                    _isInputActive = false;
                    _isInputEnded = true;
                }
            }
        }
        
        public override Vector2 GetCurrentPosition()
        {
            return _currentTouch?.position ?? Vector2.zero;
        }
    }
}