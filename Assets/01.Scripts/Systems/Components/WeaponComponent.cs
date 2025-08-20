using UnityEngine;
using Zenject;
using Game.Core;
using Game.Models;
using Game.Data;
using Game.Core.Event;
using R3;

namespace Game.Systems {
    /// <summary>
    /// 무기 기본 클래스 무기에 장착 (CombatModel 사용)
    /// </summary>
    public class WeaponComponent : MonoBehaviour, IWeapon {
        [Header("무기 설정")]
        [SerializeField] private SkillName _skillName;
        [SerializeField] private Transform _attackPoint;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField] private DamageType _damageType = DamageType.Physical;
        [Header("기본 스탯")]
        [SerializeField] private int _baseAttack = 10;
        [SerializeField] private int _baseDefense = 0;
        [SerializeField] private float _baseAttackSpeed = 1;
        [SerializeField] private float _rangeMultiplier = 1; // 길이 배율
        [SerializeField] private float _widthOrAngleMultiplier = 1; // 너비, 각도  배율
        [Header("디버그")]
        [SerializeField] private bool _showGizmos = true;

        // DI 주입
        [Inject] private readonly WeaponSystem _weaponSystem;
        [Inject] private readonly CombatModel _combatModel;

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
        public Transform AttackPoint => _attackPoint;
        public LayerMask TargetLayers => _targetLayers;
        public DamageType DamageType => _damageType;
        public int BaseAttack => _baseAttack;
        public int BaseDefense => _baseDefense;
        public float BaseAttackSpeed => _baseAttackSpeed;

        public float AttackSpeed => _baseAttackSpeed;
        public Vector2 Forward => -_attackPoint.right;

        public GameObject GameObj => this.gameObject;

        public float Width => _skillData?.Width * _widthOrAngleMultiplier ?? 0f;
        public float Angle => _skillData?.Angle * _widthOrAngleMultiplier ?? 0f;
        public float Range => _skillData?.Range * _rangeMultiplier ?? 0f;

        // dispose
        private CompositeDisposable _disposables = new();

        private void Awake() {
            EventBus.Subscribe<StatChangeEvent>(OnUpgradeEvent).AddTo(_disposables);
        }

        private void OnDestroy() {
            Unequip();
            _disposables?.Dispose();
        }

        private void OnUpgradeEvent(StatChangeEvent e) {
            if (e.upgradeType == UpgradeType.AttackRange) {
                _rangeMultiplier += e.value;
            } else if (e.upgradeType == UpgradeType.AttackWidth) {
                _widthOrAngleMultiplier += e.value;
            }
        }

        #region 시스템 연동 메서드 (WeaponSystem에서 호출)

        public void SetSkillData(SO_SkillData skillData) {
            _skillData = skillData;
        }

        public void SetInitialized(bool initialized) {
            _isInitialized = initialized;
        }

        public void SetAttacking(bool attacking) {
            _isAttacking = attacking;
        }

        #endregion

        #region 스탯 관리
        public void SetEquipName(EquipName equipName) {
            EquipName = equipName;
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
        /// 공격 실행 (WeaponSystem에 위임)
        /// </summary>
        public virtual void PerformAttack() {
            _weaponSystem.PerformAttack(this);
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
                Gizmos.DrawWireSphere(center, Range);
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
            float halfAngle = Angle * 0.5f;
            Vector3 forward = Forward;

            Vector3 leftBound = Quaternion.Euler(0, 0, halfAngle) * forward * Range;
            Vector3 rightBound = Quaternion.Euler(0, 0, -halfAngle) * forward * Range;

            Gizmos.DrawLine(center, center + leftBound);
            Gizmos.DrawLine(center, center + rightBound);

            // 호 그리기 (간단히)
            int segments = 20;
            Vector3 prevPoint = center + leftBound;
            for (int i = 1; i <= segments; i++) {
                float angle = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
                Vector3 point = center + Quaternion.Euler(0, 0, angle) * forward * Range;
                Gizmos.DrawLine(prevPoint, point);
                prevPoint = point;
            }
        }

        private void DrawRectangleGizmo(Vector3 center) {
            Vector3 forward = Forward;
            Vector3 boxCenter = center + forward * (Range * 0.5f);
            Vector3 size = new Vector3(Width, Range, 0.1f);

            Gizmos.matrix = Matrix4x4.TRS(boxCenter, _attackPoint.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = Matrix4x4.identity;
        }

        private void DrawLineGizmo(Vector3 center) {
            Vector3 forward = Forward;
            Vector3 endPoint = center + forward * Range;

            Gizmos.DrawLine(center, endPoint);

            if (Width > 0) {
                Vector3 right = _attackPoint.right;
                float halfWidth = Width * 0.5f;

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
            GameDebug.Log($"스킬 범위: {Range}");
            GameDebug.Log($"스킬 타입: {_skillData?.RangeType}");
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
        /// 무기 장착 (WeaponSystem에 위임)
        /// </summary>
        public void Equip(GameObject owner) {
            if (_isInitialized) {
                Unequip();
            }
            _owner = owner;
            _weaponId = owner.GetInstanceID();
            SetAttackPoint(owner.transform);

            _weaponSystem.EquipWeapon(this, owner);
        }

        /// <summary>
        /// 장착 해제 (WeaponSystem에 위임)
        /// </summary>
        public void Unequip() {
            _weaponSystem.UnequipWeapon(this);
        }
        #endregion
    }
}