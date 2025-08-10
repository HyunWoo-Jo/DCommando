using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace Game.ViewModels
{
    public class ViewModelProjectInstaller : MonoInstaller {
        public override void InstallBindings() {

            // 공통 Bind
            BindViewModels();
        }
        /// <summary>
        /// 공통 Bind
        /// </summary>
        private void BindViewModels() {
            Container.Bind<SceneViewModel>().AsSingle();

            Container.BindInterfacesAndSelfTo<CrystalViewModel>().AsSingle();
        }

      
    }
}
