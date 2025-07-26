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
        [Inject] private InputMoveData _inputMoveData;

        private void Awake() {
            
        }

        private void Update() {
            UpdateMove();
        }




        private void UpdateMove() {
            Vector3 newDir = _inputMoveData.moveDirObservable.Value;
            newDir.z = newDir.y; // y -> z 변환
            newDir.y = 0;
            newDir.Normalize();

            if (newDir.sqrMagnitude > 0.0001f) // 방향이 있을 때만 회전
{
                // 목표 회전은 Y축만
                Quaternion targetRot = Quaternion.LookRotation(newDir, Vector3.up);

                // 부드럽게 회전
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    _characterData.rotationSpeed * Time.deltaTime
                );
            }

          
            transform.position += newDir * _characterData.moveSpeed * Time.deltaTime ;  
        }

    }
}
