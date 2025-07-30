using UnityEngine;
using R3;
namespace Game.Systems {
    public interface IUpdater {
        Observable<float> OnUpdate { get; }
        Observable<float> OnLateUpdate { get; }
        Observable<float> OnFixedUpdate { get; }

    }
}
