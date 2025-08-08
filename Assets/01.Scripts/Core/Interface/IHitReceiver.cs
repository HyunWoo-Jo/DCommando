using UnityEngine;

namespace Game.Core
{
    public interface IHitReceiver {
        void OnHit(GameObject hitObject, DamageType type);
    }
}
