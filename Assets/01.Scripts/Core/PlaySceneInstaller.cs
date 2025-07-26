using UnityEngine;
using Zenject;
using Data;
using UI;
namespace Core
{
    public class PlaySceneInstaller : MonoInstaller 
    {
        public override void InstallBindings() {
            // Control
            Container.Bind<InputMoveData>().AsCached(); // Data
            Container.Bind<ControllerViewModel>().AsCached(); // VM
        }
    }
}
