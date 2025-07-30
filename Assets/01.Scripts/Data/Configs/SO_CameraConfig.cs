using UnityEngine;
using System;
namespace Game.Data
{
    [Serializable]
    public struct CameraFollowData {
        public float followSpeed;
        public float smoothTime;
        public Vector3 offset;
        public bool useFixedUpdate;
    }

    [Serializable]
    public struct CameraZoomData {
        public float minZoom;
        public float maxZoom;
        public float zoomSpeed;
        public float defaultZoom;
    }

    [Serializable]
    public struct CameraShakeData {
        public float intensity;
        public float duration;
        public float frequency;
        public AnimationCurve shakeCurve;
    }

    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Config/Camera")]
    public class SO_CameraConfig : ScriptableObject
    {
        [SerializeField] private CameraFollowData _followData;
        [SerializeField] private CameraZoomData _zoomData;
        [SerializeField] private CameraShakeData _shakeData;

        public CameraFollowData FollowData => _followData;
        public CameraZoomData ZoomData => _zoomData;
        public CameraShakeData ShakeData => _shakeData;
    }
}
