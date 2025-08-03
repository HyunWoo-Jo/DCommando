using Zenject;
using UnityEngine;
using Game.Core;
namespace Game.Data {
    /// <summary>
    /// Data 계층 DI 바인딩
    /// ScriptableObject, Config, DB 모델
    /// </summary>
    public class DataInstaller : MonoInstaller {
        public override void InstallBindings() {

            BindScriptableObjects();
            BindConfigurations();
            BindDatabaseModels();
            GameDebug.Log(GetType().Name + " Bind 완료");
        }

        /// <summary>
        /// ScriptableObject 바인딩
        /// </summary>
        private void BindScriptableObjects() {

            // ScriptableObject들 Configs
            Container.Bind<SO_InputConfig>().FromScriptableObjectResource("Configs/InputConfig").AsSingle();
            Container.Bind<SO_GoldConfig>().FromScriptableObjectResource("Configs/GoldConfig").AsSingle();
            Container.Bind<SO_CrystalConfig>().FromScriptableObjectResource("Configs/CrystalConfig").AsSingle();
            Container.Bind<SO_UIConfig>().FromScriptableObjectResource("Configs/UIConfig").AsSingle();
            Container.Bind<SO_ExpConfig>().FromScriptableObjectResource("Configs/ExpConfig").AsSingle();

            Container.Bind<SO_CameraConfig>().FromScriptableObjectResource("Configs/CameraConfig").AsSingle();
        }

        /// <summary>
        /// 설정 데이터 바인딩
        /// </summary>
        private void BindConfigurations() {

        }

        /// <summary>
        /// 데이터베이스 모델 바인딩
        /// </summary>
        private void BindDatabaseModels() {

        }
    }
}