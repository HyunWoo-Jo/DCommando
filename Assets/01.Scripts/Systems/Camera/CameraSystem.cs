using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Model;
using Game.Policies;
using Game.Services;
using R3;
using System.Threading;
using System;
using UnityEngine;
using Zenject;
using Game.Core.Event;
using UnityEngine.PlayerLoop;

namespace Game.Systems
{
    public class CameraSystem : IInitializable, IDisposable
    {
        [Inject] private CameraModel _cameraModel;
        [Inject] private ICameraService _cameraService;
        [Inject] private ICameraPolicy _cameraPolicy;
        [Inject] private SO_CameraConfig _config;
        [Inject] private IUpdater _updater;

        private CompositeDisposable _disposables = new();

        #region Zenject 관리
        public void Initialize() {
            // 초기 줌 설정
            _cameraModel.SetCurrentZoom(_config.ZoomData.defaultZoom);
            _cameraService.SetCameraZoom(_config.ZoomData.defaultZoom);

            SubscribeToEvents();
        }
        public void Dispose() {
            _disposables?.Dispose();
        }

        #endregion
        private void SubscribeToEvents() {
            // 카메라 명령 이벤트 구독
            // Updata 등록
            _updater.OnLateUpdate
                .Where(_ => _cameraPolicy.CanFollow(_cameraModel.RORP_FollowTarget.CurrentValue) && _cameraModel.RORP_IsFollowing.CurrentValue)
                .Select(_ => _cameraModel.RORP_FollowTarget.CurrentValue)// target이 가능하고 추적 중일때만 호출
                .Subscribe(UpdateFollowTarget)
                .AddTo(_disposables);

            // 이동
            EventBus.Subscribe<CameraMoveToEvent>(OnCameraMoveToEvent).AddTo(_disposables);
            EventBus.Subscribe<CameraSetPositionEvent>(OnCameraSetPositionEvent).AddTo(_disposables);
            // 줌
            EventBus.Subscribe<CameraZoomToEvent>(OnCameraZoomToEvent).AddTo(_disposables);
            EventBus.Subscribe<CameraSetZoomEvent>(OnCameraSetZoomEvent).AddTo(_disposables);
            // 쉐이크
            EventBus.Subscribe<CameraShakeEvent>(OnCameraShakeEvent).AddTo(_disposables);
            // Follow
            EventBus.Subscribe<CameraFollowTargetEvent>(OnCameraFollowTargetEvent).AddTo(_disposables);
            EventBus.Subscribe<CameraStopFollowEvent>(OnCameraStopFollowEvent).AddTo(_disposables);

            // Stop
            EventBus.Subscribe<CameraStopAllAnimationsEvent>(OnCameraStopAllAnimationsEvent).AddTo(_disposables);
        }

        #region 핸들
        private async void OnCameraMoveToEvent(CameraMoveToEvent evt) {
            await MoveCameraToAsync(evt.position, evt.duration);
        }

        private void OnCameraSetPositionEvent(CameraSetPositionEvent evt) {
            SetCameraPosition(evt.position);
        }

        private async void OnCameraZoomToEvent(CameraZoomToEvent evt) {
            await ZoomToAsync(evt.zoom, evt.duration);
        }

        private void OnCameraSetZoomEvent(CameraSetZoomEvent evt) {
            SetZoom(evt.zoom);
        }

        private async void OnCameraShakeEvent(CameraShakeEvent evt) {
            await ShakeAsync(evt.intensity, evt.duration);
        }

        private void OnCameraFollowTargetEvent(CameraFollowTargetEvent evt) {
            SetFollowTarget(evt.target);
        }

        private void OnCameraStopFollowEvent(CameraStopFollowEvent evt) {
            StopFollowing();
        }

        private void OnCameraStopAllAnimationsEvent(CameraStopAllAnimationsEvent evt) {
            StopAllAnimations();
        }
        #endregion

        #region Core
        /// <summary>
        /// Updater 를 통해 호출
        /// </summary>
        private void UpdateFollowTarget(Transform followTarget) {
            Vector3 targetPosition = followTarget.position + _config.FollowData.offset;
            _cameraModel.SetTargetPosition(targetPosition);
        }

        #region event에 연결된 함수
        private void SetFollowTarget(Transform target) {
            if (_cameraPolicy.CanFollow(target)) {
                _cameraModel.SetFollowTarget(target);
                _cameraModel.SetFollowing(true);
                EventBus.Publish(new CameraFollowStartedEvent(target));
            }
        }

        private void StopFollowing() {
            _cameraModel.SetFollowing(false);
            EventBus.Publish(new CameraFollowStoppedEvent());
        }

        private void SetZoom(float zoom) {
            var currentZoom = _cameraModel.RORP_CurrentZoom.CurrentValue;
            if (_cameraPolicy.CanZoom(currentZoom, zoom, _config.ZoomData.minZoom, _config.ZoomData.maxZoom)) {
                var clampedZoom = Mathf.Clamp(zoom, _config.ZoomData.minZoom, _config.ZoomData.maxZoom);
                _cameraModel.SetCurrentZoom(clampedZoom);
            }
        }

        private async UniTask MoveCameraToAsync(Vector3 position, float duration) {
            // 일시적으로 Follow 중지
            bool wasFollowing = _cameraModel.RORP_IsFollowing.CurrentValue;
            if (wasFollowing) {
                _cameraModel.SetFollowing(false);
            }
            await _cameraService.MoveCameraToAsync(position, duration);
            EventBus.Publish(new CameraMovedEvent(position));
            if (wasFollowing) {
                _cameraModel.SetFollowing(true);
            }
        }

        private async UniTask ZoomToAsync(float zoom, float duration) {
            var currentZoom = _cameraModel.RORP_CurrentZoom.CurrentValue;
            if (!_cameraPolicy.CanZoom(currentZoom, zoom, _config.ZoomData.minZoom, _config.ZoomData.maxZoom))
                return;

            var clampedZoom = Mathf.Clamp(zoom, _config.ZoomData.minZoom, _config.ZoomData.maxZoom);

            await _cameraService.ZoomCameraToAsync(clampedZoom, duration);
            _cameraModel.SetCurrentZoom(clampedZoom);
        }

        private async UniTask ShakeAsync(float intensity, float duration) {
            if (!_cameraPolicy.CanShake()) return;


            _cameraModel.SetShaking(true);
            EventBus.Publish(new CameraShakeStartedEvent());

            try {
                await _cameraService.ShakeAsync(duration, intensity);
            } catch (OperationCanceledException) {
                // 취소
            } finally {
                _cameraModel.SetShaking(false);
                EventBus.Publish(new CameraShakeEndedEvent());
            }
        }

        private void SetCameraPosition(Vector3 position) {
            _cameraService.SetCameraPosition(position);
            EventBus.Publish(new CameraMovedEvent(position));
        }

        private void StopAllAnimations() {
            _cameraService.StopAllTweens();
        }
        #endregion
        #endregion
    }
}
