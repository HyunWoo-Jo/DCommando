using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Core;
using DG.Tweening;
using System.Threading;
using System;

namespace Game.Services {
    public class CameraService : ICameraService {
        private Camera _mainCamera;

        public Camera MainCamera => SearchMainCamera();

        private Tween _zoomTween;

        private Camera SearchMainCamera() {
            if (_mainCamera == null) {
                _mainCamera = Camera.main;
                if (_mainCamera == null) {
                    Debug.LogError("메인 카메라 X");
                    return null;
                }
            }
            return _mainCamera;
        }

        public void SetCameraPosition(Vector3 position) {
            if (_mainCamera != null)
                _mainCamera.transform.position = position;
        }

        public void SetCameraZoom(float zoom) {
            if (_mainCamera != null) {
                if (_mainCamera.orthographic)
                    _mainCamera.orthographicSize = zoom;
                else
                    _mainCamera.fieldOfView = zoom;
            }
        }

        public async UniTask MoveCameraToAsync(Vector3 position, float duration) {
            if (_mainCamera == null) return;

            await _mainCamera.transform
                .DOMove(position, duration)
                .SetEase(Ease.OutQuart)
                .AsyncWaitForCompletion();
        }
        
        public async UniTask ZoomCameraToAsync(float zoom, float duration) {
            if (_mainCamera == null) return;
            _zoomTween?.Kill(); // 기존 Tween 정리

            if (_mainCamera.orthographic) {
                _zoomTween = DOTween.To(
                    () => _mainCamera.orthographicSize,
                    value => _mainCamera.orthographicSize = value,
                    zoom,
                    duration
                ).SetEase(Ease.OutQuart);
            } else {
                _zoomTween = DOTween.To(
                    () => _mainCamera.fieldOfView,
                    value => _mainCamera.fieldOfView = value,
                    zoom,
                    duration
                ).SetEase(Ease.OutQuart);
            }
            await _zoomTween.AsyncWaitForCompletion();
        }

        public async UniTask ShakeAsync(float duration, float strength = 1f) {
            if (_mainCamera == null) return;

            await _mainCamera.transform
                .DOShakePosition(duration, strength)
                .SetEase(Ease.OutQuart)
                .AsyncWaitForCompletion();
        }

        public void StopAllTweens() {
            _mainCamera?.transform.DOKill();
            _zoomTween?.Kill();
        }
    }
}