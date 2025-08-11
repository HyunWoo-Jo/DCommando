using UnityEngine;
using Zenject;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Services;
using Game.Models;
using Game.Core;
using UnityEngine.UIElements;

namespace Game.Systems.Weapon {
    /// <summary>
    /// 무기 기본 클래스 무기에 장착 (CombatModel 사용)
    /// </summary>
    public class WeaponComponent : MonoBehaviour, IWeapon {
        [Header("무기 설정")]
        [SerializeField] private SkillName _skillName;
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _targetLayers;

        [Header("기본 스탯")]
        [SerializeField] private int _baseAttack = 10;
        [SerializeField] private int _baseDefense = 0;
        [SerializeField] private float _baseAttackSpeed = 1;
        [SerializeField] private int _rangeMultiplier = 1; // 길이 배율
        [SerializeField] private int _widthOrAngleMultiplier = 1; // 너비, 각도  배율
        [Header("디버그")]
        [SerializeField] private bool _showGizmos = true;

        // DI 주입
        [Inject] private readonly ISkillDataService _skillDataService;
        [Inject] private readonly EquipSystem _equipSystem; 
        [Inject] private readonly CombatModel _combatModel;
        [Inject] private readonly CombatSystem _combatSystem;

        // 스킬 데이터
        private SO_SkillData _skillData;

        // 상태
        private bool _isInitialized = false;
        private bool _isAttacking = false;

        // 소유자
        private GameObject _owner;
        // 고유 ID
        private int _weaponId;

        
       

        // 프로퍼티
        public EquipName EquipName { get; private set; }
        public SkillName SkillName => _skillName;
        public SO_SkillData SkillData => _skillData;
        public bool IsInitialized => _isInitialized;
        public bool IsAttacking => _isAttacking;
        public int WeaponId => _weaponId;

        public float AttackSpeed => _baseAttackSpeed;
        public Vector2 Forward => -_attackPoint.right;

        public GameObject GameObj => this.gameObject;

        private float Width => _skillData.Width * _widthOrAngleMultiplier;
        private float Angle => _skillData.Angle * _widthOrAngleMultiplier;

        private float Range => _skillData.Range * _rangeMultiplier;

        private void OnDestroy() {
            Unequip();
        }
 

        #region 스탯 관리
        public void SetEquipName(EquipName equipName ) {
            EquipName = EquipName;
        }

        /// <summary>
        /// 현재 최종 공격력 반환
        /// </summary>
        public int GetFinalAttack() {
            if (!_isInitialized) return _baseAttack;
            return _combatModel.GetFinalAttack(_weaponId);
        }

        /// <summary>
        /// Combat 데이터 반환
        /// </summary>
        public CombatData GetCombatData() {
            if (!_isInitialized) return new CombatData(_baseAttack, _baseDefense, _baseAttackSpeed);
            return _combatModel.GetCombatData(_weaponId);
        }
        #endregion

        #region 공격 처리
        /// <summary>
        /// 공격 실행
        /// </summary>
        public virtual void PerformAttack() {
            if (!_isInitialized || _isAttacking) {
                GameDebug.LogWarning($"공격 불가: Initialized={_isInitialized}, Attacking={_isAttacking}");
                return;
            }

            _isAttacking = true;

            // 범위 내 타겟 찾기
            var targets = FindTargetsInRange();

            // 각 타겟에 대해 데미지 처리
            foreach (var target in targets) {
                ProcessAttackTarget(target);
            }

            GameDebug.Log($"무기 공격 실행: {_skillName}, 타겟 수: {targets.Count}, 공격력: {GetFinalAttack()}");

            // 공격 완료
            _isAttacking = false;

            // 공격 완료 이벤트
            OnAttackCompleted(targets);
        }

