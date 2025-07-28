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
    /// 게임 도메인 시스템(전투, 퀘스트, 인벤토리 등)
    /// Model, Service, Policy 계층 참조 가능
    /// </summary>
    public class SystemsInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            switch (_sceneName) {
                case SceneName.MainLobby:
                break;
                case SceneName.Play:
                BindInputStrategies();
                BindGameplaySystems();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
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
            Container.Bind<InputSystem>().AsCached();
 

            Container.Bind<GoldSystem>().AsCached();
        }



        /// <summary>
        /// 전투 시스템 바인딩
        /// </summary>
        private void BindBattleSystems() {

        }

    }
}