using UnityEngine;

namespace Game.Policies
{
    public interface ICameraPolicy {
        bool CanFollow(Transform target);
        bool CanZoom(float currentZoom, float targetZoom, float minZoom, float maxZoom);
        bool CanShake();
        Vector3 ClampCameraPosition(Vector3 position, Bounds bounds);
    }
}
