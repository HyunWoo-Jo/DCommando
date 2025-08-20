using Game.Core;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// AI 동작을 제어하는 컴포넌트
    /// </summary>
    public class AIComponent : MonoBehaviour{
        [SerializeField] private SO_BehaviourTree _behaviourTree;

        public SO_BehaviourTree BehaviourTree => _behaviourTree;
        public bool isStop;
        private void Awake() {
            _behaviourTree = _behaviourTree.CreateCopy();
            _behaviourTree.SetID(gameObject.GetInstanceID());

            var health = GetComponent<HealthComponent>();

            health.OnDeath += OnDeath;
        }

        private void OnDestroy() {
            var health = GetComponent<HealthComponent>();
            health.OnDeath -= OnDeath;
        }


        public void OnDeath() {
            isStop = true;
        }

        public void Update() {
            if (GameTime.IsPaused || isStop) return;
            _behaviourTree.Update();
        }

        public void SetBehaviourTree(BehaviourNodeBase rootNode) {
            _behaviourTree.SetRootNode(rootNode);
        }
    }
}