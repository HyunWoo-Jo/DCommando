using UnityEngine;
using Game.Policies;
using Game.Data;
using Game.Core;

namespace Game.Systems {
    public abstract class InputStrategyBase : IInputStrategy {
        protected readonly IInputPolicy _inputPolicy;
        protected readonly SO_InputConfig _config;

        protected InputType _inputType;

        protected InputStrategyBase(IInputPolicy inputPolicy, SO_InputConfig config) {
            _inputPolicy = inputPolicy;
            _config = config;
        }

        public virtual void UpdateInput() {
            ProcessInput();
        }

        protected abstract void ProcessInput();

        public abstract Vector2 GetCurrentPosition();

        public InputType GetInputType() => _inputType;
    }
}