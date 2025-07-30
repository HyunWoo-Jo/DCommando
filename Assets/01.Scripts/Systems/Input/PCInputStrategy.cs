using UnityEngine;
using Game.Policies;
using Game.Data;
using Game.Core;
using UnityEngine.EventSystems;

namespace Game.Systems
{
    public class PCInputStrategy : InputStrategyBase {

        protected override void ProcessInput() {
            switch (_inputType) {
                case InputType.None:
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    _inputType = InputType.First;
                break;

                case InputType.First:
                if (Input.GetMouseButtonUp(0))
                    _inputType = InputType.End;
                else if (Input.GetMouseButton(0))
                    _inputType = InputType.Push;
                break;

                case InputType.Push:
                if (!Input.GetMouseButton(0)) // 버튼 해제 or Up
                    _inputType = InputType.End;
                break;

                case InputType.End:
                _inputType = InputType.None; // 한 프레임만 유지
                break;
            }
        }

        public override Vector2 GetCurrentPosition() {
            return Input.mousePosition;
        }
    }
}