using UnityEngine;
using Zenject;
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
            Container.Bind<SO_InputConfig>().FromScriptableObjectResource("Configs/InputConfig").AsSingle();
            Container.Bind<SO_GoldConfig>().FromScriptableObjectResource("Configs/GoldConfig").AsSingle();
            Container.Bind<SO_CrystalConfig>().FromScriptableObjectResource("Configs/CrystalConfig").AsSingle();
            Container.Bind<SO_ExpConfig>().FromScriptableObjectResource("Configs/ExpConfig").AsSingle();

            Container.Bind<SO_CameraConfig>().FromScriptableObjectResource("Configs/CameraConfig").AsSingle();
        }

    }
}
