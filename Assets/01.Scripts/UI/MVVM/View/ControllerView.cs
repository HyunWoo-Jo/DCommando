
using UnityEngine;
using Zenject;
using System;
using R3;
using UnityEngine.Assertions;
using System.Diagnostics;
////////////////////////////////////////////////////////////////////////////////////
// Auto Generated Code
namespace UI
{
    public class ControllerView : MonoBehaviour
    {
        [Inject] private ControllerViewModel _viewModel;

        [SerializeField] private Transform _pivotTr;
        private Vector3 _originPos; // 기본 Position
        private const float MoveInterval = 100;

        private void Awake() {
#if UNITY_EDITOR // Assertion
            RefAssert();
#endif
            // 초기화
            _originPos = _pivotTr.position;
            Bind();

        }
        private void Start() {
            _viewModel.Notify();
        }

        

#if UNITY_EDITOR
        // 검증
        private void RefAssert() {
            Assert.IsNotNull(_pivotTr);
        }
#endif

        private void Bind() {
            // Move Dir에 BInd
            _viewModel.RO_MoveDir
                .ThrottleLastFrame(1)
                .Subscribe(UpdateUI)
                .AddTo(this);
        }

        // Controller UI 갱신
        private void UpdateUI(Vector3 dir) {
            Vector3 targetPos = _originPos + dir * MoveInterval;
            _pivotTr.position = targetPos;
        }
        ////////////////////////////////////////////////////////////////////////////////////
        // your logic here

    }
}
