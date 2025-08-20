using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core;

namespace Game.Services {
    public class AddressableService<TKey, TAsset> : IAddressableService<TKey, TAsset>, IInitializable, IDisposable
        where TAsset : UnityEngine.Object {
        private readonly Dictionary<TKey, TAsset> _loadedAssets = new();
        private readonly Dictionary<TKey, AsyncOperationHandle<TAsset>> _handles = new();
        private readonly Dictionary<TKey, string> _keyToAddressMap = new();

        #region 초기화

        public void Initialize() {
            GameDebug.Log($"AddressableService<{typeof(TKey).Name}, {typeof(TAsset).Name}> 초기화 완료");
        }

        public void Dispose() {
            UnloadAll();
        }
        #endregion

        public void RegisterAddressKey(TKey key, string addressKey) {
            _keyToAddressMap[key] = addressKey;
        }

        public void RegisterAddressKeys(IReadOnlyDictionary<TKey, string> addressMap) {
            
            foreach (var kvp in addressMap) {
                _keyToAddressMap[kvp.Key] = kvp.Value;
            }
        }

        public TAsset LoadAsset(TKey key) {
            if (_loadedAssets.TryGetValue(key, out TAsset cachedAsset)) {
                return cachedAsset;
            }
            if (!_keyToAddressMap.TryGetValue(key, out string addressKey)) {
                GameDebug.LogError($"주소 키를 찾을 수 없음: {key} ({typeof(TAsset).Name})");
                return null;
            }
            try {
                // Addressables로 로드
                var handle = Addressables.LoadAssetAsync<TAsset>(addressKey);
                
                var asset = handle.WaitForCompletion();

                if (asset != null) {
                    _loadedAssets[key] = asset;
                    _handles[key] = handle;
                    GameDebug.Log($"에셋 로드 완료: {key} ({typeof(TAsset).Name})");
                } else {
                    GameDebug.LogError($"에셋 로드 결과가 null: {key}");
                }

                return asset;
            } catch (System.Exception e) {
                GameDebug.LogError($"에셋 로드 실패: {key}, 에러: {e.Message}");
                return null;
            }
        }

        public async UniTask<TAsset> LoadAssetAsync(TKey key) {
            // 이미 로드된 경우 캐시에서 반환
            if (_loadedAssets.TryGetValue(key, out TAsset cachedAsset)) {
                return cachedAsset;
            }

            // 주소 키 찾기
            if (!_keyToAddressMap.TryGetValue(key, out string addressKey)) {
                GameDebug.LogError($"주소 키를 찾을 수 없음: {key} ({typeof(TAsset).Name})");
                return null;
            }

            try {
                // Addressables로 로드
                var handle = Addressables.LoadAssetAsync<TAsset>(addressKey);
                var asset = await handle.ToUniTask();

                if (asset != null) {
                    _loadedAssets[key] = asset;
                    _handles[key] = handle;
                    GameDebug.Log($"에셋 로드 완료: {key} ({typeof(TAsset).Name})");
                } else {
                    GameDebug.LogError($"에셋 로드 결과가 null: {key}");
                }

                return asset;
            } catch (System.Exception e) {
                GameDebug.LogError($"에셋 로드 실패: {key}, 에러: {e.Message}");
                return null;
            }
        }

        public bool HasAsset(TKey key) {
            return _loadedAssets.ContainsKey(key);
        }

        public bool HasAddressKey(TKey key) {
            return _keyToAddressMap.ContainsKey(key);
        }

        public void UnloadAsset(TKey key) {
            if (_loadedAssets.Remove(key)) {
                if (_handles.TryGetValue(key, out var handle)) {
                    if (handle.IsValid()) Addressables.Release(handle);
                    _handles.Remove(key);
                    GameDebug.Log($"에셋 언로드: {key} ({typeof(TAsset).Name})");
                }
            }
        }

        public void UnloadAll() {
            foreach (var handle in _handles.Values) {
                if (handle.IsValid()) Addressables.Release(handle);
            }

            _loadedAssets.Clear();
            _handles.Clear();
            GameDebug.Log($"모든 {typeof(TAsset).Name} 에셋 언로드 완료");
        }

        public int GetLoadedAssetCount() {
            return _loadedAssets.Count;
        }

        public IReadOnlyCollection<TKey> GetLoadedAssetKeys() {
            return _loadedAssets.Keys;
        }

        public TAsset GetLoadedAsset(TKey key) {
            _loadedAssets.TryGetValue(key, out TAsset asset);
            return asset;
        }
    }
}