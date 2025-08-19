using UnityEngine;
using Zenject;
using UnityEngine.AddressableAssets;
namespace Game.Core
{
    /// <summary>
    /// DI 유틸 Addressables를 사용한 로드
    /// </summary>
    public static class DiContainerAddressablesExtensions
    {
        public static ScopeConcreteIdArgConditionCopyNonLazyBinder BindAddressable<T>(this DiContainer container, string key) where T : UnityEngine.Object {
            return container.Bind<T>().FromMethod(_ => {
                var handle = Addressables.LoadAssetAsync<T>(key);
                handle.WaitForCompletion();
                return handle.Result;
            });
        }
    }
}
