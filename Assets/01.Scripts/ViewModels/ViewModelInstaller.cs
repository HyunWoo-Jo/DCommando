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

            // 공통 Bind
            BindViewModels();
            BindUIViewModels();

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

        /// <summary>
        /// 공통 Bind
        /// </summary>
        private void BindViewModels() {
            Container.BindInterfacesAndSelfTo<CrystalViewModel>().AsSingle();
        }


        private void BindMainLobbySceneViewModels() {

        }


        private void BindPlaySceneViewModels() {
            Container.BindInterfacesAndSelfTo<ControllerViewModel>().AsCached();
            Container.BindInterfacesAndSelfTo<GoldViewModel>().AsCached();
        }


        private void BindUIViewModels() {
            Container.Bind<UIViewModel>().AsSingle();
        }
    }
}