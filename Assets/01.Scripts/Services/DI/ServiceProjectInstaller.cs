using Game.Core;
using Game.Data;
using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace Game.Services
{
    public class ServiceProjectInstaller : MonoInstaller {
        public override void InstallBindings() {
            BindNetworkServices();
            BindStorageServices();
            BindAddressables();
            BindExternalServices();
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
            Container.Bind<ISceneService>().To<SceneService>().AsSingle();


        }

        private void BindAddressables() {
            Container.Bind<IAddressableService<SkillName, SO_SkillData>>()
                .To<AddressableService<SkillName, SO_SkillData>>()
                .AsSingle();

            Container.Bind<IAddressableService<EquipName, GameObject>>()
                .To<AddressableService<EquipName, GameObject>>()
                .AsSingle();

            Container.Bind<IAddressableService<EquipName, Sprite>>()
                .To<AddressableService<EquipName, Sprite>>()
                .AsSingle();

            Container.Bind<IAddressableService<UIName, GameObject>>()
                .To<AddressableService<UIName, GameObject>>().AsSingle();
        }
    }
}
