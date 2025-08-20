using UnityEngine;

namespace Game.Core.Event
{
    public readonly struct CameraMoveToEvent {
        public readonly Vector3 position;
        public readonly float duration;

        public CameraMoveToEvent(Vector3 position, float duration) {
            this.position = position;
            this.duration = duration;
        }
    }

    public readonly struct CameraSetPositionEvent {
        public readonly Vector3 position;

        public CameraSetPositionEvent(Vector3 position) {
            this.position = position;
        }
    }

    public readonly struct CameraZoomToEvent {
        public readonly float zoom;
        public readonly float duration;

        public CameraZoomToEvent(float zoom, float duration) {
            this.zoom = zoom;
            this.duration = duration;
        }
    }

    public readonly struct CameraSetZoomEvent {
        public readonly float zoom;

        public CameraSetZoomEvent(float zoom) {
            this.zoom = zoom;
        }
    }

    public readonly struct CameraShakeEvent {
        public readonly float intensity;
        public readonly float duration;

        public CameraShakeEvent(float intensity, float duration) {
            this.intensity = intensity;
            this.duration = duration;
        }
    }

    public readonly struct CameraFollowTargetEvent {
        public readonly Transform target;

        public CameraFollowTargetEvent(Transform target) {
            this.target = target;
        }
    }

    public readonly struct CameraStopFollowEvent { }

    public readonly struct CameraStopAllAnimationsEvent { }

    // 카메라 상태 알림 이벤트들
    public readonly struct CameraMovedEvent {
        public readonly Vector3 position;

        public CameraMovedEvent(Vector3 position) {
            this.position = position;
        }
    }

    public readonly struct CameraZoomedEvent {
        public readonly float zoom;

        public CameraZoomedEvent(float zoom) {
            this.zoom = zoom;
        }
    }

    public readonly struct CameraShakeStartedEvent { }

    public readonly struct CameraShakeEndedEvent { }

    public readonly struct CameraFollowStartedEvent {
        public readonly Transform target;

        public CameraFollowStartedEvent(Transform target) {
            this.target = target;
        }
    }

    public readonly struct CameraFollowStoppedEvent { }
}
