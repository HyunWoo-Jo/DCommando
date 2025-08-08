using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core;
using Game.Data;

namespace Game.Services {
    public class UIService : IUIService, IDisposable {
        [Inject] private IAddressableService<UIName, GameObject> _addressableService;

        // UI 정보 관리
        private readonly Dictionary<UIName, UIInfoData> _uiInfoDict = new();

        // 인스턴스 관리
        private readonly Dictionary<UIName, int> _instanceRefCount = new();    // 인스턴스 참조 카운터
        private readonly List<GameObject> _activeInstances = new();           // 활성 인스턴스 추적

        #region 초기화 Zenject에서 관리
        [Inject]
        public void Initialize() {
            try {
                LoadUIInfoFromCSV();
                RegisterAddressableKeys();

                GameDebug.Log($"UI Service 초기화 완료: UI {_uiInfoDict.Count}개");
            } catch (Exception e) {
                GameDebug.LogError($"UI Service 초기화 실패: {e.Message}");
            }
        }

        public void Dispose() {
            _addressableService.UnloadAll();
        }

        private void LoadUIInfoFromCSV() {
            var csvData = CSVReader.ReadToMultiColumnDictionary("AddressKey/UIAddressKey");

            foreach (var row in csvData) {
                try {
                    // CSV / UI Name, UI Type, AddressKey
                    var uiNameStr = row.Key;
                    var columns = row.Value;
                    if (columns.Count != 3) {
                        GameDebug.LogError($"UI CSV 행 형식 오류: {uiNameStr}");
                        continue;
                    }

                    // UIName 파싱
                    if (!Enum.TryParse<UIName>(uiNameStr, out var uiName)) {
                        GameDebug.LogError($"알 수 없는 UIName: {uiNameStr}");
                        continue;
                    }

                    // UIType 파싱
                    if (!Enum.TryParse<UIType>(columns[1], out var uiType)) {
                        GameDebug.LogError($"알 수 없는 UIType: {columns[1]} for {uiNameStr}");
                        continue;
                    }

                    // AddressKey
                    var addressKey = columns[2];

                    var uiInfo = new UIInfoData(uiName, addressKey, uiType);
                    _uiInfoDict[uiName] = uiInfo;
                } catch (Exception e) {
                    GameDebug.LogError($"UI 정보 파싱 실패: {row.Key}, 에러: {e.Message}");
                }
            }
        }

        private void RegisterAddressableKeys() {
            var addressKeyMap = _uiInfoDict.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.addressableKey
            );

            _addressableService.RegisterAddressKeys(addressKeyMap);
            GameDebug.Log($"UI 주소 키 {addressKeyMap.Count}개 등록 완료");
        }
        #endregion

        #region 참조 카운트 관리
        private void IncreaseRefCount(UIName uiName) {
            if (!_instanceRefCount.ContainsKey(uiName))
                _instanceRefCount[uiName] = 0;
            _instanceRefCount[uiName]++;
        }

        private void DecreaseRefCount(UIName uiName) {
            if (!_instanceRefCount.ContainsKey(uiName)) return;

            _instanceRefCount[uiName]--;
            if (_instanceRefCount[uiName] <= 0) {
                _instanceRefCount.Remove(uiName);
                _addressableService.UnloadAsset(uiName);
            }
        }
        #endregion

        #region UI 로드 API

        /// <summary>
        /// UI를 Component 타입으로 로드합니다
        /// </summary>
        public async UniTask<T> LoadUIAsync<T>(UIName uiName) where T : Component {
            return await LoadDirectUIAsync<T>(uiName);
        }

        /// <summary>
        /// UI를 GameObject로 로드합니다
        /// </summary>
        public async UniTask<GameObject> LoadUIGameObjectAsync(UIName uiName) {
            return await LoadDirectUIGameObjectAsync(uiName);
        }

        /// <summary>
        /// UI 프리팹을 로드합니다 (인스턴스화 안함)
        /// </summary>
        public async UniTask<GameObject> LoadUIPrefabAsync(UIName uiName) {
            var prefab = await _addressableService.LoadAssetAsync(uiName);

            if (prefab != null) {
                GameDebug.Log($"UI 프리팹 로드 완료: {uiName}");
            } else {
                GameDebug.LogError($"UI 프리팹 로드 실패: {uiName}");
            }

            return prefab;
        }

        private async UniTask<T> LoadDirectUIAsync<T>(UIName uiName) where T : Component {
            var gameObject = await LoadDirectUIGameObjectAsync(uiName);
            if (gameObject == null) return null;

            var component = gameObject.GetComponent<T>();
            if (component == null) {
                GameDebug.LogError($"Component 누락: {typeof(T).Name} in {uiName}");
                ReleaseUI(gameObject);
                return null;
            }

            return component;
        }

