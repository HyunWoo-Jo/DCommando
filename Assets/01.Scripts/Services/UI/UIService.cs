using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Game.Core;
using Game.Data;
using System;

using Object = UnityEngine.Object;
using System.IO;

namespace Game.Services {
    public class UIService : IUIService {

        private readonly Dictionary<UIName, AsyncOperationHandle> _loadedAssets = new(); // Addressables 로드된 에셋 핸들을 저장                                 
        private readonly Dictionary<UIName, int> _assetRefCount = new();    // Addressable 참조 카운터
        private readonly Dictionary<UIName, string> _uiAddressableKeys = new(); // UIName -> AddressableKey 매핑

        #region 초기화
        /// <summary>
        /// UIName과 AddressableKey 매핑을 설정합니다.
        /// </summary>
        public void Initialize(Dictionary<UIName, string> uiKeyMappings) {
            _uiAddressableKeys.Clear();
            foreach (var mapping in uiKeyMappings) {
                _uiAddressableKeys[mapping.Key] = mapping.Value;
            }
        }
        #endregion

        #region 참조 카운트 관리
        // 참조 카운트 증가
        private void IncreaseRefCount(UIName uiName) {
            if (!_assetRefCount.ContainsKey(uiName)) _assetRefCount[uiName] = 0;
            _assetRefCount[uiName]++;
        }

        // 참조 카운트 감소 및 0이 되면 Addressables 해제
        private void DecreaseRefCount(UIName uiName) {
            if (!_assetRefCount.ContainsKey(uiName)) return;

            _assetRefCount[uiName]--;
            if (_assetRefCount[uiName] > 0) return; // 아직 참조가 남아있음

            if (_loadedAssets.TryGetValue(uiName, out var handle) && handle.IsValid())
                Addressables.Release(handle);

            _loadedAssets.Remove(uiName);
            _assetRefCount.Remove(uiName);
        }
        #endregion

        #region 공개 로드 API

        // Component 단위 로드
        public async UniTask<T> LoadUIAsync<T>(UIName uiName) where T : Component {
            return await LoadDirectUIAsync<T>(uiName);
        }

        // GameObject 단위 로드
        public async UniTask<GameObject> LoadUIGameObjectAsync(UIName uiName) {
            return await LoadDirectUIGameObjectAsync(uiName);
        }
        public async UniTask<GameObject> LoadUIPrefabAsync(UIName uiName) {
            return await LoadPrefabAync(uiName);
        }
        #endregion

        #region 직접 로드

        private async UniTask<T> LoadDirectUIAsync<T>(UIName uiName) where T : Component {
            var gameObject = await LoadDirectUIGameObjectAsync(uiName);
            if (gameObject == null) return null;

            var component = gameObject.GetComponent<T>();
            if (component == null) {
                GameDebug.LogError($"Component 누락: {typeof(T).Name} in {uiName}");
                Object.Destroy(gameObject);
                DecreaseRefCount(uiName);
                return null;
            }

            return component;
        }

        private async UniTask<GameObject> LoadPrefabAync(UIName uiName) {
            if (!_uiAddressableKeys.TryGetValue(uiName, out var addressableKey)) {
                GameDebug.LogError($"UIName에 대한 AddressableKey를 찾을 수 없습니다: {uiName}");
                return null;
            }
            AsyncOperationHandle<GameObject> handle = default;
            GameObject prefab = null;
            try {
                handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
                prefab = await handle.ToUniTask();
                if (prefab == null)
                    throw new System.IO.FileNotFoundException($"Prefab not found: {addressableKey} for {uiName}");

                _loadedAssets[uiName] = handle;
                return prefab;
            }catch(Exception e) {
                if (handle.IsValid())
                    GameDebug.LogError($"UI 로드 실패: {uiName} ({addressableKey})\n{e}");
                throw;
            }
        }

        private async UniTask<GameObject> LoadDirectUIGameObjectAsync(UIName uiName) {
            GameObject instance = null;
            try {
                var prefab = await LoadPrefabAync(uiName);
                instance = DIHelper.InstantiateWithInjection(prefab);
                IncreaseRefCount(uiName);

                return instance;
            } catch (Exception e) {
                _loadedAssets.Remove(uiName);
                // 실패 시 자원 정리
                if (instance != null)
                    Object.Destroy(instance);
                throw;
            }
        }

        

        #endregion

        #region 해제

        /// <summary>
        /// UI 오브젝트를 파괴합니다.
        /// </summary>
        public void ReleaseUI(GameObject obj) {
            if (obj != null) {
                Object.Destroy(obj);
            }
        }

        /// <summary>
        /// UIName을 지정해 참조 카운트를 감소시킵니다.
        /// </summary>
        public void ReleaseUI(UIName uiName) {
            DecreaseRefCount(uiName);
        }

        /// <summary>
        /// 모든 Addressables 자원을 일괄 해제합니다.
        /// </summary>
        public void ReleaseAll() {
            // 남아 있는 Addressables 참조 강제 0으로 설정 후 해제
            foreach (var uiName in new List<UIName>(_assetRefCount.Keys)) {
                _assetRefCount[uiName] = 0;
                DecreaseRefCount(uiName);
            }
        }

        #endregion
    }
}