using Zenject;
using Game.Core.Event;
namespace Game.Core
{
    /// <summary>
    /// Core 계층 전용 DI 바인딩
    /// </summary>
    public class CoreInstaller : MonoInstaller {
        public override void InstallBindings() {
            // Core 계층 내부 바인딩만 담당
            BindCoreServices();
            BindCoreUtilities();
            BindCoreEvents();
        }

        /// <summary>
        /// Core 서비스 바인딩
        /// </summary>
        private void BindCoreServices() {
       

        }

        /// <summary>
        /// Core 유틸리티 바인딩
        /// </summary>
        private void BindCoreUtilities() {

        }

        /// <summary>
        /// Core 이벤트 바인딩
        /// </summary>
        private void BindCoreEvents() {

        }
    }
}