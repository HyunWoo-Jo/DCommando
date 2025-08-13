using Game.Core;
using Game.Data;
using UnityEngine;
using Zenject;

namespace Game.Services
{
    /// <summary>
    /// Service 계층 DI 바인딩
    /// 네트워크, 저장, SDK 연동
    /// Data, Policy 계층 참조 가능
    /// </summary>
    public class ServiceInstaller : MonoInstaller{
        [SerializeField] private SceneName _sceneName;
        public override void InstallBindings() {


            switch (_sceneName) {
                case SceneName.MainScene:
                break;
                case SceneName.PlayScene:
                BindStageService();
                break;
            }

        }


        private void BindStageService() {
            Container.BindInterfacesAndSelfTo<StageService>().AsCached();

            Container.Bind<IAddressableService<EnemyName, GameObject>>()
             .To<AddressableService<EnemyName, GameObject>>().AsCached();

            Container.Bind<IAddressableService<StageName, SO_StageConfig>>()
             .To<AddressableService<StageName, SO_StageConfig>>().AsCached();
        }
    }
}