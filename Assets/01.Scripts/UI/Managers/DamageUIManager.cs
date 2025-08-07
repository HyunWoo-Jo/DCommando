using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using Zenject;
using DG.Tweening;
using Game.Core;
using Game.Core.Event;
using Game.ViewModels;
using R3;

namespace Game.UI {
    /// <summary>
    /// ������ UI �Ŵ��� - �÷��� ������ �ؽ�Ʈ�� Object Pool�� ����
    /// </summary>
    public class DamageUIManager : MonoBehaviour {
        [Header("Dependencies")]
        [Inject] private UIViewModel _viewModel;
        [Inject] private SO_DamageUIStyle _damageUIStyle;

        [Header("Pool Settings")]
        [SerializeField] private int _poolDefaultCapacity = 20;
        [SerializeField] private int _poolMaxSize = 100;

        // ���� ����
        private Dictionary<int, Transform> _healthPositionDict = new();
        private CompositeDisposable _disposables = new();
        private ObjectPool<TextMeshProUGUI> _damageUIPool;
        private GameObject _damageUIPrefab;
        private Transform _damageUIParent;

        private void Start() {
            Bind();
            InitializeDamageUIAsync();
        }

        private void OnDestroy() {
            _disposables?.Dispose();
            _damageUIPool?.Dispose();
            _viewModel?.ReleaseDamageUI();
        }

        // ���ε�
        private void Bind() {
            EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenedHealthUI)
                .AddTo(_disposables);
            EventBus.Subscribe<UIClosedNotificationEvent>(OnClosedHealthUI)
                .AddTo(_disposables);
            EventBus.Subscribe<DamageTakenEvent>(OnDamageUI)
                .AddTo(_disposables);
            EventBus.Subscribe<HealedEvent>(OnHealUI)
                .AddTo(_disposables);
        }

        // ������ UI �ʱ�ȭ
        private async void InitializeDamageUIAsync() {
            try {
                // �񵿱�� ������ �ε�
                _damageUIPrefab = await _viewModel.LoadDamageUIPrefabAsync();
                _damageUIParent = _viewModel.GetParent(UIType.HUD);

                // Object Pool ����
                _damageUIPool = new ObjectPool<TextMeshProUGUI>(
                    createFunc: CreateDamageUI,
                    actionOnGet: OnGetDamageUI,
                    actionOnRelease: OnReleaseDamageUI,
                    actionOnDestroy: OnDestroyDamageUI,
                    collectionCheck: true,
                    defaultCapacity: _poolDefaultCapacity,
                    maxSize: _poolMaxSize
                );

                GameDebug.Log($"DamageUIManager �ʱ�ȭ �Ϸ� Pool Size: {_poolDefaultCapacity}");
            } catch (System.Exception e) {
                GameDebug.LogError($"DamageUIManager �ʱ�ȭ ����: {e.Message}");
            }
        }

        #region Object Pool ����

        // ������Ʈ ����
        private TextMeshProUGUI CreateDamageUI() {
            if (_damageUIPrefab == null || _damageUIParent == null) {
                GameDebug.LogError("DamageUI Prefab �Ǵ� Parent�� null");
                return null;
            }

            GameObject obj = Instantiate(_damageUIPrefab, _damageUIParent);
            return obj.GetComponent<TextMeshProUGUI>();
        }

        // Ǯ���� ������ ��
        private void OnGetDamageUI(TextMeshProUGUI textComp) {
            textComp.gameObject.SetActive(true);
            if (textComp != null) {
                textComp.alpha = 1.0f;
            }

            // Ʈ�� ����
            textComp.gameObject.transform.DOKill();
        }

        // Ǯ�� ��ȯ�� ��
        private void OnReleaseDamageUI(TextMeshProUGUI textComp) {
            if (textComp != null) {
                var obj = textComp.gameObject;
                obj.SetActive(false);
                obj.transform.DOKill();
            }
        }

        // ������Ʈ �ı��� ��
        private void OnDestroyDamageUI(TextMeshProUGUI textComp) {
            if (textComp != null) {
                textComp.transform.DOKill();
                Destroy(textComp.gameObject);
            }
        }

        #endregion

        #region UI �̺�Ʈ ó��

