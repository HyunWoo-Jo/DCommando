using UnityEngine;
using UnityEngine.Assertions;
using Game.Core;
using PlasticGui.WorkspaceWindow.Locks;
namespace Game.Systems
{
    /// <summary>
    /// �ִϸ��̼ǿ��� �߻��ϴ� �̺�Ʈ�� �������ִ� ������Ʈ
    /// Animator ������Ʈ�� �ִ� GameObject�� �ʿ�
    /// </summary>
    /// 
    public class AnimControllComponent : MonoBehaviour
    {
        private Animator _animator;

        private IAnimAttackReceiver _attackReceiver;

        // ���
        public static readonly int ATTACK_KEY = Animator.StringToHash("Attack"); 
        public static readonly int IS_MOVE_KEY = Animator.StringToHash("IsMove"); 
        public static readonly int IS_DEBUFF_KEY = Animator.StringToHash("IsDebuff");
        public static readonly int DEATH_KEY = Animator.StringToHash("Death");
        public static readonly int IS_DEATH_KEY = Animator.StringToHash("IsDeath");


        public static readonly int ATTACK_SPEED = Animator.StringToHash("AttackSpeed");
        public static readonly int MOVE_SPEED = Animator.StringToHash("MoveSpeed");

        private bool _isMove = false;

        

        private void Awake() {
            _animator = GetComponent<Animator>();
#if UNITY_EDITOR
            Assert.IsNotNull(_animator, "Animator�� �ݵ�� �ʿ��� ������Ʈ�Դϴ�");

#endif

            _attackReceiver = GetComponentInParent<IAnimAttackReceiver>();
        }
        #region �ܺο��� ȣ��Ǵ� ����

        public void DebuffAnim(bool isDebuff) {
            _animator.SetBool(IS_DEBUFF_KEY, isDebuff);
        }

        public void DeathAnim(bool isDeath) {
            if (isDeath) {
                _animator.SetBool(IS_DEATH_KEY, true);
                _animator.SetTrigger(DEATH_KEY);
            } else {
                _animator.SetBool(IS_DEATH_KEY, false);
            }
        }

        public void AttackAnim() {
            _animator.SetTrigger(ATTACK_KEY);
        }

        public void MoveAnim(bool isMove) {
            if (_isMove != isMove) {
                _animator.SetBool(IS_MOVE_KEY, isMove);
                _isMove = isMove;
            }
        }

        public void SetAttackSpeed(float speed) {
            _animator.SetFloat(ATTACK_SPEED, speed);
        }
        public void SetMoveSpeed(float speed) {
            _animator.SetFloat(MOVE_SPEED, speed);
        }
        #endregion

        #region Animation���� ȣ�� �Ǵ� ����
        public void OnAttackStart() {
            _attackReceiver?.OnAttackStart();
        }

        public void OnAttackEnd() {
            _attackReceiver.OnAttackEnd();
        }
        #endregion
    }
}
