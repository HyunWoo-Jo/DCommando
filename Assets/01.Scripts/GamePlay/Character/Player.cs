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
            newDir.z = newDir.y; // y -> z ��ȯ
            newDir.y = 0;
            newDir.Normalize();

            if (newDir.sqrMagnitude > 0.0001f) // ������ ���� ���� ȸ��
{
                // ��ǥ ȸ���� Y�ุ
                Quaternion targetRot = Quaternion.LookRotation(newDir, Vector3.up);

                // �ε巴�� ȸ��
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