        // Health UI ���� �� ��ġ ���
        private void OnOpenedHealthUI(UIOpenedNotificationEvent openedEvent) {
            if (openedEvent.uiName == UIName.Health_UI) {
                _healthPositionDict[openedEvent.id] = openedEvent.uiObject.transform;
                GameDebug.Log($"Health UI ��ġ ��� Character {openedEvent.id}");
            }
        }

        // Health UI ���� �� ��ġ ����
        private void OnClosedHealthUI(UIClosedNotificationEvent closedEvent) {
            if (closedEvent.uiName == UIName.Health_UI) {
                // ID�� ���� ������ �� �����Ƿ� null�� Transform ����
                var keysToRemove = new List<int>();
                foreach (var kvp in _healthPositionDict) {
                    if (kvp.Value == null) {
                        keysToRemove.Add(kvp.Key);
                    }
                }

                foreach (var key in keysToRemove) {
                    _healthPositionDict.Remove(key);
                    GameDebug.Log($"Health UI ��ġ ���� Character {key}");
                }
            }
        }

        #endregion

        #region ������/ġ�� UI ����

        // ������ UI ���� (�̺�Ʈ���� ȣ��)
        private void OnDamageUI(DamageTakenEvent damageEvent) {
            ShowDamageUI(damageEvent.characterID, damageEvent.damage, damageEvent.type);
        }

        // ġ�� UI ���� (�̺�Ʈ���� ȣ��)
        private void OnHealUI(HealedEvent healedEvent) {
            ShowDamageUI(healedEvent.characterID, healedEvent.healAmount, DamageType.Heal);
        }

        // ������ UI ���� (���� �޼���)
        public void ShowDamageUI(int characterId, int amount, DamageType damageType, bool isCritical = false) {
            if (!CanShowDamageUI(characterId)) return;

            var textComp = _damageUIPool.Get();
            if (textComp == null) {
                GameDebug.LogError("TextMeshProUGUI ������Ʈ�� ã�� �� ����");
                _damageUIPool.Release(textComp);
                return;
            }

            // UI ����
            SetupDamageUI(textComp, amount, damageType, isCritical);

            // ��ġ ����
            Vector3 worldPos = _healthPositionDict[characterId].position;
            Vector3 startPos = worldPos + _damageUIStyle.StartOffset;
            textComp.gameObject.transform.position = startPos;

            // �ִϸ��̼� ����
            StartDamageAnimation(textComp, isCritical);

            GameDebug.Log($"������ UI ���� Character {characterId}: {amount} {damageType} damage");
        }

        // Miss UI ����
        public void ShowMissUI(int characterId) {
            if (!CanShowDamageUI(characterId)) return;


            var textComp = _damageUIPool.Get();
            if (textComp == null) {
                GameDebug.LogError("TextMeshProUGUI ������Ʈ�� ã�� �� ����");
                _damageUIPool.Release(textComp);
                return;
            }

            // Miss UI ����
            textComp.text = "MISS";
            textComp.color = _damageUIStyle.MissColor;

            // ��ġ ����
            Vector3 worldPos = _healthPositionDict[characterId].position;
            Vector3 startPos = worldPos + _damageUIStyle.StartOffset;
            textComp.gameObject.transform.position = startPos;

            // Miss �ִϸ��̼�
            StartMissAnimation(textComp);

            GameDebug.Log($"Miss UI ���� Character {characterId}");
        }

        #endregion

        #region UI ���� �� �ִϸ��̼�

        // ������ UI ����
        private void SetupDamageUI(TextMeshProUGUI textComp, int amount, DamageType damageType, bool isCritical) {
            // �ؽ�Ʈ ����
            if (damageType == DamageType.Heal) {
                textComp.text = $"+{amount}";
            } else {
                string criticalMark = isCritical ? "!" : "";
                textComp.text = $"-{amount}{criticalMark}";
            }

            // ���� ����
            textComp.color = _damageUIStyle.GetDamageTypeColor(damageType);

            // ũ��Ƽ���� ��� ��Ʈ ũ�� ����
            if (isCritical && damageType != DamageType.Heal) {
                var criticalStyle = _damageUIStyle.GetFloatingUIStyle(DamageUIType.Critical);
                textComp.fontSize *= criticalStyle.fontSizeMultiplier;
            }
        }

