using Zenject;
using UnityEngine;
namespace Game.Data
{
    /// <summary>
    /// Data 계층 DI 바인딩
    /// ScriptableObject, Config, DB 모델
    /// </summary>
    public class DataInstaller : MonoInstaller {
        public override void InstallBindings() {

            BindScriptableObjects();
            BindConfigurations();
            BindDatabaseModels();
            Debug.Log(GetType().Name + " Bind 완료");
        }

        /// <summary>
        /// ScriptableObject 바인딩
        /// </summary>
        private void BindScriptableObjects() {

            // 다른 ScriptableObject들 (실제 에셋이 있을 때 주석 해제)
            Container.Bind<SO_InputConfig>().FromScriptableObjectResource("Configs/InputConfig").AsSingle();
            Container.Bind<SO_GoldConfig>().FromScriptableObjectResource("Configs/GoldConfig").AsSingle();
            Container.Bind<SO_UIConfig>().FromScriptableObjectResource("Configs/UIConfig").AsSingle();

           
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