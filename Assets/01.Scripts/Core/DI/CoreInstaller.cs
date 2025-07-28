using Zenject;

namespace Game.Core.Core
{
    /// <summary>
    /// Core 계층 전용 DI 바인딩
    /// </summary>
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Core 계층 내부 바인딩만 담당
            BindCoreServices();
            BindCoreUtilities();
            BindCoreEvents();
        }

        /// <summary>
        /// Core 서비스 바인딩
        /// </summary>
        private void BindCoreServices()
        {
            // 이벤트 시스템
            // Container.Bind<IEventBus>().To<EventBus>().AsSingle();
            
            // 상태머신
            // Container.Bind<IStateMachine>().To<StateMachine>().AsSingle();
        }

        /// <summary>
        /// Core 유틸리티 바인딩
        /// </summary>
        private void BindCoreUtilities()
        {
            // 로거
            // Container.Bind<ILogger>().To<UnityLogger>().AsSingle();
            
            // 시간 관리자
            // Container.Bind<ITimeManager>().To<TimeManager>().AsSingle();
        }

        /// <summary>
        /// Core 이벤트 바인딩
        /// </summary>
        private void BindCoreEvents()
        {
            // 글로벌 이벤트들
            // Container.DeclareSignal<GameStartedSignal>();
            // Container.DeclareSignal<GamePausedSignal>();
        }
    }
}