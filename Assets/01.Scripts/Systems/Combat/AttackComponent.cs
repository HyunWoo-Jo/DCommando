using UnityEngine;

namespace Game.Systems
{
    /// <summary>
    /// 공격 하는 오브젝트에 추가
    /// 무기 Collider 부분에 추가해야함 (EX: 검, 활)
    /// </summary>
    public class AttackComponent : MonoBehaviour
    {
        private bool _isAttackAble;

        [SerializeField] private LayerMask _targetLayer;

        public void SetAttackAble() {
            _isAttackAble = true;
        }

        // hit
        private void OnCollisionEnter2D(Collision2D collision) {
            if (_isAttackAble) return;
           if((_targetLayer.value & (1 << collision.gameObject.layer)) != 0){
                _isAttackAble = false;
           }
        }

        private void OnCollisionStay2D(Collision2D collision) {
            if((_targetLayer.value & (1 << collision.gameObject.layer)) != 0) {

            }
        }
    }
}
