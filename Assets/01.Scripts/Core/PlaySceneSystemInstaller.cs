using UnityEngine;
using Zenject;
using GamePlay;
using static UnityEngine.AudioSettings;
namespace Core
{
    public class PlaySceneSystemInstaller : MonoInstaller
    {
        public override void InstallBindings() {
            // Input Bind


#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            Container.Bind<IInputStrategy>().To<MobileInputStrategy>().AsCached();
#else
            Container.Bind<IInputStrategy>().To<PcInputStrategy>().AsCached();
#endif
           
        }
    }
}
