using UnityEngine;
using Zenject;
using Game.Models;
using R3;
using Cysharp.Threading.Tasks;
using Game.Data;
using UnityEngine.Assertions;
using Game.Core;

namespace Game.Systems
{
    // 실제 데이터를 이용해 Play를 Controller하는 클레스
    public class PlayerController : MonoBehaviour
    {
        [Inject] private readonly PlayerMoveModel _moveModel;
        [Header("캐릭터 스텟")]
        [SerializeField] private CharacterMoveData _characterMoveData;
        private AnimControllComponent _animControll;

        /// Model의 데이터에 접근
        private float MoveSpeed => _moveModel.RORP_MoveData.CurrentValue.moveSpeed;
        private float RotationSpeed => _moveModel.RORP_MoveData.CurrentValue.rotationSpeed;
        
        private void Awake() {

            _animControll = GetComponentInChildren<AnimControllComponent>();
#if UNITY_EDITOR
            Assert.IsNotNull(_animControll, "AnimController가 존재하지 않습니다.");

#endif

            _moveModel.SetMoveData(_characterMoveData);
            _moveModel.RORP_MoveDirection
                .Skip(1)
                .ThrottleLastFrame(1)
                .Subscribe(MoveAndRotate)
                .AddTo(this);
        }


        /// <summary>
        /// dir로 이동과 회전
        /// </summary>
        /// <param name="dir"></param>
        private void MoveAndRotate(Vector2 dir) {
            if (dir.sqrMagnitude > 0.0001f) {                
                // 회전
                Vector3 currentRot = transform.eulerAngles;
                float targetY = dir.x >= 0 ? 180f : 0f;
                float zFromDir = -Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg;
                float newZ = Mathf.LerpAngle(currentRot.z, zFromDir, RotationSpeed * Time.deltaTime);
                
                // 이동 + 회전 처리
                transform.SetPositionAndRotation((Vector2)transform.position + MoveSpeed * Time.deltaTime * dir, Quaternion.Euler(0, targetY, newZ));
                _animControll.MoveAnim(true);
            } else {
                _animControll.MoveAnim(false);
            }


           
        }

    }
}
