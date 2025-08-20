using UnityEngine;
using UnityEngine.UI;
using R3;
using TMPro;
using Game.ViewModels;
using Game.Core;
using Zenject;
using DG.Tweening;
using Game.Core.Event;

namespace Game.UI {
    public class ExpView : MonoBehaviour {
        [Inject] private ExpViewModel _viewModel;

        [Header("Unity 레퍼")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Image _expBarFill;

        // Data
        private float _lastProgress = 0f;
        private const float DURATION_SCALE = 3f;

        private void Awake() {
            _expBarFill.fillAmount = 0;
            Bind();
        }

        private void OnDestroy() {
            _viewModel?.Dispose();
        }

        private void Bind() {
            if (_levelText != null) {
                _viewModel.RORP_LevelText
                    .Subscribe(text => _levelText.text = text)
                    .AddTo(this);
            }

            if (_expBarFill != null) {
                _viewModel.RORP_ExpProgress
                    .Subscribe(AnimateExpBar)
                    .AddTo(this);
            }

        }


        private void AnimateExpBar(float targetProgress) {
            _expBarFill.DOKill();

            // 레벨업 감지 (현재 진행도가 목표보다 높으면 레벨업 발생)
            if (_lastProgress > targetProgress) {
                // 레벨업 애니메이션
                AnimateLevelUpSequence(targetProgress);
            } else {
                // 일반 애니메이션
                AnimateNormalProgress(targetProgress);
            }
        }

        /// <summary>
        /// 레벨업 시퀀스 애니메이션
        /// </summary>
        private void AnimateLevelUpSequence(float targetProgress) {
            var sequence = DOTween.Sequence();

            // 현재 진행도 -> 1.0 (경험치바 채우기) - 빠르게
            float fillDuration = (1.0f - _lastProgress) * 0.3f; // 고정 시간으로 빠르게
            sequence.Append(_expBarFill.DOFillAmount(1.0f, fillDuration).SetEase(Ease.OutQuart));

            // 1.0 -> 0 (레벨업 후 초기화) - 즉시
            sequence.Append(_expBarFill.DOFillAmount(0f, 0.05f).SetEase(Ease.InQuad));

            // 0 -> 목표 진행도 (새 레벨 경험치) - 빠르게
            float targetDuration = targetProgress * 0.3f; // 고정 시간으로 빠르게
            sequence.Append(_expBarFill.DOFillAmount(targetProgress, targetDuration).SetEase(Ease.OutQuart));

            // 완료 시 마지막 진행도 업데이트
            sequence.OnComplete(() => _lastProgress = targetProgress);
        }

        /// <summary>
        /// 일반 진행도 애니메이션
        /// </summary>
        private void AnimateNormalProgress(float targetProgress) {
            float duration = Mathf.Abs(targetProgress - _lastProgress) * DURATION_SCALE;
            Ease easeType = targetProgress > _lastProgress ? Ease.OutQuart : Ease.InOutQuad;

            _expBarFill.DOFillAmount(targetProgress, duration)
                .SetEase(easeType)
                .OnComplete(() => _lastProgress = targetProgress);
        }



#if UNITY_EDITOR
        [ContextMenu("Gain Exp 50")]
        private void TestGainExp() {
            GameDebug.Log("Gain Exp 50");
            EventBus.Publish(new ExpRewardEvent(50));
        }

        [ContextMenu("Gain Exp 200")]
        private void TestGainExpLarge() {
            GameDebug.Log("Gain Exp 200");
            EventBus.Publish(new ExpRewardEvent(200));
        }
#endif
    }
}