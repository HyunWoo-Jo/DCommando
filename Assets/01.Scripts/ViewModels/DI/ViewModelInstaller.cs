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

            // Scene 별 Bind
            switch (_sceneName) {
                case SceneName.MainLobby:
                BindMainLobbySceneViewModels();
                break;
                case SceneName.Play:
                BindPlaySceneViewModels();
                break;
            }
            Debug.Log(GetType().Name + " Bind 완료");
        }

        private void BindMainLobbySceneViewModels() {

        }


        private void BindPlaySceneViewModels() {
            Container.BindInterfacesAndSelfTo<ControllerViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<GoldViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<PausePanelViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<ExpViewModel>().AsCached();

            Container.BindInterfacesAndSelfTo<HealthViewModel>().AsCached();
        }


    }
}