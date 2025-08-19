using UnityEngine;
using Zenject;
using Game.Core.Styles;
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
            
            // Core 계층 바인딩 (이벤트, 상태머신, 유틸 등)
            BindStyle();
        }


        private void BindStyle() {
           Container.BindAddressable<SO_CrystalStyle>("Styles/CrystalStyle.asset").AsSingle();
           Container.BindAddressable<SO_GoldStyle>("Styles/GoldStyle.asset").AsSingle();
           Container.BindAddressable<SO_DamageUIStyle>("Styles/DamageUIStyle.asset").AsSingle();
        }

    }
}