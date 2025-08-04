using UnityEngine;

namespace Game.Core
{
    public interface IHealthInitializable {
        void InitHealth(GameObject obj, Vector2 offset); 
    }
}
