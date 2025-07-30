using Game.Core;
using Game.Model;
using Game.Models;
using UnityEngine;
using Zenject;

namespace Game.Models
{
    /// <summary>
    /// Model 바인딩
    /// </summary>
    public class ModelInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            BindModel();
            switch (_sceneName) {
                case SceneName.MainLobby:
                break;
                case SceneName.Play:
                BindPlayerModels();
                BindGameModels();

                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }

        // 전체에서 사용되는 바인딩
        private void BindModel() {
            Container.Bind<CameraModel>().AsSingle();
            Container.Bind<CrystalModel>().AsSingle();
        }

        /// <summary>
        /// 플레이어 관련 모델 바인딩
        /// </summary>
        private void BindPlayerModels() {
            Container.Bind<PlayerMoveModel>().AsCached();
            Container.Bind<InputModel>().AsCached();
            Container.Bind<GoldModel>().AsCached();   
        }

        /// <summary>
        /// 게임 관련 모델 바인딩
        /// </summary>
        private void BindGameModels() {
            Container.Bind<UIModel>().AsCached();
        }

    }
}