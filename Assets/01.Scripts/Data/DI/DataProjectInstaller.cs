using UnityEngine;
using Zenject;
using Game.Core;
namespace Game.Data
{
    public class DataProjectInstaller : MonoInstaller
    {
        public override void InstallBindings() {
            BindScriptableObjects();
        }

        /// <summary>
        /// ScriptableObject 바인딩
        /// </summary>
        private void BindScriptableObjects() {

            // ScriptableObject들 Configs
            Container.BindAddressable<SO_InputConfig>("Configs/InputConfig.asset").AsSingle();
            Container.BindAddressable<SO_GoldConfig>("Configs/GoldConfig.asset").AsSingle();
            Container.BindAddressable<SO_CrystalConfig>("Configs/CrystalConfig.asset").AsSingle();
            Container.BindAddressable<SO_ExpConfig>("Configs/ExpConfig.asset").AsSingle();

            Container.BindAddressable<SO_CameraConfig>("Configs/CameraConfig.asset").AsSingle();
        }

    }
}
