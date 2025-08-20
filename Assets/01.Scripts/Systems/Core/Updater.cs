using UnityEngine;
using Zenject;
using R3;
using Game.Core;
namespace Game.Systems
{

    /// <summary>
    /// 일반 클레스를 Mono 기반 업데이트가 작동할 수 있도록 도와주는 클레스 Installer에서 관리
    /// </summary>
    public class Updater : MonoBehaviour, IUpdater {

        private readonly Subject<float> _onUpdate = new();
        private readonly Subject<float> _onLateUpdate = new();
        private readonly Subject<float> _onFixedUpdate = new();

        public Observable<float> OnUpdate => _onUpdate;
        public Observable<float> OnLateUpdate => _onLateUpdate;
        public Observable<float> OnFixedUpdate => _onFixedUpdate;

        private void Update() {
            _onUpdate.OnNext(GameTime.DeltaTime);
        }

        private void LateUpdate() {
            _onLateUpdate.OnNext(GameTime.DeltaTime);
        }

        private void FixedUpdate() {
            _onFixedUpdate.OnNext(GameTime.DeltaTime);
        }

        private void OnDestroy() {
            _onUpdate?.Dispose();
            _onLateUpdate?.Dispose();
            _onFixedUpdate?.Dispose();
        }
    }
}
