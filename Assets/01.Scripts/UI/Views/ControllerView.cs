using UnityEngine;
using Zenject;
using System;
using R3;
using UnityEngine.Assertions;
using Game.ViewModels;

namespace Game.UI
{
    public class ControllerView : MonoBehaviour
    {
        [Inject] private ControllerViewModel _viewModel;

        [Header("컨트롤러 UI 컴포넌트")]
        [SerializeField] private Transform _baseTr;
        [SerializeField] private Transform _pivotTr;
        
        [Header("컨트롤러 설정")]
        [SerializeField] private float _moveInterval = 100f;    // 최대 변위
        [SerializeField] private float _smooth = 10f;          // 보간 속도
        
        private Vector2 _originBasePos; // 기본 Position
        private Vector2 _originPivotLocalPos; // 기본 Position
        private bool _isControllerActive = false;

        private void Awake() {
#if UNITY_EDITOR // Assertion
            RefAssert();
#endif
            // 초기화
            _originBasePos = _baseTr.position;
            _originPivotLocalPos = _pivotTr.localPosition;
            Bind();
        }
        
#if UNITY_EDITOR
        // 검증
        private void RefAssert() {
            Assert.IsNotNull(_baseTr, "BaseTr이 할당되지 않았습니다!");
            Assert.IsNotNull(_pivotTr, "PivotTr이 할당되지 않았습니다!");
        }
#endif

        private void Bind() {
            // 드래그 시작 감지 (StartPosition 변화)
            _viewModel.StartPosition
                .Skip(1) // 초기값 무시
                .Where(pos => pos != Vector2.zero)
                .Subscribe(OnInputStart)
                .AddTo(this);
            
            // 드래그 방향 업데이트
            _viewModel.DragDirection
                .Subscribe(OnDragDirectionChanged)
                .AddTo(this);
                
            // 이동 상태에 따른 컨트롤러 표시/숨김
            _viewModel.IsMoving
                .Subscribe(OnMovingStateChanged)
                .AddTo(this);
                
            // 입력 타입 감지로 종료 감지
            _viewModel.InputType
                .Where(type => type == Core.InputType.End)
                .Subscribe(_ => OnInputEnd())
                .AddTo(this);
        }

        // 입력 시작 시
        private void OnInputStart(Vector2 startPosition) {
            _baseTr.position = startPosition;
            _isControllerActive = true;
            SetControllerVisibility(true);
        }
        
        // 입력 종료 시
        private void OnInputEnd() {
            if (!_viewModel.IsMoving.CurrentValue) {
                _isControllerActive = false;
                SetControllerVisibility(false);
                ResetControllerPosition();
            }
        }
        
        // 드래그 방향 변화 시 피벗 위치 조정
        private void OnDragDirectionChanged(Vector2 dragDirection) {
            if (!_isControllerActive) return;
            
            // 위치 계산
            Vector2 target = _originPivotLocalPos + dragDirection * _moveInterval;
            _pivotTr.localPosition = Vector3.Lerp(
                _pivotTr.localPosition,   // 현재
                target,                   // 목표
                _smooth * Time.deltaTime);
        }
        
        // 이동 상태 변경 시
        private void OnMovingStateChanged(bool isMoving) {
            if (!isMoving && _isControllerActive) {
                // 이동 중지 시 컨트롤러 숨김
                _isControllerActive = false;
                SetControllerVisibility(false);
                ResetControllerPosition();
            }
        }
        
        // 컨트롤러 위치 초기화
        private void ResetControllerPosition() {
            _baseTr.position = _originBasePos;
            _pivotTr.localPosition = _originPivotLocalPos;
        }
        
        // 컨트롤러 가시성 설정
        private void SetControllerVisibility(bool visible) {
            _baseTr.gameObject.SetActive(visible);
        }
        
    }
}