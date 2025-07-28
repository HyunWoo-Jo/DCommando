using Game.Core;
using UnityEngine;
using Zenject;

namespace Game.Core
{
    /// <summary>
    /// 씬별 DI 컨텍스트 설정
    /// 각 씬마다 필요한 계층의 인스톨러들만 설치
    /// </summary>
    public class SceneContext : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            // 씬 이름을 컨테이너에 바인딩
            Container.BindInstance(_sceneName).WithId("SceneName");

            // 씬별 특화 바인딩
            InstallSceneSpecificBindings();
            Debug.Log(GetType().Name + " Bind 완료");
        }


        private void InstallSceneSpecificBindings() {
            InstallSceneSpecificBindings(_sceneName);
        }

        /// <summary>
        /// 씬별 특화 바인딩
        /// </summary>
        private void InstallSceneSpecificBindings(SceneName _sceneName) {
            switch (_sceneName) {
                case SceneName.MainLobby:
                BindMainLobbyScene();
                break;
                case SceneName.Play:
                BindPlayScene();
                break;
            }
        }

        /// <summary>
        /// 메인 로비 씬 바인딩
        /// </summary>
        private void BindMainLobbyScene() {
            Debug.Log("MainLobby 씬 특화 바인딩 설정");
        }

        /// <summary>
        /// 게임플레이 씬 바인딩
        /// </summary>
        private void BindPlayScene() {
            Debug.Log("Gameplay 씬 특화 바인딩 설정");
        }
    }
}