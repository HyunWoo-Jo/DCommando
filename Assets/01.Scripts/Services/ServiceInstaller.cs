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
            BindNetworkServices();
            BindStorageServices();
            BindAddressables();
            BindExternalServices();

            switch (_sceneName) {
                case SceneName.MainLobby:
                break;
                case SceneName.Play:
                break;
            }
            GameDebug.Log(GetType().Name + " Bind 완료");
        }

        /// <summary>
        /// 네트워크 서비스 바인딩
        /// </summary>
        private void BindNetworkServices() {
            Container.Bind<INetworkService>().To<FirebaseService>().AsSingle().NonLazy();

            Container.Bind<ICrystalService>().To<CrystalService>().AsSingle();
            Container.Bind<IGoldService>().To<GoldService>().AsSingle();
            Container.BindInterfacesAndSelfTo<EquipService>().AsSingle().NonLazy(); // firebase + Addressables 
        }


        /// <summary>
        /// 저장 서비스 바인딩
        /// </summary>
        private void BindStorageServices() {

        }

        /// <summary>
        ///기타 서비스 바인딩
        /// </summary>
        private void BindExternalServices() {
            Container.Bind<IUIService>().To<UIService>().AsSingle().NonLazy();
            Container.Bind<ICameraService>().To<CameraService>().AsSingle();

            Container.BindInterfacesAndSelfTo<SkillDataService>().AsSingle().NonLazy();
            
           
        }

        private void BindAddressables() {
            Container.Bind<IAddressableService<SkillName, SO_SkillData>>()
                .To<AddressableService<SkillName, SO_SkillData>>()
                .AsSingle();

            Container.Bind<IAddressableService<EquipName, GameObject>>()
                .To<AddressableService<EquipName, GameObject>>()
                .AsSingle();

            Container.Bind<IAddressableService<UIName, GameObject>>()
                .To<AddressableService<UIName, GameObject>>().AsSingle();
        }

    }
}