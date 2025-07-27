using UnityEngine;
using Data;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Zenject;
namespace GamePlay
{
    /// <summary>
    /// Input 상태를 체크하기위한 Abstruct class
    /// </summary>
    public abstract class InputBase : IInputStrategy {
        private InputData _inputData;
        protected Vector2 firstFramePosition;
        protected float clickStartTime;

        [Inject]
        protected InputBase(InputData inputData) {
            _inputData = inputData;
            // Func Bind
            _inputData.GetCurrentPointPosition = GetPointPosition;
            _inputData.GetFirstFramePointPosition = GetFirstFramePosition;
        }

        protected InputType InputType {
            get { return _inputData.inputTypeObservable.Value; }
            set { _inputData.inputTypeObservable.Value = value; }
        }

        public virtual float ClickTime() {
            return Time.time - clickStartTime;
        }
        public InputType GetInputType() => InputType;

        public virtual Vector2 GetFirstFramePosition() => firstFramePosition;

        public abstract Vector2 GetPointPosition();

        /// <summary>
        /// 클릭 구현부 / 필수로 들어가야 되는 목록: 클릭 시작 (시간, 위치), 상황에 맞는 클릭 타입 
        /// </summary>
        public abstract void UpdateInput();

    }
}
