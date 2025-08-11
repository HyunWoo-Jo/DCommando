using UnityEngine;
using Zenject;
using Game.Models;
using R3;
using Cysharp.Threading.Tasks;
using Game.Data;
using UnityEngine.Assertions;
using Game.Core;
using Game.Services;
using System.Collections.Generic;
namespace Game.Systems
{
    // 실제 데이터를 이용해 Play를 Controller하는 클레스
    public class PlayerController : MonoBehaviour
    {
        [Inject] private readonly EquipSystem _equipSystem;

        [Inject] private readonly PlayerMoveModel _moveModel;
        [Header("캐릭터 스텟")]
        [SerializeField] private CharacterMoveData _characterMoveData;
        
        private AnimControllComponent _animControll;
        private CombatComponent _combatComponent;

        [Header("무기 위치")]
        [SerializeField] private Transform _weaponPivot;

        private List<IWeapon> _weapons = new();

        /// Model의 데이터에 접근
        private float MoveSpeed => _moveModel.RORP_MoveData.CurrentValue.moveSpeed;
        private float RotationSpeed => _moveModel.RORP_MoveData.CurrentValue.rotationSpeed;

        private float timer = 0f;

        private float AttackSpeed => _combatComponent.AttackSpeed;

        private void Awake() {

            _animControll = GetComponentInChildren<AnimControllComponent>();
            _combatComponent = GetComponent<CombatComponent>();
#if UNITY_EDITOR
            Assert.IsNotNull(_animControll, "AnimController가 존재하지 않습니다.");
            Assert.IsNotNull(_combatComponent, "CombatComponent가 존재하지 않습니다.");
#endif
            // Bin
            Bind();

            // 무기 생성
            _equipSystem.InstanceWeapon().
                ContinueWith(AddWeapon); // 기본 무기 추가
        }


        private void Update() {
            if (GameTime.IsPaused) return;
            if (AttackSpeed < float.Epsilon) return;
            timer += Time.deltaTime;
            
            // AttackSpeed를 초당 공격 횟수로 변환
            float attackInterval = 1f / AttackSpeed;

            if (timer > attackInterval) {
                _animControll.AttackAnim();
                timer -= attackInterval;
            }
        }


        private void Bind() {
            // 이동
            _moveModel.SetMoveData(_characterMoveData);
            _moveModel.RORP_MoveDirection
              .Skip(1)
              .ThrottleLastFrame(1)
              .Subscribe(MoveAndRotate)
              .AddTo(this);
            // 전투
            _combatComponent.OnAttackEnd.Subscribe(data => {
                foreach (var weapon in _weapons) {
                    weapon.PerformAttack();
                }
            });

        }

        private void AddWeapon(IWeapon weapon) {     
            weapon.Equip(this.gameObject);
            weapon.SetAttackPoint(transform);
            GameObject weaponObj = weapon.GameObj;
            weaponObj.transform.SetParent(_weaponPivot);
            weaponObj.transform.localRotation = Quaternion.identity;
            weaponObj.transform.localPosition = Vector3.zero;
            _weapons.Add(weapon);
        }

        private void RemoveWeapon(IWeapon weapon) {
            weapon.Unequip();

            _weapons.Remove(weapon);
        }

        /// <summary>
        /// dir로 이동과 회전
        /// </summary>
        /// <param name="dir"></param>
        private void MoveAndRotate(Vector2 dir) {
            if (dir.sqrMagnitude > 0.0001f) {                
                // 이동 + 회전 처리
                transform.SetPositionAndRotation((Vector2)transform.position + MoveSpeed * Time.deltaTime * dir, GetRotateQuaternion(dir));

                _animControll.MoveAnim(true);
            } else {
                _animControll.MoveAnim(false);
            }
        }

        public Quaternion GetRotateQuaternion(Vector2 dir) {
            // 회전
            Vector3 currentRot = transform.eulerAngles;
            float targetY = dir.x >= 0 ? 180f : 0f;
            float zFromDir = -Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg;
            float newZ = Mathf.LerpAngle(currentRot.z, zFromDir, RotationSpeed * Time.deltaTime);

            return Quaternion.Euler(0, targetY, newZ);
        }

    }
}
