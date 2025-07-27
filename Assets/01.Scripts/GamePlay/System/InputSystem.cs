using Data;
using R3;
using UnityEngine;
using Zenject;

namespace GamePlay
{

    /// <summary>
    /// 책임: Input Data를 가공해 사용 데이터로 변환
    /// </summary>
    public class InputSystem : MonoBehaviour
    {
        [Inject] private IInputStrategy _inputStrategy;
        [Inject] private InputData _inputData;
        [Inject] private PlayerMoveData _playerMoveData;

        private void Awake() {
            Bind();
        }
        private void Bind() {

            // Input Type이 변경되면 playerData Update
            _inputData.inputTypeObservable
                .ThrottleLastFrame(1)
                .Subscribe(UpdatePlayerData)
                .AddTo(this);
        }

        private void Update() {
            _inputStrategy.UpdateInput();
        }

        private void UpdatePlayerData(InputType type) {
            if(type == InputType.First || type == InputType.Push) { // Input data를 Move Data 로 변환
                Vector2 dir = _inputData.GetCurrentPointPosition() - _inputData.GetFirstFramePointPosition(); // 방향 계산
                _playerMoveData.moveDirObservable.Value = dir.normalized;
            } else if(type == InputType.End) {
                _playerMoveData.moveDirObservable.Value = Vector3.zero;
            }
        }
    }
}
