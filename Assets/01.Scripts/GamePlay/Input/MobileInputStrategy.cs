using UnityEngine;
using UnityEngine.EventSystems;
using Data;
using UI;
using Zenject;
namespace GamePlay
{
    /// <summary>
    /// 책임: 모바일 환경에서의 터치 상태를 판별
    /// </summary>
    public class MobileInputStrategy : InputBase {
        private Touch? _touch = null;
        private float _prevPinchDistance;   // 직전 프레임 두 손가락 거리
        private bool _isPinching;          // 핀치 진행 중인지

        [Inject]
        public MobileInputStrategy(InputData inputData) : base(inputData) {
        }

        public override void UpdateInput() {
            //// 클릭 처리
            _touch = null;
            // 터치 한개
            if (Input.touchCount == 1) {
                // 클릭 처리
                foreach (var touch in Input.touches) { // 터치 목록 검색
                    if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId)) { // UI 클릭이 아닌 첫번째 터치 목록을 받아옴
                        _touch = touch;
                        break;
                    }
                }
                if (_touch.HasValue) {
                    Touch touch = _touch.Value;
                    if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId)) { // 시작할때 등록 UI 클릭이면 등록하지 않음
                        InputType = InputType.First;
                        firstFramePosition = touch.position;
                        clickStartTime = Time.time;
                    }
                    if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved) {
                        InputType = InputType.Push;
                    }
                    // 해당 터치가 이번 프레임에 끝났는지 확인 (손가락을 뗐거나 취소됨)
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                        InputType = InputType.End;
                    }
                } else {
                    InputType = InputType.None;
                }
            }
        }
        public override Vector2 GetPointPosition() {
            return _touch.HasValue ? _touch.Value.position : Vector2.zero;
        }
    }
}
