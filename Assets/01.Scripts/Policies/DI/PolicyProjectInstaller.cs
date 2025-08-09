using UnityEngine;
using Zenject;

namespace Game.Policies
{
    public class PolicyProjectInstaller : MonoInstaller
    {
        public override void InstallBindings() {
            BindPolicy();
        }

        /// <summary>
        /// 기본 정책 바인드
        /// </summary>
        private void BindPolicy() {
            Container.Bind<ICrystalPolicy>().To<CrystalPolicy>().AsSingle();
            Container.Bind<ICameraPolicy>().To<CameraPolicy>().AsSingle();
        }

    }
}
