using Game.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Models
{
    /// <summary>
    /// Model 바인딩
    /// </summary>
    public class ModelInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            switch (_sceneName) {
                case SceneName.MainScene:
                break;
                case SceneName.PlayScene:
                BindPlayerModels();
                BindGameModels();
                BindCombatSceneModel();
                break;
            }
        }


        /// <summary>
        /// 플레이어 관련 모델 바인딩
        /// </summary>
        private void BindPlayerModels() {
            Container.BindInterfacesAndSelfTo<PlayerMoveModel>().AsCached();
            Container.BindInterfacesAndSelfTo<InputModel>().AsCached();
            Container.BindInterfacesAndSelfTo<GoldModel>().AsCached();   
            Container.BindInterfacesAndSelfTo<ExpModel>().AsCached();
        }

        /// <summary>
        /// 전투 관련 모델 바인딩
        /// </summary>
        private void BindCombatSceneModel() {
            Container.BindInterfacesAndSelfTo<HealthModel>().AsCached();
            Container.BindInterfacesAndSelfTo<CombatModel>().AsCached();
        }

        /// <summary>
        /// 게임 관련 모델 바인딩
        /// </summary>
        private void BindGameModels() {
            Container.BindInterfacesAndSelfTo<StageModel>().AsCached();
            Container.BindInterfacesAndSelfTo<UIModel>().AsCached();
            
        }

    }
}