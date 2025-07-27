
using UnityEngine;
using Zenject;
using System;
using R3;
using UnityEngine.Assertions;
using System.Diagnostics;
using Data;
////////////////////////////////////////////////////////////////////////////////////
// Auto Generated Code
namespace UI
{
    public class ControllerView : MonoBehaviour
    {
        [Inject] private ControllerViewModel _viewModel;


        [SerializeField] private Transform _baseTr;
        [SerializeField] private Transform _pivotTr;
        private Vector2 _originBasePos; // 기본 Position
        private Vector2 _originPivotLocalPos; // 기본 Position
        [SerializeField] private float moveInterval = 100f;    // 최대 변위
        [SerializeField] private float smooth = 10f;          // 보간 속도

        private void Awake() {
#if UNITY_EDITOR // Assertion
            RefAssert();
#endif
            // 초기화
            _originBasePos = _baseTr.position;
            _originPivotLocalPos = _pivotTr.localPosition;
            Bind();

        }
        private void Start() {
            _viewModel.Notify();
        }

        

#if UNITY_EDITOR
        // 검증
        private void RefAssert() {
            Assert.IsNotNull(_baseTr);
            Assert.IsNotNull(_pivotTr);
        }
#endif

        private void Bind() {
            // Move Dir에 BInd
            _viewModel.RO_MoveDir
                .ThrottleLastFrame(1)
                .Subscribe(UpdatePivotUI)
                .AddTo(this);

            // Input Type 이 바뀌면 호출
            _viewModel.RO_InputType
                .ThrottleLastFrame(1)
                .Subscribe(UpdateBaseUI)
                .AddTo(this);
        }

        // Base 갱신
        private void UpdateBaseUI(InputType inputType) {
            switch (inputType) {

                case InputType.First:
                _baseTr.transform.position = _viewModel.FirstFramePointScreenPosition;
                break;
                case InputType.End:
                _baseTr.transform.position = _originBasePos;
                break;
            }

        }

        // Controller Pivot UI 갱신
        private void UpdatePivotUI(Vector2 dir) {
            if (dir.sqrMagnitude < 0.1f)    
   {            // 위치 초기화
                _pivotTr.localPosition = _originPivotLocalPos;
                return;                           
            }

            // 위치 계산
            Vector2 target = _originPivotLocalPos + dir * moveInterval;
            _pivotTr.localPosition = Vector3.Lerp(
                _pivotTr.localPosition,   // 현재
                target,                   // 목표
                smooth * Time.unscaledDeltaTime);
        }
    }
}
