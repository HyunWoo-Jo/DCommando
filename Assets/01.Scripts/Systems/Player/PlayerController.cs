using UnityEngine;
using Zenject;
using Game.Models;
using R3;
using Cysharp.Threading.Tasks;
using Game.Data;

namespace Game.Systems
{
    // 실제 데이터를 이용해 Play를 Controller하는 클레스
    public class PlayerController : MonoBehaviour
    {
        [Inject] private PlayerMoveModel _moveModel;
        [Header("캐릭터 스텟")]
        [SerializeField] private CharacterMoveData _characterMoveData;


        /// Model의 데이터에 접근
        private float MoveSpeed => _moveModel.RORP_MoveData.CurrentValue.moveSpeed;
        private float RotationSpeed => _moveModel.RORP_MoveData.CurrentValue.rotationSpeed;
        
        private void Awake() {

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
            if (dir.sqrMagnitude > 0.0001f) { // 방향이 있을 때만 회전

                if (dir.sqrMagnitude < 0.0001f) return;

                // 현재 회전
                Vector3 currentRot = transform.eulerAngles;

                // Y 각도 
                float targetY = dir.x >= 0 ? 180f : 0f;

                // Z 각도 획득
                float zFromDir = -Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg;

                // 보간하여 부드럽게 적용
                float newZ = Mathf.LerpAngle(currentRot.z, zFromDir, RotationSpeed * Time.deltaTime);

                // 3. 회전 적용 (Y는 즉시 스냅, X는 0 유지)
                transform.rotation = Quaternion.Euler(0, targetY, newZ);
            }


            transform.position = (Vector2)transform.position + MoveSpeed * Time.deltaTime * dir;
        }

    }
}
