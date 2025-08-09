using System.ComponentModel;
using UnityEngine;
using Zenject;

namespace Game.ViewModels
{
    public class ViewModelProjectInstaller : MonoInstaller {
        public override void InstallBindings() {

            // 공통 Bind
            BindViewModels();
            BindUIViewModels();
        }
        /// <summary>
        /// 공통 Bind
        /// </summary>
        private void BindViewModels() {
            Container.BindInterfacesAndSelfTo<CrystalViewModel>().AsSingle();
        }

        private void BindUIViewModels() {
            Container.Bind<UIViewModel>().AsSingle();
        }
    }
}
