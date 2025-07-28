using UnityEngine;

namespace Game.Policies {
    public interface IInputPolicy
    {
        bool IsValidClick(float clickTime, float threshold);
        bool IsValidDrag(float dragDistance, float threshold);
        bool ShouldIgnoreUIClick(int touchId = -1);
    }
}