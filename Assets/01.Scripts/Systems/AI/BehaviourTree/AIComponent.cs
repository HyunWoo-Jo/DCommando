using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// AI ������ �����ϴ� ������Ʈ
    /// </summary>
    public class AIComponent : MonoBehaviour{
        [SerializeField] private float _updateInterval = 0.1f; // AI ������Ʈ ����

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
            // ���� Ŭ�������� ����
        }

        public void SetBehaviourTree(BehaviourNodeBase rootNode) {
            _behaviourTree.SetRootNode(rootNode);
        }
    }
}