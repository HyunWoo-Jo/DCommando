using UnityEngine;

namespace Game.Services
{
    public interface IInputStrategy
    {
        void UpdateInput();
        Vector2 GetCurrentPosition();
        bool IsInputActive();
        bool IsInputStarted();
        bool IsInputEnded();
    }
}