        private async UniTask<GameObject> LoadDirectUIGameObjectAsync(UIName uiName) {
            GameObject instance = null;
            try {
                // AddressableService를 통해 프리팹 로드
                var prefab = await _addressableService.LoadAssetAsync(uiName);
                if (prefab == null) {
                    GameDebug.LogError($"UI 프리팹 로드 실패: {uiName}");
                    return null;
                }

                // DI 주입과 함께 인스턴스화
                instance = DIHelper.InstantiateWithInjection(prefab);

                // 참조 카운트 증가 및 인스턴스 추적
                IncreaseRefCount(uiName);
                _activeInstances.Add(instance);

                GameDebug.Log($"UI 인스턴스 생성 완료: {uiName}");
                return instance;
            } catch (Exception e) {
                GameDebug.LogError($"UI 로드 실패: {uiName}, 에러: {e.Message}");

                // 실패 시 자원 정리
                if (instance != null) {
                    ReleaseUI(instance);
                }
                throw;
            }
        }

        #endregion

        #region UI 해제

        /// <summary>
        /// UI 오브젝트를 파괴합니다
        /// </summary>
        public void ReleaseUI(GameObject obj) {
            if (obj != null) {
                // 활성 인스턴스 목록에서 제거
                _activeInstances.Remove(obj);

                // UIName 찾아서 참조 카운트 감소
                var uiName = FindUINameByInstance(obj);
                if (uiName.HasValue) {
                    DecreaseRefCount(uiName.Value);
                }

                UnityEngine.Object.Destroy(obj);
                GameDebug.Log($"UI 인스턴스 해제: {obj.name}");
            }
        }

        /// <summary>
        /// UIName을 지정해 참조 카운트를 감소시킵니다
        /// </summary>
        public void ReleaseUI(UIName uiName) {
            DecreaseRefCount(uiName);
            GameDebug.Log($"UI 참조 해제: {uiName}");
        }

        /// <summary>
        /// 모든 UI 인스턴스와 자원을 해제합니다
        /// </summary>
        public void ReleaseAll() {
            // 모든 활성 인스턴스 파괴
            foreach (var instance in new List<GameObject>(_activeInstances)) {
                if (instance != null) {
                    UnityEngine.Object.Destroy(instance);
                }
            }

            _activeInstances.Clear();
            _instanceRefCount.Clear();

            // AddressableService의 모든 UI 에셋 언로드
            _addressableService.UnloadAll();

            GameDebug.Log("모든 UI 자원 해제 완료");
        }

        #endregion

        #region UI 정보 조회

        /// <summary>
        /// UIName으로 UI 정보를 가져옵니다
        /// </summary>
        public UIInfoData GetUIInfo(UIName uiName) {
            _uiInfoDict.TryGetValue(uiName, out var uiInfo);
            return uiInfo;
        }

        /// <summary>
        /// UIName으로 AddressableKey를 가져옵니다
        /// </summary>
        public string GetAddressableKey(UIName uiName) {
            var uiInfo = GetUIInfo(uiName);
            return uiInfo?.addressableKey ?? string.Empty;
        }

        /// <summary>
        /// UIName으로 UIType을 가져옵니다
        /// </summary>
        public UIType GetUIType(UIName uiName) {
            var uiInfo = GetUIInfo(uiName);
            return uiInfo?.uiType ?? UIType.Screen;
        }

        /// <summary>
        /// 특정 UIType에 해당하는 모든 UI 목록을 가져옵니다
        /// </summary>
        public List<UIInfoData> GetUIsByType(UIType uiType) {
            return _uiInfoDict.Values
                .Where(info => info.uiType == uiType)
                .ToList();
        }

        /// <summary>
        /// 모든 UI 정보를 가져옵니다
        /// </summary>
        public IReadOnlyDictionary<UIName, UIInfoData> GetAllUIInfo() {
            return _uiInfoDict;
        }

        /// <summary>
        /// UI가 존재하는지 확인합니다
        /// </summary>
        public bool HasUI(UIName uiName) {
            return _uiInfoDict.ContainsKey(uiName);
        }

        #endregion

        #region 상태 조회

        /// <summary>
        /// 로드된 UI 프리팹이 있는지 확인합니다
        /// </summary>
        public bool HasLoadedPrefab(UIName uiName) {
            return _addressableService.HasAsset(uiName);
        }

        /// <summary>
        /// UI 주소 키가 등록되어 있는지 확인합니다
        /// </summary>
        public bool HasUIKey(UIName uiName) {
            return _addressableService.HasAddressKey(uiName);
        }

        /// <summary>
        /// 현재 활성 인스턴스 개수를 반환합니다
        /// </summary>
        public int GetActiveInstanceCount() {
            return _activeInstances.Count;
        }

        /// <summary>
        /// 특정 UI의 참조 카운트를 반환합니다
        /// </summary>
        public int GetRefCount(UIName uiName) {
            return _instanceRefCount.GetValueOrDefault(uiName, 0);
        }

        #endregion

        #region 유틸리티

        private UIName? FindUINameByInstance(GameObject instance) {
            // 인스턴스 이름이나 태그를 통해 UIName을 찾는 로직
            var instanceName = instance.name.Replace("(Clone)", "");

            foreach (UIName uiName in System.Enum.GetValues(typeof(UIName))) {
                if (instanceName.Contains(uiName.ToString())) {
                    return uiName;
                }
            }

            return null;
        }

  

        #endregion
    }
}