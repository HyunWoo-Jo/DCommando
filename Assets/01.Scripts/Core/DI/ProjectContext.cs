using UnityEngine;
using Zenject;

namespace Game.Core
{
    /// <summary>
    /// 프로젝트 전체 DI 컨텍스트 설정
    /// Core 계층만 바인딩 (다른 계층은 참조하지 않음)
    /// </summary>
    public class ProjectContext : MonoInstaller
    {
        [Header("프로젝트 설정")]
        [SerializeField] private bool _enableDebugMode = false;
        
        public override void InstallBindings()
        {
            // 디버그 모드 설정
            Container.BindInstance(_enableDebugMode).WithId("DebugMode");
            
            // DIHelper 초기화
            DIHelper.Initialize(Container);
            
            // Core 계층 바인딩 (이벤트, 상태머신, 유틸 등)
            BindCoreLayer();
        }

        /// <summary>
        /// Core 계층 바인딩
        /// </summary>
        private void BindCoreLayer()
        {
            // 이벤트 시스템
            // Container.Bind<IEventBus>().To<EventBus>().AsSingle();
            
            // 상태머신
            // Container.Bind<IStateMachine>().To<StateMachine>().AsSingle();
            
            // 로거
            // Container.Bind<ILogger>().To<UnityLogger>().AsSingle();
            
            // 시간 관리자
            // Container.Bind<ITimeManager>().To<TimeManager>().AsSingle();
        }
    }
}