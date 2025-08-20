using UnityEngine;
using Zenject;

namespace Game.Models
{
    public class ModelProjectInstaller : MonoInstaller
    {
        public override void InstallBindings() {
            BindModel();
        }

        // 전체에서 사용되는 바인딩
        private void BindModel() {
            Container.BindInterfacesAndSelfTo<CameraModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<CrystalModel>().AsSingle();
            Container.BindInterfacesAndSelfTo<EquipModel>().AsSingle();
        }
    }
}
