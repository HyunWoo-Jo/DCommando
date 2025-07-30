using Game.Core;
using Game.Data;
using Game.Models;
using Game.Policies;
using UnityEngine;
using Zenject;

namespace Game.Systems
{
    /// <summary>
    /// Systems 계층 DI 바인딩
    /// </summary>
    public class SystemsInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            BindSystem();
            switch (_sceneName) {
                case SceneName.MainLobby:
                break;
                case SceneName.Play:
                BindInputStrategies();
                BindGameplaySystems();
                BindPlayerSystem();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }

        private void BindSystem() {
            Container.Bind<UISystem>().AsSingle();

            Container.Bind<CrystalSystem>().AsSingle();
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
 

            Container.Bind<GoldSystem>().AsCached();
        }

        private void BindPlayerSystem() {
            Container.Bind<PlayerMoveSystem>().AsCached().NonLazy();
        }


        /// <summary>
        /// 전투 시스템 바인딩
        /// </summary>
        private void BindBattleSystems() {

        }

    }
}