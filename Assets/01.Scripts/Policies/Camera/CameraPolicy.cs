using UnityEngine;

namespace Game.Policies
{
    public class CameraPolicy : ICameraPolicy {
        public bool CanFollow(Transform target) {
            return target != null && target.gameObject.activeInHierarchy;
        }

        public bool CanZoom(float currentZoom, float targetZoom, float minZoom, float maxZoom) {
            return targetZoom >= minZoom && targetZoom <= maxZoom;
        }

        public bool CanShake() {
            // 추후 변경되어야함 (조건 정하기)
            return true;
        }

        public Vector3 ClampCameraPosition(Vector3 position, Bounds bounds) {
            return new Vector3(
                Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
                Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
                position.z
            );
        }
    }
}
