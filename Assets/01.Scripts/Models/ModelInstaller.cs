using Game.Core;
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
                BindCombatSceneModel();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }

        // 전체에서 사용되는 바인딩
        private void BindModel() {
            Container.BindInterfacesAndSelfTo<CameraModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<CrystalModel>().AsSingle();
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
            Container.BindInterfacesAndSelfTo<UIModel>().AsCached();
            
        }

    }
}