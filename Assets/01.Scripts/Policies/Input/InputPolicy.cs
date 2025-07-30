using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Policies {
    public class InputPolicy : IInputPolicy
    {
        public bool IsValidClick(float clickTime, float threshold)
        {
            return clickTime <= threshold;
        }
        
        public bool IsValidDrag(float dragDistance, float threshold)
        {
            return dragDistance >= threshold;
        }
        

    }
}