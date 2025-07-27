using UnityEngine;
using Zenject;
using Data;
using UI;
namespace Core
{
    public class PlaySceneInstaller : MonoInstaller 
    {
        public override void InstallBindings() {
            // Input
            Container.Bind<InputData>().AsCached();

            // Control
            Container.Bind<PlayerMoveData>().AsCached(); // Data
            Container.Bind<ControllerViewModel>().AsCached(); // VM
        }
    }
}
