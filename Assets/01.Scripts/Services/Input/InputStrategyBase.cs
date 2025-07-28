using UnityEngine;
using Game.Policies;
using Game.Data;

namespace Game.Services
{
    public abstract class InputStrategyBase : IInputStrategy
    {
        protected readonly IInputPolicy _inputPolicy;
        protected readonly SO_InputConfig _config;
        
        protected bool _isInputActive;
        protected bool _isInputStarted;
        protected bool _isInputEnded;
        
        protected InputStrategyBase(IInputPolicy inputPolicy, SO_InputConfig config)
        {
            _inputPolicy = inputPolicy;
            _config = config;
        }
        
        public virtual void UpdateInput()
        {
            _isInputStarted = false;
            _isInputEnded = false;
            
            ProcessInput();
        }
        
        protected abstract void ProcessInput();
        
        public abstract Vector2 GetCurrentPosition();
        
        public bool IsInputActive() => _isInputActive;
        public bool IsInputStarted() => _isInputStarted;
        public bool IsInputEnded() => _isInputEnded;
    }
}