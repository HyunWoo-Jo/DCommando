using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Game.Services {
    public interface IAddressableService<TKey, TAsset> where TAsset : UnityEngine.Object {
        /// <summary>
        /// 주소 키를 등록합니다
        /// </summary>
        void RegisterAddressKey(TKey key, string addressKey);

        /// <summary>
        /// 주소 키 맵을 일괄 등록합니다
        /// </summary>
        void RegisterAddressKeys(IReadOnlyDictionary<TKey, string> addressMap);

        /// <summary>
        /// 에셋을 비동기로 로드합니다
        /// </summary>
        UniTask<TAsset> LoadAssetAsync(TKey key);

        /// <summary>
        /// 해당 에셋이 로드되어 있는지 확인합니다
        /// </summary>
        bool HasAsset(TKey key);

        /// <summary>
        /// 해당 주소 키가 등록되어 있는지 확인합니다
        /// </summary>
        bool HasAddressKey(TKey key);

        /// <summary>
        /// 특정 에셋을 언로드합니다
        /// </summary>
        void UnloadAsset(TKey key);

        /// <summary>
        /// 모든 에셋을 언로드합니다
        /// </summary>
        void UnloadAll();

        /// <summary>
        /// 현재 로드된 에셋 개수를 반환합니다
        /// </summary>
        int GetLoadedAssetCount();

        /// <summary>
        /// 현재 로드된 에셋 키 목록을 반환합니다
        /// </summary>
        IReadOnlyCollection<TKey> GetLoadedAssetKeys();

        /// <summary>
        /// 로드된 에셋을 즉시 반환합니다 (로드되지 않은 경우 null)
        /// </summary>
        TAsset GetLoadedAsset(TKey key);
    }
}