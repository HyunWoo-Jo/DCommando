using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Game.Services
{
    public interface IUIService
    {
        UniTask<T> LoadUIAsync<T>(string addressableKey) where T : Component;
        UniTask<GameObject> LoadUIGameObjectAsync(string addressableKey);
        void ReleaseUI(GameObject uiObject);
        void ReleaseUI(string addressableKey);
    }
}