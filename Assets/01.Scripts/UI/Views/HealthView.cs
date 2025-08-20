using UnityEngine;
using UnityEngine.UI;
using TMPro;
using R3;
using Zenject;
using DG.Tweening;

using Game.ViewModels;
using Game.Core;
using static UnityEditor.PlayerSettings;

namespace Game.UI.Views {
    public class HealthView : MonoBehaviour, IHealthInitializable {
        [Inject] private HealthViewModel _healthViewModel;

        [Header("Unity 레퍼런스")]
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private GameObject _healthBarObject;
        [SerializeField] private TextMeshProUGUI _damageText;

        [Header("색상 설정")]
        [SerializeField] private Color _normalHealthColor = Color.green;
        [SerializeField] private Color _lowHealthColor = Color.yellow;
        [SerializeField] private Color _criticalHealthColor = Color.red;
        [SerializeField] private Color _deadHealthColor = Color.black;

        [Header("애니메이션 설정")]
        [SerializeField] private float _barAnimationDuration = 0.3f;
        [SerializeField] private float _colorAnimationDuration = 0.2f;
        [SerializeField] private Ease _animationEase = Ease.OutQuart;

        // Data
        private float _lastRatio = 1f;
        private Color _lastColor;
        private CompositeDisposable _disposables = new();

        // Owner 정보
        private Transform _ownerTr;
        private Vector3 _uiOffset;
        private int _ownerID;


        // Tweens;
        private Tween _lowHealthBlinkTween;

        /// <summary>
        /// HealthComponent에서 EventBus를 통해 주입
        /// </summary>
        public void InitHealth(GameObject obj, Vector2 offset) {
            _ownerTr = obj.transform;
            _uiOffset = offset;
            _ownerID = obj.GetInstanceID();
            _lastColor = _normalHealthColor;

            // UI 위치 업데이트 구독
            Observable.EveryUpdate()
                .Where(_ => _ownerTr != null && _ownerTr)  // Unity destroyed 객체 체크
                .Select(_ => _ownerTr.position)
                .DistinctUntilChanged()  // 위치가 변경될 때만 업데이트
                .Subscribe(pos => UpdateWorldToScreen(pos))
                .AddTo(_disposables);

            Bind();
        }
        private void OnDestroy() {
            // 애니메이션 정리
            
            _healthBarFill?.DOKill();
            if (_healthBarObject != null) {
                _healthBarObject.transform.DOKill();
            }
           
            // 구독 정리
            _disposables?.Dispose();
        }

        /// <summary>
        /// 데이터 바인딩
        /// </summary>
        private void Bind() {
            // 체력 텍스트 바인딩
            _healthViewModel.GetHealthDisplayTextProperty(_ownerID)?
                .Subscribe(UpdateHealthText)
                .AddTo(_disposables);

            // 체력 비율 바인딩 (애니메이션)
            _healthViewModel.GetHealthRatioProperty(_ownerID)?
                .Subscribe(UpdateHealthBar)
                .AddTo(_disposables);

            // 체력 색상 바인딩 (부드러운 전환)
            _healthViewModel.GetHealthRatioProperty(_ownerID)?
                .Subscribe(UpdateHealthColor)
                .AddTo(_disposables);

            // 사망 상태 바인딩
            _healthViewModel.GetIsDeadProperty(_ownerID)?
                .Subscribe(UpdateDeathState)
                .AddTo(_disposables);

            // 저체력 상태 바인딩 (추가 효과용)
            _healthViewModel.GetLowHealthProperty(_ownerID)?
                .Subscribe(UpdateLowHealthEffect)
                .AddTo(_disposables);

            // 알림 구독
            _healthViewModel.OnHealthNotification
                .Subscribe(OnHealthNotification)
                .AddTo(_disposables);
        }

        #region UI 업데이트 메서드
        /// <summary>
        /// 체력 텍스트 업데이트
        /// </summary>
        private void UpdateHealthText(string healthText) {
            if (_healthText != null) {
                _healthText.text = healthText;
            }
        }

