using System;
using UnityEngine;
using Zenject;
using Game.Core;

namespace Game.ViewModels {
    /// <summary>
    /// ViewModel DI 바인딩
    /// </summary>
    public class ViewModelInstaller : MonoInstaller {
        [SerializeField] private SceneName _sceneName;

        public override void InstallBindings() {
            BindUIViewModels();
            // Scene 별 Bind
            switch (_sceneName) {
                case SceneName.MainScene:
                BindMainLobbySceneViewModels();
                break;
                case SceneName.PlayScene:
                BindPlaySceneViewModels();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }

        private void BindMainLobbySceneViewModels() {
            Container.BindInterfacesAndSelfTo<InventoryViewModel>().AsCached();
        }


        private void BindPlaySceneViewModels() {
            Container.BindInterfacesAndSelfTo<ControllerViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<GoldViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<PausePanelViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<ExpViewModel>().AsCached();

            Container.BindInterfacesAndSelfTo<HealthViewModel>().AsCached();
        }
        private void BindUIViewModels() {
            Container.Bind<UIViewModel>().AsCached();
        }

    }
}