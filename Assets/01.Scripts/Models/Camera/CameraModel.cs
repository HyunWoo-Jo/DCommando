using UnityEngine;
using R3;
using System;
namespace Game.Models {
    public class CameraModel : IDisposable {
        // Camera 상태
        private readonly ReactiveProperty<Vector3> RP_targetPosition = new();
        private readonly ReactiveProperty<float> RP_currentZoom = new();
        private readonly ReactiveProperty<bool> RP_isFollowing = new();
        private readonly ReactiveProperty<Transform> RP_followTarget = new();
        private readonly ReactiveProperty<bool> RP_isShaking = new();
        private readonly ReactiveProperty<Vector3> RP_shakeOffset = new();

        // Zenject에서 관리
        public void Dispose() {
            RP_targetPosition?.Dispose();
            RP_currentZoom?.Dispose();
            RP_isFollowing?.Dispose();
            RP_followTarget?.Dispose();
            RP_isShaking?.Dispose();
            RP_shakeOffset?.Dispose();
        }

        // Properties
        public ReadOnlyReactiveProperty<Vector3> RORP_TargetPosition => RP_targetPosition.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<float> RORP_CurrentZoom => RP_currentZoom.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> RORP_IsFollowing => RP_isFollowing.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<Transform> RORP_FollowTarget => RP_followTarget.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<bool> RORP_IsShaking => RP_isShaking.ToReadOnlyReactiveProperty();
        public ReadOnlyReactiveProperty<Vector3> RORP_ShakeOffset => RP_shakeOffset.ToReadOnlyReactiveProperty();

        // Methods
        public void SetTargetPosition(Vector3 position) => RP_targetPosition.Value = position;
        public void SetCurrentZoom(float zoom) => RP_currentZoom.Value = zoom;
        public void SetFollowing(bool isFollowing) => RP_isFollowing.Value = isFollowing;
        public void SetFollowTarget(Transform target) => RP_followTarget.Value = target;
        public void SetShaking(bool isShaking) => RP_isShaking.Value = isShaking;
        public void SetShakeOffset(Vector3 offset) => RP_shakeOffset.Value = offset;

        
    }
}