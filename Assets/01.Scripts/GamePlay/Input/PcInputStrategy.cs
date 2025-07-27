using UnityEngine;
using UnityEngine.EventSystems;
using Data;
using UI;
using Zenject;
namespace GamePlay
{
    /// <summary>
    /// 책임: PC 환경에서의 터치 상태를 판별
    /// </summary>
    public class PcInputStrategy : InputBase {

        [Inject]
        public PcInputStrategy(InputData inputData) : base(inputData) {
        }

        public override void UpdateInput() {

            //// 클릭 처리
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) { // UI 위 클릭이 아닐경우                
                InputType = InputType.First;
                clickStartTime = Time.time;
                firstFramePosition = GetPointPosition();
            } else if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) { // Push중
                InputType = InputType.Push;
            }
            if (Input.GetMouseButtonUp(0) && (InputType == InputType.First || InputType == InputType.Push)) { // UP 
                InputType = InputType.End;
            } else if (InputType == InputType.End) {
                InputType = InputType.None;
            }
        }

        public override Vector2 GetPointPosition() {
            return Input.mousePosition;
        }
    }

}
