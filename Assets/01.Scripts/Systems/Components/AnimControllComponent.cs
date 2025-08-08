using UnityEngine;
using UnityEngine.Assertions;
using Game.Core;
using PlasticGui.WorkspaceWindow.Locks;
namespace Game.Systems
{
    /// <summary>
    /// 애니메이션에서 발생하는 이벤트를 전달해주는 컴포넌트
    /// Animator 컴포넌트가 있는 GameObject에 필요
    /// </summary>
    /// 
    public class AnimControllComponent : MonoBehaviour
    {
        private Animator _animator;

        private IAnimAttackReceiver _attackReceiver;

        // 상수
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
            Assert.IsNotNull(_animator, "Animator가 반드시 필요한 컴포넌트입니다");

#endif

            _attackReceiver = GetComponentInParent<IAnimAttackReceiver>();
        }
        #region 외부에서 호출되는 영역

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

        #region Animation에서 호출 되는 영역
        public void OnAttackStart() {
            _attackReceiver?.OnAttackStartEvent();
        }

        public void OnAttackEnd() {
            _attackReceiver?.OnAttackEndEvent();
        }
        #endregion
    }
}
