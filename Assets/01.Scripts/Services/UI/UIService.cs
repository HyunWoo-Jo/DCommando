using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Core;

namespace Game.Services
{
    public class UIService : IUIService
    {
        private readonly Dictionary<string, AsyncOperationHandle> _loadedAssets = new();
        
        public async UniTask<T> LoadUIAsync<T>(string addressableKey) where T : Component
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                var prefab = await handle.ToUniTask();
                
                if (prefab == null)
                {
                    GameDebug.LogError($"UI Prefab을 찾을 수 없습니다: {addressableKey}");
                    return null;
                }
                var instance = DIHelper.InstantiateWithInjection(prefab);
                var component = instance.GetComponent<T>();
                
                if (component == null)
                {
                    GameDebug.LogError($"UI Component를 찾을 수 없습니다: {typeof(T).Name} in {addressableKey}");
                    Object.Destroy(instance);
                    return null;
                }
                
                // Handle 저장
                _loadedAssets[addressableKey] = handle;
                
                return component;
            }
            catch (System.Exception e)
            {
                GameDebug.LogError($"UI 로드 실패: {addressableKey}, Error: {e.Message}");
                return null;
            }
        }
        
        public async UniTask<GameObject> LoadUIGameObjectAsync(string addressableKey)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                var prefab = await handle.ToUniTask();
                
                if (prefab == null)
                {
                    GameDebug.LogError($"UI Prefab을 찾을 수 없습니다: {addressableKey}");
                    return null;
                }

                var instance = DIHelper.InstantiateWithInjection(prefab);
                
                // Handle 저장
                _loadedAssets[addressableKey] = handle;
                
                return instance;
            }
            catch (System.Exception e)
            {
                GameDebug.LogError($"UI 로드 실패: {addressableKey}, Error: {e.Message}");
                return null;
            }
        }
        
        public void ReleaseUI(GameObject uiObject)
        {
            if (uiObject != null)
            {
                Object.Destroy(uiObject);
            }
        }
        
        public void ReleaseUI(string addressableKey)
        {
            if (_loadedAssets.TryGetValue(addressableKey, out var handle))
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
                _loadedAssets.Remove(addressableKey);
            }
        }
        
        public void ReleaseAll()
        {
            foreach (var kvp in _loadedAssets)
            {
                if (kvp.Value.IsValid())
                {
                    Addressables.Release(kvp.Value);
                }
            }
            _loadedAssets.Clear();
        }
    }
}