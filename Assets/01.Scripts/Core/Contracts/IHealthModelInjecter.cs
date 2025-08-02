using UnityEngine;

namespace Game.Core
{
    public interface IHealthInjecter {
        void InjectHealth(object healthModel, GameObject obj, Vector2 offset); 
    }
}
