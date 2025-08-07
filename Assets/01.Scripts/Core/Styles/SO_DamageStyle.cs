using UnityEngine;
using Game.Core;

namespace Game.Core {
    /// <summary>
    /// ������ UI ��Ÿ�� ���� ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "DamageUIStyle", menuName = "Styles/DamageUIStyle")]
    public class SO_DamageUIStyle : ScriptableObject {
        [Header("������ Ÿ�Ժ� ����")]
        [SerializeField]
        private DamageTypeColorData[] _damageTypeColors = new DamageTypeColorData[]
        {
            new DamageTypeColorData { damageType = DamageType.Physical, color = Color.red, displayName = "����" },
            new DamageTypeColorData { damageType = DamageType.Magic, color = Color.blue, displayName = "����" },
            new DamageTypeColorData { damageType = DamageType.Fire, color = new Color(1f, 0.5f, 0f), displayName = "ȭ��" },
            new DamageTypeColorData { damageType = DamageType.Ice, color = Color.cyan, displayName = "����" },
            new DamageTypeColorData { damageType = DamageType.Poison, color = Color.green, displayName = "��" },
            new DamageTypeColorData { damageType = DamageType.Lightning, color = Color.yellow, displayName = "����" },
            new DamageTypeColorData { damageType = DamageType.Pure, color = Color.white, displayName = "����" },
            new DamageTypeColorData { damageType = DamageType.Heal, color = new Color(0f, 1f, 0.5f), displayName = "ȸ��" }
        };

        [Header("�÷��� UI Ÿ�Ժ� ����")]
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

        [Header("�ִϸ��̼� ����")]
        [SerializeField] private Vector3 _startOffset = new Vector3(0, 50f, 0);
        [SerializeField] private Vector3 _endOffset = new Vector3(0, 150f, 0);
        [SerializeField] private float _fadeDelay = 0.5f;
        [SerializeField] private AnimationCurve _movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Miss ���� ����")]
        [SerializeField] private Color _missColor = Color.gray;

        // ������Ƽ
        public Vector3 StartOffset => _startOffset;
        public Vector3 EndOffset => _endOffset;
        public float FadeDelay => _fadeDelay;
        public AnimationCurve MovementCurve => _movementCurve;
        public Color MissColor => _missColor;

        // ������ Ÿ�Ժ� ���� ��������
        public Color GetDamageTypeColor(DamageType damageType) {
            foreach (var data in _damageTypeColors) {
                if (data.damageType == damageType) {
                    return data.color;
                }
            }

            // �⺻�� (���� ������)
            return Color.red;
        }

        // ������ Ÿ�Ժ� ǥ�� �̸� ��������
        public string GetDamageTypeDisplayName(DamageType damageType) {
            foreach (var data in _damageTypeColors) {
                if (data.damageType == damageType) {
                    return data.displayName;
                }
            }

            return damageType.ToString();
        }

        // �÷��� UI Ÿ�Ժ� ��Ÿ�� ��������
        public FloatingUIStyleData GetFloatingUIStyle(DamageUIType uiType) {
            foreach (var data in _floatingUIStyles) {
                if (data.uiType == uiType) {
                    return data;
                }
            }

            // �⺻��
            return new FloatingUIStyleData {
                uiType = DamageUIType.Normal,
                fontSizeMultiplier = 1.0f,
                animationDuration = 1.5f,
                scaleMultiplier = 1.0f
            };
        }

        // ��� ������ Ÿ�� ���� ������ ��ȯ (�����Ϳ�)
        public DamageTypeColorData[] GetAllDamageTypeColors() {
            return _damageTypeColors;
        }

        // ��� �÷��� UI ��Ÿ�� ������ ��ȯ (�����Ϳ�)
        public FloatingUIStyleData[] GetAllFloatingUIStyles() {
            return _floatingUIStyles;
        }

#if UNITY_EDITOR
        // �����Ϳ��� �� ����
        private void OnValidate()
        {
            // ������ Ÿ���� ��� �����Ǿ� �ִ��� Ȯ��
            var allDamageTypes = System.Enum.GetValues(typeof(DamageType));
            if (_damageTypeColors.Length != allDamageTypes.Length)
            {
                Debug.LogWarning($"DamageUIStyle: ��� ������ Ÿ���� �������� �ʾҽ��ϴ�. ����: {_damageTypeColors.Length}, �ʿ�: {allDamageTypes.Length}");
            }
            
            // �÷��� UI Ÿ���� ��� �����Ǿ� �ִ��� Ȯ��
            var allDamageUITypes = System.Enum.GetValues(typeof(DamageUIType));
            if (_floatingUIStyles.Length != allDamageUITypes.Length)
            {
                Debug.LogWarning($"DamageUIStyle: ��� �÷��� UI Ÿ���� �������� �ʾҽ��ϴ�. ����: {_floatingUIStyles.Length}, �ʿ�: {allDamageUITypes.Length}");
            }
        }
#endif
    }

    /// <summary>
    /// ������ Ÿ�Ժ� ���� ������
    /// </summary>
    [System.Serializable]
    public struct DamageTypeColorData {
        [Header("������ Ÿ��")]
        public DamageType damageType;

        [Header("UI ����")]
        public Color color;

        [Header("ǥ�� �̸�")]
        public string displayName;
    }

    /// <summary>
    /// �÷��� UI Ÿ�Ժ� ��Ÿ�� ������
    /// </summary>
    [System.Serializable]
    public struct FloatingUIStyleData {
        [Header("UI Ÿ��")]
        public DamageUIType uiType;

        [Header("��Ʈ ũ�� ����")]
        [Range(0.1f, 3.0f)]
        public float fontSizeMultiplier;

        [Header("�ִϸ��̼� ���� �ð�")]
        [Range(0.5f, 5.0f)]
        public float animationDuration;

        [Header("������ ����")]
        [Range(0.1f, 3.0f)]
        public float scaleMultiplier;
    }

   
}