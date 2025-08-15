using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// AI 동작을 제어하는 컴포넌트
    /// </summary>
    public class AIComponent : MonoBehaviour{
        [SerializeField] private float _updateInterval = 0.1f; // AI 업데이트 간격

        private BehaviourTree _behaviourTree;
        private float _lastUpdateTime;

        public BehaviourTree BehaviourTree => _behaviourTree;

        private void Awake() {
            _behaviourTree = new BehaviourTree(gameObject.GetInstanceID() );
        }

        private void Start() {
            BuildBehaviourTree();
        }

        public void Update() {
            if (Time.time - _lastUpdateTime >= _updateInterval) {
                _behaviourTree.Update();
                _lastUpdateTime = Time.time;
            }
        }



        protected virtual void BuildBehaviourTree() {
            // 하위 클래스에서 구현
        }

        public void SetBehaviourTree(BehaviourNodeBase rootNode) {
            _behaviourTree.SetRootNode(rootNode);
        }
    }
}