using Game.Core;
using UnityEngine;
using Zenject;

namespace Game.Policies
{
    /// <summary>
    /// Policy 계층 DI 바인딩
    /// 정책 도메인
    /// Service 계층 참조 가능
    /// </summary>
    public class PolicyInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;


        public override void InstallBindings() {
            switch (_sceneName) {
                case SceneName.MainLobby:
                break;
                case SceneName.Play:
                BindGamePolicies();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }

        /// <summary>
        /// 게임 정책 바인딩
        /// </summary>
        private void BindGamePolicies() {
            // 입력 정책 바인딩 (InputSystem에서 필요)
            Container.Bind<IInputPolicy>().To<InputPolicy>().AsCached();
            Container.Bind<IGoldPolicy>().To<GoldPolicy>().AsCached();
        }
    }
}