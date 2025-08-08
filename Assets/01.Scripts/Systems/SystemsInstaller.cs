using Game.Core;
using Game.Data;
using Game.Models;
using Game.Policies;
using UnityEngine;
using Zenject;

namespace Game.Systems {
    /// <summary>
    /// Systems 계층 DI 바인딩
    /// </summary>
    public class SystemsInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            BindSystem();
            BindCombatSystems();
            switch (_sceneName) {
                case SceneName.MainLobby:
                break;
                case SceneName.Play:
                BindInputStrategies();
                BindGameplaySystems();
                BindPlayerSystems();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
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

        /// <summary>
        /// 입력 전략 바인딩
        /// </summary>
        private void BindInputStrategies() {

        }

        /// <summary>
        /// 게임플레이 시스템 바인딩
        /// </summary>
        private void BindGameplaySystems() {
            // 마우스 입력 전략 바인딩
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDRIOD)
            Container.Bind<IInputStrategy>().To<MobileInputStrategy>().AsCached();
#else
            Container.Bind<IInputStrategy>().To<PCInputStrategy>().AsCached();
#endif
            Container.BindInterfacesAndSelfTo<InputSystem>().AsCached();


            Container.BindInterfacesAndSelfTo<GoldSystem>().AsCached();
        }

        private void BindPlayerSystems() {
            Container.BindInterfacesAndSelfTo<PlayerMoveSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<ExpSystem>().AsCached().NonLazy();
        }


        /// <summary>
        /// 전투 시스템 바인딩
        /// </summary>
        private void BindCombatSystems() {
            Container.BindInterfacesAndSelfTo<HealthSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<CombatSystem>().AsCached().NonLazy();
        }

    }
}