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
    public class SystemInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            BindSystems();
            switch (_sceneName) {
                case SceneName.MainScene:
                break;
                case SceneName.PlayScene:
                BindInputStrategies();
                BindGameplaySystems();
                BindCombatSystems();
                BindPlayerSystems();
                BindEnemySystem();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }


        /// <summary>
        /// 입력 전략 바인딩
        /// </summary>
        private void BindInputStrategies() {
            // 마우스 입력 전략 바인딩
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDRIOD)
            Container.Bind<IInputStrategy>().To<MobileInputStrategy>().AsCached();
#else
            Container.Bind<IInputStrategy>().To<PCInputStrategy>().AsCached();
#endif
        }
        // 핵심 System 바인드
        private void BindSystems() {
            Container.BindInterfacesAndSelfTo<UISystem>().AsCached().NonLazy();
        }


        /// <summary>
        /// 게임플레이 시스템 바인딩
        /// </summary>
        private void BindGameplaySystems() {

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


        private void BindEnemySystem() {
            Container.BindInterfacesAndSelfTo<StageSystem>().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<AIDataProvider>().AsCached().NonLazy();
        }
    }
}