
using Zenject;
using System;
using Data;
using R3;
using UnityEngine;
namespace UI
{
    public class ControllerViewModel 
    {
        [Inject] private PlayerMoveData _inputPlayerMoveData;
        [Inject] private InputData _inputData;

        /// <summary>
        /// RO Data
        /// </summary>
        public ReadOnlyReactiveProperty<Vector2> RO_MoveDir => _inputPlayerMoveData.moveDirObservable;
        public ReadOnlyReactiveProperty<InputType> RO_InputType => _inputData.inputTypeObservable;

        public Vector2 FirstFramePointScreenPosition => _inputData.GetFirstFramePointPosition();
        public Vector2 CurrentPointScreenPosition => _inputData.GetCurrentPointPosition();

        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() {

        }

    }
} 
