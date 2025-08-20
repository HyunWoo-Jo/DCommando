using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Services
{
    public interface ICameraService
    {
        Camera MainCamera { get; }
        void SetCameraPosition(Vector3 position);
        void SetCameraZoom(float zoom);
        UniTask MoveCameraToAsync(Vector3 position, float duration);
        UniTask ZoomCameraToAsync(float zoom, float duration);
        UniTask ShakeAsync(float duration, float strength = 1f);
        void StopAllTweens();
    }
}
