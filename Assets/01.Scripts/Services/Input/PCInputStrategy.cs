using UnityEngine;
using Game.Policies;
using Game.Data;

namespace Game.Services
{
    public class PCInputStrategy : InputStrategyBase
    {
        public PCInputStrategy(IInputPolicy inputPolicy, SO_InputConfig config) 
            : base(inputPolicy, config)
        {
        }
        
        protected override void ProcessInput()
        {
            // 마우스 클릭 시작
            if (Input.GetMouseButtonDown(0))
            {
                if (!_inputPolicy.ShouldIgnoreUIClick())
                {
                    _isInputActive = true;
                    _isInputStarted = true;
                }
            }
            // 마우스 클릭 중
            else if (Input.GetMouseButton(0) && _isInputActive)
            {
                // 드래그 중이므로 특별한 처리 없음
            }
            // 마우스 클릭 종료
            else if (Input.GetMouseButtonUp(0) && _isInputActive)
            {
                _isInputActive = false;
                _isInputEnded = true;
            }
            // 입력이 없는 상태
            else if (!Input.GetMouseButton(0))
            {
                _isInputActive = false;
            }
        }
        
        public override Vector2 GetCurrentPosition()
        {
            return Input.mousePosition;
        }
    }
}