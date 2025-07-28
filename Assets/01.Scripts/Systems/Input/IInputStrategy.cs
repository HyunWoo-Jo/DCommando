using Game.Core;
using UnityEngine;

namespace Game.Systems {
    public interface IInputStrategy {
        void UpdateInput();
        Vector2 GetCurrentPosition();

        InputType GetInputType();
    }
}