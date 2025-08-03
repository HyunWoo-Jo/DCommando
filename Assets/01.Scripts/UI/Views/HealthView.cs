using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;
using Game.ViewModels;
using UnityEngine.Assertions;
using Game.Core;
using Zenject;
using DG.Tweening;

namespace Game.UI
{
    public class HealthView : MonoBehaviour, IHealthInjecter {
        [Inject] private readonly HealthViewModel.Factory _viewModelFactory;
        private HealthViewModel _viewModel; // 외부에서 주입

        [Header("Unity 레퍼")]
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private Color _normalHealthColor = Color.green;
        [SerializeField] private Color _lowHealthColor = Color.red;
        [SerializeField] private Color _deadHealthColor = Color.black;

        // Data
        private float _lastRatio = 1f;
        private const float DURATION_SCALE = 6f;
        // owner
        private Transform _ownerTr;
        private Vector3 _uiOffset;

        /// <summary>
        /// Systems.HealthComponent에서 EventBus를 통해 주입
        /// </summary>
        public void InjectHealth(object healthModel, GameObject obj, Vector2 offset) {
            _ownerTr = obj.transform;
            _uiOffset = offset;

            // UI 위치 변경 구독
            Observable.EveryValueChanged(_ownerTr, tr => tr.position)
                .Subscribe(pos => UpdateWorldToScreen(pos))
                .AddTo(this);
                

            // Model은 Systems.HealthComponent에서 생성
            _viewModel = _viewModelFactory.Create(); // ViewModel 생성
            _viewModel.Initialize(healthModel);
            Bind();
        }

        private void OnDestroy() {
            _viewModel?.Dispose();
        }

        /// <summary>
        /// UI 바인딩
        /// </summary>
        private void Bind() {
            // 체력 텍스트 바인딩
            if (_healthText != null) {
                _viewModel.RORP_HealthText
                    .Subscribe(text => _healthText.text = text)
                    .AddTo(this);
            }

            // 체력 바 바인딩
            if (_healthBarFill != null) {
                _viewModel.RORP_HealthRatio
                    .Subscribe(AnimateHealthBar)
                    .AddTo(this);
            }

            // 저체력 상태에 따른 색상 변경
            if (_healthBarFill != null) {
                _viewModel.RORP_IsLowHealth
                    .Subscribe(isLowHealth => UpdateHealthBarColor(isLowHealth))
                    .AddTo(this);
            }

            // 사망 상태에 따른 색상 변경
            _viewModel.RORP_IsDead
                .Subscribe(isDead => UpdateDeadState(isDead))
                .AddTo(this);
        }


        /// <summary>
        /// 체력바 색상 업데이트
        /// </summary>
        private void UpdateHealthBarColor(bool isLowHealth) {
            if (_healthBarFill != null && !_viewModel.IsDead) {
                _healthBarFill.color = isLowHealth ? _lowHealthColor : _normalHealthColor;
            }
        }

        /// <summary>
        /// 사망 상태 업데이트
        /// </summary>
        private void UpdateDeadState(bool isDead) {
            if (_healthBarFill != null && isDead) {
                _healthBarFill.color = _deadHealthColor;
            } else if (_healthBarFill != null && !isDead) {
                // 사망 상태가 아닐 때는 저체력 상태에 따라 색상 결정
                UpdateHealthBarColor(_viewModel.RORP_IsLowHealth.CurrentValue);
            }
        }
        /// <summary>
        /// Tween 애니메이션 바
        /// </summary>
        private void AnimateHealthBar(float targetRatio) {
            _healthBarFill.DOKill();

            float duration = Mathf.Abs(targetRatio - _lastRatio) * DURATION_SCALE;
            Ease easeType = targetRatio < _lastRatio ? Ease.OutQuart : Ease.InOutQuad;

            _healthBarFill.DOFillAmount(targetRatio, duration)
                .SetEase(easeType)
                .OnComplete(() => _lastRatio = targetRatio);
        }
        /// <summary>
        /// UI 위치 업데이트
        /// </summary>
        private void UpdateWorldToScreen(Vector3 worldPos) {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + _uiOffset);
            transform.position = screenPos;
        }

#if UNITY_EDITOR
        // 테스트용 메서드들 (Inspector에서 호출 가능)
        [ContextMenu("Take Damage 10")]
        private void TestTakeDamage() {
            GameDebug.Log("Damage 10");
            _viewModel?.TakeDamage(10);
        }

        [ContextMenu("Heal 20")]
        private void TestHeal() {
            GameDebug.Log("Heal 20");
            _viewModel?.Heal(20);
        }

        [ContextMenu("Revive")]
        private void TestRevive() {
            _viewModel?.Revive();
        }
#endif
    }
}