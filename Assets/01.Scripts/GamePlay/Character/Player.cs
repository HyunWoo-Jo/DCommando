using UnityEngine;
using Zenject;
using Data;
using R3;
using System;
using R3.Triggers;
namespace GamePlay
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private CharacterData _characterData;
        [Inject] private PlayerMoveData _playerMoveData;

        private void Awake() {

            Bind();
        }


        private void Bind() {
            _playerMoveData.moveDirObservable
                .ThrottleLastFrame(1)
                .Subscribe(UpdateMove)
                .AddTo(this);
        }


        private void UpdateMove(Vector2 dir) {
            if (dir.sqrMagnitude > 0.0001f) { // 방향이 있을 때만 회전

                if (dir.sqrMagnitude < 0.0001f) return;

                // 현재 회전
                Vector3 currentRot = transform.eulerAngles;

                // Y 각도 
                float targetY = dir.x >= 0 ? 180f : 0f;

                // Z 각도 획득
                float zFromDir = -Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg;

                // 보간하여 부드럽게 적용
                float newZ = Mathf.LerpAngle(currentRot.z, zFromDir, _characterData.rotationSpeed * Time.deltaTime);

                // 3. 회전 적용 (Y는 즉시 스냅, X는 0 유지)
                transform.rotation = Quaternion.Euler(0, targetY, newZ);
            }

          
            transform.position = (Vector2)transform.position + dir * _characterData.moveSpeed * Time.deltaTime ;  
        }

    }
}
