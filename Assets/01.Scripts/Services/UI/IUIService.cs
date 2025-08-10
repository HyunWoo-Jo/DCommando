using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Game.Data;
using Game.Core;

namespace Game.Services {
    public interface IUIService {
        #region UI 로드
        /// <summary>
        /// UI를 Component 타입으로 로드합니다
        /// </summary>
        UniTask<T> LoadUIAsync<T>(UIName uiName) where T : Component;

        /// <summary>
        /// UI를 동기로 로드
        /// </summary>

        GameObject LoadUIGameObject(UIName uiName);

        /// <summary>
        /// UI를 GameObject로 로드합니다
        /// </summary>
        UniTask<GameObject> LoadUIGameObjectAsync(UIName uiName);

        /// <summary>
        /// UI 프리팹을 로드합니다 (인스턴스화 안함)
        /// </summary>
        UniTask<GameObject> LoadUIPrefabAsync(UIName uiName);
        #endregion

        #region UI 해제
        /// <summary>
        /// UI 오브젝트를 파괴합니다
        /// </summary>
        void ReleaseUI(GameObject obj);

        /// <summary>
        /// UIName을 지정해 참조 카운트를 감소시킵니다
        /// </summary>
        void ReleaseUI(UIName uiName);

        /// <summary>
        /// 모든 UI 인스턴스와 자원을 해제합니다
        /// </summary>
        void ReleaseAll();
        #endregion

        #region UI 정보 조회
        /// <summary>
        /// UIName으로 UI 정보를 가져옵니다
        /// </summary>
        UIInfoData GetUIInfo(UIName uiName);

        /// <summary>
        /// UIName으로 AddressableKey를 가져옵니다
        /// </summary>
        string GetAddressableKey(UIName uiName);

        /// <summary>
        /// UIName으로 UIType을 가져옵니다
        /// </summary>
        UIType GetUIType(UIName uiName);

        /// <summary>
        /// 특정 UIType에 해당하는 모든 UI 목록을 가져옵니다
        /// </summary>
        List<UIInfoData> GetUIsByType(UIType uiType);

        /// <summary>
        /// 모든 UI 정보를 가져옵니다
        /// </summary>
        IReadOnlyDictionary<UIName, UIInfoData> GetAllUIInfo();

        /// <summary>
        /// UI가 존재하는지 확인합니다
        /// </summary>
        bool HasUI(UIName uiName);
        #endregion

        #region 상태 조회
        /// <summary>
        /// 로드된 UI 프리팹이 있는지 확인합니다
        /// </summary>
        bool HasLoadedPrefab(UIName uiName);

        /// <summary>
        /// UI 주소 키가 등록되어 있는지 확인합니다
        /// </summary>
        bool HasUIKey(UIName uiName);

        /// <summary>
        /// 현재 활성 인스턴스 개수를 반환합니다
        /// </summary>
        int GetActiveInstanceCount();

        /// <summary>
        /// 특정 UI의 참조 카운트를 반환합니다
        /// </summary>
        int GetRefCount(UIName uiName);
        #endregion
    }
}