        /// <summary>
        /// 범위 내 타겟 찾기
        /// </summary>
        private List<Collider2D> FindTargetsInRange() {
            var targets = new List<Collider2D>();

            if (_skillData == null || _attackPoint == null) return targets;

            Vector2 attackPosition = _attackPoint.position;

            switch (_skillData.RangeType) {
                case SkillRangeType.Circle:
                targets.AddRange(FindCircleTargets(attackPosition));
                break;
                case SkillRangeType.Sector:
                targets.AddRange(FindSectorTargets(attackPosition));
                break;
                case SkillRangeType.Rectangle:
                targets.AddRange(FindRectangleTargets(attackPosition));
                break;
                case SkillRangeType.Line:
                targets.AddRange(FindLineTargets(attackPosition));
                break;
            }

            return targets;
        }

        private List<Collider2D> FindCircleTargets(Vector2 center) {
            var targets = new List<Collider2D>();
            var colliders = Physics2D.OverlapCircleAll(center, Range, _targetLayers);
            targets.AddRange(colliders);
            return targets;
        }

        private List<Collider2D> FindSectorTargets(Vector2 center) {
            var targets = new List<Collider2D>();
            var potentialTargets = Physics2D.OverlapCircleAll(center,Range, _targetLayers);

            float halfAngle = Angle * 0.5f * _widthOrAngleMultiplier;
            Vector3 forward = Forward;

            foreach (var collider in potentialTargets) {
                Vector2 directionToTarget = (collider.transform.position - _attackPoint.position).normalized;
                float angleToTarget = Vector2.Angle(forward, directionToTarget);

                if (angleToTarget <= halfAngle) {
                    targets.Add(collider);
                }
            }

            return targets;
        }

        private List<Collider2D> FindRectangleTargets(Vector2 center) {
            var targets = new List<Collider2D>();

            // 사각형 영역 계산
            Vector2 boxSize = new Vector2(Width, Range);
            Vector2 boxCenter = center + Forward * (Range * 0.5f);

            var colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, _attackPoint.eulerAngles.z, _targetLayers);
            
            targets.AddRange(colliders);

            return targets;
        }

        private List<Collider2D> FindLineTargets(Vector2 center) {
            var targets = new List<Collider2D>();

            if (_skillData.Width > 0) {
                // 두께가 있는 선 (사각형으로 처리)
                Vector2 boxSize = new Vector2(Width, Range);
                Vector2 boxCenter = center + (Vector2)Forward * (Range* 0.5f);

                var colliders = Physics2D.OverlapBoxAll(boxCenter, boxSize, _attackPoint.eulerAngles.z, _targetLayers);
                targets.AddRange(colliders);
            } else {
                // 단순한 선 (RaycastAll 사용)
                Vector2 direction = Forward;
                var hits = Physics2D.RaycastAll(center, direction, Range, _targetLayers);

                foreach (var hit in hits) {
                    if (hit.collider != null) {
                        targets.Add(hit.collider);
                    }
                }
            }

            return targets;
        }

        /// <summary>
        /// 개별 타겟에 대한 공격 처리
        /// </summary>
        private void ProcessAttackTarget(Collider2D target) {
            // CombatComponent 찾기
            var combatComponent = target.GetComponent<CombatComponent>();
            if (combatComponent == null) {
                GameDebug.LogWarning($"CombatComponent를 찾을 수 없음: {target.name}");
                return;
            }

            // 최종 데미지 계산
            float finalDamage = CalculateFinalDamage();

            // 데미지 처리 (CombatSystem을 통해)
            int attackerId = _weaponId; // 소유자 ID를 사용
            int targetId = target.gameObject.GetInstanceID();

            // 무기 타입에 따른 데미지 타입 결정 (확장 가능)
            DamageType damageType = GetWeaponDamageType();

            _combatSystem.ProcessAttack(attackerId, targetId, damageType, finalDamage);

            GameDebug.Log($"무기 공격 처리: {target.name}, 데미지: {finalDamage}");
        }

