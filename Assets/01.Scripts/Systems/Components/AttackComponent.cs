using Game.Core;
using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// 공격 하는 오브젝트에 추가
    /// 무기 Collider 부분에 추가해야함 (EX: 검)
    /// </summary>
    public class AttackComponent : MonoBehaviour {
        private IHitReceiver _hitReceiver;

        [SerializeField] private LayerMask _targetLayer; 
        [SerializeField] private DamageType _damageType;


        private void Awake() {
            _hitReceiver = transform.GetComponentInParent<IHitReceiver>();
        }

        public void SetInit(IHitReceiver receiver, DamageType type) {
            _hitReceiver = receiver;
            _damageType = type;
        }

        // hit
        private void OnTriggerEnter2D(Collider2D collision) {
            if ((_targetLayer.value & (1 << collision.gameObject.layer)) != 0) {
                _hitReceiver?.OnHit(collision.gameObject, _damageType);
            }
        }
        private void OnCollisionEnter(Collision collision) {
            if ((_targetLayer.value & (1 << collision.gameObject.layer)) != 0) {
                _hitReceiver?.OnHit(collision.gameObject, _damageType);
            }
        }

    }
}