        /// <summary>
        /// 체력 바 업데이트 (애니메이션)
        /// </summary>
        private void UpdateHealthBar(float ratio) {
            if (_healthBarFill == null) return;

            // 애니메이션으로 부드럽게 변경
            _healthBarFill.DOFillAmount(ratio, _barAnimationDuration)
                .SetEase(_animationEase)
                .OnComplete(() => _lastRatio = ratio);
        }

        /// <summary>
        /// 체력 색상 업데이트 (부드러운 전환)
        /// </summary>
        private void UpdateHealthColor(float ratio) {
            if (_healthBarFill == null) return;
            // 추후 스타일로 변경
            Color targetColor;
            if (ratio == 0) {
                targetColor = _deadHealthColor;
            } else if (ratio <= 0.1f) {
                targetColor = _criticalHealthColor;
            } else if(ratio <= 0.3f) {
                targetColor = _lowHealthColor;
            } else {
                targetColor = _normalHealthColor;
            }

            if (_lastColor != targetColor) {
                // 색상 애니메이션
                _healthBarFill.DOColor(targetColor, _colorAnimationDuration)
                    .SetEase(_animationEase)
                    .OnComplete(() => _lastColor = targetColor);
                _lastColor = targetColor;
            }
        }

        /// <summary>
        /// 사망 상태 업데이트
        /// </summary>
        private void UpdateDeathState(bool isDead) {
            if (_healthBarObject != null) {
                // 사망 시 체력바 숨김 또는 특별한 표시
                if (isDead) {
                    _healthBarFill.color = _deadHealthColor;
                    // 사망 애니메이션 (예: 페이드 아웃)
                    _healthBarObject.transform.DOScale(0.8f, 0.3f)
                        .SetEase(Ease.InBack);
                } else {
                    // 부활 시 원래 크기로
                    _healthBarObject.transform.DOScale(1f, 0.3f)
                        .SetEase(Ease.OutBack);
                }
            }
        }

        /// <summary>
        /// 저체력 효과 업데이트
        /// </summary>
        private void UpdateLowHealthEffect(bool isLowHealth) {
            if (_healthBarFill == null) return;

            if (isLowHealth) {
                // 저체력 시 깜빡이는 효과
                _lowHealthBlinkTween = _healthBarFill.DOFade(0.5f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine);
            } else {
                // 깜빡임 중지
                _lowHealthBlinkTween?.Kill();
                _healthBarFill.DOFade(1f, 0.2f);
            }
        }

        /// <summary>
        /// 월드 좌표를 스크린 좌표로 변환
        /// </summary>
        private void UpdateWorldToScreen(Vector3 worldPos) {
            if (Camera.main == null) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + _uiOffset);
            transform.position = screenPos;
        }
        #endregion

        #region 이벤트 핸들러
        /// <summary>
        /// 체력 알림 처리
        /// </summary>
        private void OnHealthNotification(string message) {
            Debug.Log($"[HealthView {_ownerID}] {message}");
        }
        #endregion

        #region 외부 인터페이스
        /// <summary>
        /// 데미지 처리
        /// </summary>
        public void TakeDamage(int damage) {
            _healthViewModel.TakeDamage(_ownerID, damage);
        }

        /// <summary>
        /// 치료 처리
        /// </summary>
        public void Heal(int healAmount) {
            _healthViewModel.Heal(_ownerID, healAmount);
        }

        /// <summary>
        /// 부활 처리
        /// </summary>
        public void Revive(int reviveHp = -1) {
            _healthViewModel.Revive(_ownerID, reviveHp);
        }

        /// <summary>
        /// 최대 체력 설정
        /// </summary>
        public void SetMaxHp(int maxHp) {
            _healthViewModel.SetMaxHp(_ownerID, maxHp);
        }
        #endregion


        #region 에디터 테스트 메서드
#if UNITY_EDITOR
        [ContextMenu("Test Take Damage (10)")]
        private void TestTakeDamage() {
            TakeDamage(10);
        }

        [ContextMenu("Test Heal (20)")]
        private void TestHeal() {
            Heal(20);
        }

        [ContextMenu("Test Revive")]
        private void TestRevive() {
            Revive();
        }

        [ContextMenu("Test Set Max HP (150)")]
        private void TestSetMaxHp() {
            SetMaxHp(150);
        }

#endif
        #endregion
    }
}