        /// <summary>
        /// 최종 데미지 계산 (스킬 데이터 + CombatModel 스탯)
        /// </summary>
        private float CalculateFinalDamage() {
            // 스킬 데이터에서 기본 데미지와 배율
            float skillBaseDamage = _skillData.AdditionalDamage;
            float skillMultiplier = _skillData.DamageMultiplier;

            // CombatModel에서 현재 공격력
            int weaponAttack = GetFinalAttack();

            // 최종 데미지 계산: (무기공격력 + 스킬추가데미지) * 스킬배율
            float finalDamage = (weaponAttack + skillBaseDamage) * skillMultiplier;

            return Mathf.Max(0, finalDamage);
        }

        /// <summary>
        /// 무기 타입에 따른 데미지 타입 반환 (확장 가능)
        /// </summary>
        protected virtual DamageType GetWeaponDamageType() {
            return DamageType.Physical; // 기본은 물리 데미지
        }

        protected virtual void OnAttackCompleted(List<Collider2D> targets) {
            // 자식 클래스에서 오버라이드 가능
        }
        #endregion

        #region 디버그
#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if (!_showGizmos || _attackPoint == null) return;
            if (_skillData != null) {
                DrawSkillRange();
                DrawWeaponInfo();
            } else {
                // 기본 범위 표시
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(_attackPoint.position, 1f);
            }
        }

        private void DrawSkillRange() {
            Vector3 center = _attackPoint.position;
            Gizmos.color = Color.red;

            switch (_skillData.RangeType) {
                case SkillRangeType.Circle:
                Gizmos.DrawWireSphere(center, _skillData.Range);
                break;
                case SkillRangeType.Sector:
                DrawSectorGizmo(center);
                break;
                case SkillRangeType.Rectangle:
                DrawRectangleGizmo(center);
                break;
                case SkillRangeType.Line:
                DrawLineGizmo(center);
                break;
            }
        }

        private void DrawWeaponInfo() {
            if (!_isInitialized) return;

            // 무기 정보를 Scene View에 표시
            Vector3 textPos = _attackPoint.position + Vector3.up;

            var combatData = GetCombatData();
            string info = $"ATK: {combatData.FinalAttack} ({combatData.baseAttack}+{combatData.bonusAttack}*{combatData.attackMultiplier:F1})";
            UnityEditor.Handles.Label(textPos, info);
        }

        private void DrawSectorGizmo(Vector3 center) {
            float halfAngle = _skillData.Angle * 0.5f;
            Vector3 forward = Forward;

            Vector3 leftBound = Quaternion.Euler(0, 0, halfAngle) * forward * _skillData.Range;
            Vector3 rightBound = Quaternion.Euler(0, 0, -halfAngle) * forward * _skillData.Range;

            Gizmos.DrawLine(center, center + leftBound);
            Gizmos.DrawLine(center, center + rightBound);

            // 호 그리기 (간단히)
            int segments = 20;
            Vector3 prevPoint = center + leftBound;
            for (int i = 1; i <= segments; i++) {
                float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
                Vector3 point = center + Quaternion.Euler(0, 0, angle) * forward * _skillData.Range;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }

        private void DrawRectangleGizmo(Vector3 center) {
            Vector3 forward = Forward;
            Vector3 boxCenter = center + forward * (_skillData.Range * 0.5f);
            Vector3 size = new Vector3(_skillData.Width, _skillData.Range, 0.1f);

            Gizmos.matrix = Matrix4x4.TRS(boxCenter, _attackPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawLineGizmo(Vector3 center) {
            Vector3 forward = Forward;
            Vector3 endPoint = center + forward * _skillData.Range;

            Gizmos.DrawLine(center, endPoint);

            if (_skillData.Width > 0) {
                Vector3 right = _attackPoint.right;
                float halfWidth = _skillData.Width * 0.5f;

                Vector3 leftStart = center - right * halfWidth;
                Vector3 rightStart = center + right * halfWidth;
                Vector3 leftEnd = endPoint - right * halfWidth;
                Vector3 rightEnd = endPoint + right * halfWidth;

                Gizmos.DrawLine(leftStart, leftEnd);
                Gizmos.DrawLine(rightStart, rightEnd);
                Gizmos.DrawLine(leftStart, rightStart);
                Gizmos.DrawLine(leftEnd, rightEnd);
            }
        }
#endif
        #endregion

        #region 공개 메서드
        /// <summary>
        /// 무기 스탯 정보 출력 (디버그용)
        /// </summary>
        [ContextMenu("무기 정보 출력")]
        public void PrintWeaponInfo() {
            if (!_isInitialized) {
                GameDebug.Log("무기가 초기화되지 않음");
                return;
            }

            var combatData = GetCombatData();
            GameDebug.Log($"=== 무기 정보: {_skillName} ===");
            GameDebug.Log($"기본 공격력: {combatData.baseAttack}");
            GameDebug.Log($"보너스 공격력: {combatData.bonusAttack}");
            GameDebug.Log($"공격력 배율: {combatData.attackMultiplier:F2}");
            GameDebug.Log($"최종 공격력: {combatData.FinalAttack}");
            GameDebug.Log($"스킬 범위: {_skillData.Range}");
            GameDebug.Log($"스킬 타입: {_skillData.RangeType}");
        }

        /// <summary>
        /// 스킬 이름 설정 (런타임)
        /// </summary>
        public void SetSkillName(SkillName newSkillName) {
            if (_skillName == newSkillName) return;

            _skillName = newSkillName;
            Unequip();
            Equip(_owner);
        }

        /// <summary>
        /// 공격 지점 설정
        /// </summary>
        public void SetAttackPoint(Transform newAttackPoint) {
            _attackPoint = newAttackPoint;
        }

        /// <summary>
        /// 타겟 레이어 설정
        /// </summary>
        public void SetTargetLayers(LayerMask newLayers) {
            _targetLayers = newLayers;
        }
        /// <summary>
        /// 무기 장착
        /// </summary>
        public async void Equip(GameObject owner) {
            if (_isInitialized) {
                Unequip();
            }
            _owner = owner;
            _weaponId = owner.GetInstanceID();
            SetAttackPoint(owner.transform);
            try {
                // Service를 통해 스킬 데이터 로드
                _skillData = await _skillDataService.LoadSkillDataAsync(_skillName);

                if (_skillData == null) {
                    GameDebug.LogError($"스킬 데이터 로드 실패: {_skillName}");
                    return;
                }

                // CombatModel에 무기 데이터 등록
                _combatModel.AddBonusAttack(_weaponId, _baseAttack);
                _combatModel.AddBonusDefense(_weaponId, _baseDefense);
                _combatModel.AddBonusAttackSpeed(_weaponId, _baseAttackSpeed);
                _isInitialized = true;
                GameDebug.Log($"무기 초기화 완료: {_skillName}, Range: {_skillData.Range}, Attack: {GetFinalAttack()}");

            } catch (System.Exception e) {
                GameDebug.LogError($"무기 초기화 실패: {_skillName}, 에러: {e.Message}");
            }

        }
        /// <summary>
        /// 장착 해제
        /// </summary>
        public void Unequip() {
            // 장착 해제
            if (_isInitialized) {
                _skillData = null;
                _combatModel.AddBonusAttack(_weaponId, -_baseAttack);
                _combatModel.AddBonusDefense(_weaponId, -_baseDefense);
                _combatModel.AddBonusAttackSpeed(_weaponId, -_baseAttackSpeed);
                _isInitialized = false;
                _skillDataService.UnloadSkill(_skillName);
                _equipSystem.UnLoadWeapon(this);
                GameDebug.Log($"무기 해제: {_skillName}, ID: {_weaponId}");
            }
        }


       
        #endregion
    }
}