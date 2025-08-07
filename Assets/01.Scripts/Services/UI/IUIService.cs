using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using System.Collections.Generic;

namespace Game.Services
{
    public interface IUIService
    {
        /// <summary>
        /// UIName과 AddressableKey 매핑을 설정합니다.
        /// </summary>
        void Initialize(Dictionary<UIName, string> uiKeyMappings);

        /// <summary>
        /// Component 단위로 UI를 로드합니다.
        /// </summary>
        UniTask<T> LoadUIAsync<T>(UIName uiName) where T : Component;

        /// <summary>
        /// GameObject 단위로 UI를 로드합니다.
        /// </summary>
        UniTask<GameObject> LoadUIGameObjectAsync(UIName uiName);
        /// <summary>
        /// UI Prefab을 로드
        /// </summary>
        UniTask<GameObject> LoadUIPrefabAsync(UIName uiName);

        /// <summary>
        /// UI 오브젝트를 파괴합니다.
        /// </summary>
        void ReleaseUI(GameObject uiObject);

        /// <summary>
        /// UIName을 지정해 참조 카운트를 감소시킵니다.
        /// </summary>
        void ReleaseUI(UIName uiName);

        /// <summary>
        /// 모든 Addressables 자원을 일괄 해제합니다.
        /// </summary>
        void ReleaseAll();
    }
}