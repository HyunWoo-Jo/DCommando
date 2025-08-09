using UnityEngine;
using Zenject;
namespace Game.Systems
{
    public class SystemProjectInstaller : MonoInstaller
    {
        public override void InstallBindings() {
            BindSystem();
        }

        private void BindSystem() {

            GameObject obj = new GameObject("Systems");

            DontDestroyOnLoad(obj);

            Container.Bind<GameInitSystem>().FromNewComponentOn(obj).AsSingle().NonLazy();

            Container.Bind<IUpdater>().To<Updater>().FromNewComponentOn(obj).AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<UISystem>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<CameraSystem>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<CrystalSystem>().AsSingle().NonLazy();

            Container.BindInterfacesAndSelfTo<EquipSystem>().AsSingle().NonLazy();
        }

    }
}
