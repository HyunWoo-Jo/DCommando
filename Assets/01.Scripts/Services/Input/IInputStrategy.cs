using Game.Core;
using UnityEngine;

namespace Game.Services
{
    public interface IInputStrategy
    {
        void UpdateInput();
        Vector2 GetCurrentPosition();

        InputType GetInputType();
    }
}