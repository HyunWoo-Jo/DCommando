using UnityEngine;
using Game.Core;

namespace Game.Core {
    /// <summary>
    /// 데미지 UI 스타일 설정 ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "DamageUIStyle", menuName = "Styles/DamageUIStyle")]
    public class SO_DamageUIStyle : ScriptableObject {
        [Header("데미지 타입별 색상")]
        [SerializeField]
        private DamageTypeColorData[] _damageTypeColors = new DamageTypeColorData[]
        {
            new DamageTypeColorData { damageType = DamageType.Physical, color = Color.red, displayName = "물리" },
            new DamageTypeColorData { damageType = DamageType.Magic, color = Color.blue, displayName = "마법" },
            new DamageTypeColorData { damageType = DamageType.Fire, color = new Color(1f, 0.5f, 0f), displayName = "화염" },
            new DamageTypeColorData { damageType = DamageType.Ice, color = Color.cyan, displayName = "얼음" },
            new DamageTypeColorData { damageType = DamageType.Poison, color = Color.green, displayName = "독" },
            new DamageTypeColorData { damageType = DamageType.Lightning, color = Color.yellow, displayName = "번개" },
            new DamageTypeColorData { damageType = DamageType.Pure, color = Color.white, displayName = "순수" },
            new DamageTypeColorData { damageType = DamageType.Heal, color = new Color(0f, 1f, 0.5f), displayName = "회복" }
        };

        [Header("플로팅 UI 타입별 설정")]
        [SerializeField]
        private FloatingUIStyleData[] _floatingUIStyles = new FloatingUIStyleData[]
        {
            new FloatingUIStyleData
            {
                uiType = DamageUIType.Normal,
                fontSizeMultiplier = 1.0f,
                animationDuration = 1.5f,
                scaleMultiplier = 1.0f
            },
            new FloatingUIStyleData
            {
                uiType = DamageUIType.Critical,
                fontSizeMultiplier = 1.3f,
                animationDuration = 2.0f,
                scaleMultiplier = 1.5f
            },
            new FloatingUIStyleData
            {
                uiType = DamageUIType.Miss,
                fontSizeMultiplier = 0.8f,
                animationDuration = 1.2f,
                scaleMultiplier = 0.9f
            }
        };

        [Header("애니메이션 설정")]
        [SerializeField] private Vector3 _startOffset = new Vector3(0, 50f, 0);
        [SerializeField] private Vector3 _endOffset = new Vector3(0, 150f, 0);
        [SerializeField] private float _fadeDelay = 0.5f;
        [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Miss 전용 색상")]
        [SerializeField] private Color _missColor = Color.gray;

        // 프로퍼티
        public Vector3 StartOffset => _startOffset;
        public Vector3 EndOffset => _endOffset;
        public float FadeDelay => _fadeDelay;
        public AnimationCurve MovementCurve => _movementCurve;
        public Color MissColor => _missColor;

        // 데미지 타입별 색상 가져오기
        public Color GetDamageTypeColor(DamageType damageType) {
            foreach (var data in _damageTypeColors) {
                if (data.damageType == damageType) {
                    return data.color;
                }
            }

            // 기본값 (물리 데미지)
            return Color.red;
        }

        // 데미지 타입별 표시 이름 가져오기
        public string GetDamageTypeDisplayName(DamageType damageType) {
            foreach (var data in _damageTypeColors) {
                if (data.damageType == damageType) {
                    return data.displayName;
                }
            }

            return damageType.ToString();
        }

        // 플로팅 UI 타입별 스타일 가져오기
        public FloatingUIStyleData GetFloatingUIStyle(DamageUIType uiType) {
            foreach (var data in _floatingUIStyles) {
                if (data.uiType == uiType) {
                    return data;
                }
            }

            // 기본값
            return new FloatingUIStyleData {
                uiType = DamageUIType.Normal,
                fontSizeMultiplier = 1.0f,
                animationDuration = 1.5f,
                scaleMultiplier = 1.0f
            };
        }

        // 모든 데미지 타입 색상 데이터 반환 (에디터용)
        public DamageTypeColorData[] GetAllDamageTypeColors() {
            return _damageTypeColors;
        }

        // 모든 플로팅 UI 스타일 데이터 반환 (에디터용)
        public FloatingUIStyleData[] GetAllFloatingUIStyles() {
            return _floatingUIStyles;
        }

#if UNITY_EDITOR
        // 에디터에서 값 검증
        private void OnValidate()
        {
            // 데미지 타입이 모두 설정되어 있는지 확인
            var allDamageTypes = System.Enum.GetValues(typeof(DamageType));
            if (_damageTypeColors.Length != allDamageTypes.Length)
            {
                Debug.LogWarning($"DamageUIStyle: 모든 데미지 타입이 설정되지 않았습니다. 현재: {_damageTypeColors.Length}, 필요: {allDamageTypes.Length}");
            }
            
            // 플로팅 UI 타입이 모두 설정되어 있는지 확인
            var allDamageUITypes = System.Enum.GetValues(typeof(DamageUIType));
            if (_floatingUIStyles.Length != allDamageUITypes.Length)
            {
                Debug.LogWarning($"DamageUIStyle: 모든 플로팅 UI 타입이 설정되지 않았습니다. 현재: {_floatingUIStyles.Length}, 필요: {allDamageUITypes.Length}");
            }
        }
#endif
    }

    /// <summary>
    /// 데미지 타입별 색상 데이터
    /// </summary>
    [System.Serializable]
    public struct DamageTypeColorData {
        [Header("데미지 타입")]
        public DamageType damageType;

        [Header("UI 색상")]
        public Color color;

        [Header("표시 이름")]
        public string displayName;
    }

    /// <summary>
    /// 플로팅 UI 타입별 스타일 데이터
    /// </summary>
    [System.Serializable]
    public struct FloatingUIStyleData {
        [Header("UI 타입")]
        public DamageUIType uiType;

        [Header("폰트 크기 배율")]
        [Range(0.1f, 3.0f)]
        public float fontSizeMultiplier;

        [Header("애니메이션 지속 시간")]
        [Range(0.5f, 5.0f)]
        public float animationDuration;

        [Header("스케일 배율")]
        [Range(0.1f, 3.0f)]
        public float scaleMultiplier;
    }

   
}