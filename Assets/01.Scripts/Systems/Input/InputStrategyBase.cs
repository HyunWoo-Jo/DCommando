using UnityEngine;
using Game.Policies;
using Game.Data;
using Game.Core;

namespace Game.Systems {
    

    /// <summary>
    /// 순수 입력만 체크
    /// </summary>
    public abstract class InputStrategyBase : IInputStrategy {
        protected InputType _inputType;

        public virtual void UpdateInput() {
            ProcessInput();
        }

        protected abstract void ProcessInput();

        public abstract Vector2 GetCurrentPosition();

        public InputType GetInputType() => _inputType;
    }
}