        // ������ �ִϸ��̼�
        private void StartDamageAnimation(TextMeshProUGUI textComp, bool isCritical) {
            var obj = textComp.gameObject;
            Vector3 startPos = obj.transform.position;
            Vector3 endPos = startPos + _damageUIStyle.EndOffset;

            // ��Ÿ�� ��������
            var styleData = isCritical ?
                _damageUIStyle.GetFloatingUIStyle(DamageUIType.Critical) :
                _damageUIStyle.GetFloatingUIStyle(DamageUIType.Normal);

            // ������ ����
            var sequence = DOTween.Sequence();

            // ���� �̵�
            sequence.Append(
                obj.transform.DOMove(endPos, styleData.animationDuration)
                    .SetEase(_damageUIStyle.MovementCurve)
            );

            // ũ��Ƽ�� �ִϸ��̼�
            if (isCritical) {
                // ������ ȿ��
                sequence.Join(
                    obj.transform.DOScale(Vector3.one * styleData.scaleMultiplier, 0.2f)
                        .SetEase(Ease.OutBack)
                        .OnComplete(() => obj.transform.DOScale(Vector3.one, 0.2f))
                );

                // ��¦�� ȿ��
                sequence.Insert(0.1f,
                    textComp.DOColor(Color.white, 0.1f)
                        .SetLoops(2, LoopType.Yoyo)
                );
            }

            // ���̵� �ƿ�
            sequence.Insert(_damageUIStyle.FadeDelay,
                textComp.DOFade(0f, styleData.animationDuration - _damageUIStyle.FadeDelay)
                    .SetEase(Ease.InQuad)
            );

            // �Ϸ� �� Ǯ�� ��ȯ
            sequence.OnComplete(() => _damageUIPool.Release(textComp));
        }

        // Miss �ִϸ��̼�
        private void StartMissAnimation(TextMeshProUGUI textComp) {
            var obj = textComp.gameObject;
            var styleData = _damageUIStyle.GetFloatingUIStyle(DamageUIType.Miss);

            Vector3 startPos = obj.transform.position;
            Vector3 endPos = startPos + _damageUIStyle.EndOffset;

            // ������ ����
            var sequence = DOTween.Sequence();

            // ���� �̵�
            sequence.Append(
                obj.transform.DOMove(endPos, styleData.animationDuration)
                    .SetEase(_damageUIStyle.MovementCurve)
            );

            // �¿� ��鸲
            sequence.Join(
                obj.transform.DOShakePosition(0.5f, new Vector3(20f, 0, 0), 8, 90f)
            );

            // ���̵� �ƿ�
            sequence.Insert(_damageUIStyle.FadeDelay,
                textComp.DOFade(0f, styleData.animationDuration - _damageUIStyle.FadeDelay)
                    .SetEase(Ease.InQuad)
            );

            // �Ϸ� �� Ǯ�� ��ȯ
            sequence.OnComplete(() => _damageUIPool.Release(textComp));
        }

        #endregion

        #region ��ƿ��Ƽ

        // ������ UI ǥ�� ���� ���� Ȯ��
        private bool CanShowDamageUI(int characterId) {
            if (_damageUIPool == null) {
                GameDebug.LogWarning("DamageUI Pool�� �ʱ�ȭ���� ����");
                return false;
            }

            if (!_healthPositionDict.TryGetValue(characterId, out Transform healthTransform)) {
                GameDebug.LogWarning($"Health UI ��ġ�� ã�� �� ���� Character {characterId}");
                return false;
            }

            if (healthTransform == null) {
                GameDebug.LogWarning($"Health UI Transform�� null Character {characterId}");
                _healthPositionDict.Remove(characterId);
                return false;
            }

            return true;
        }

        // Ǯ ���� ���� (����׿�)
        public string GetPoolStatus() {
            if (_damageUIPool == null) return "Pool not initialized";

            return $"Pool Status - Active: {_damageUIPool.CountActive}, " +
                   $"Inactive: {_damageUIPool.CountInactive}, " +
                   $"Total: {_damageUIPool.CountAll}";
        }

        #endregion
    }
}