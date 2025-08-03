using R3;
using System;
using Zenject;
using Game.Models;
using System.Diagnostics;
using Game.Core;

namespace Game.ViewModels {
    public class HealthViewModel {
        public class Factory : PlaceholderFactory<HealthViewModel> { }

        private HealthModel _healthModel; // 외부에서 주입
        // 상수
        private const float LOW_HEALTH_THRESHOLD = 0.3f;

        // Model 데이터 직접 노출
        public ReadOnlyReactiveProperty<int> RORP_CurrentHp => _healthModel?.RORP_CurrentHp;
        public ReadOnlyReactiveProperty<int> RORP_MaxHp => _healthModel?.RORP_MaxHp;
        public ReadOnlyReactiveProperty<bool> RORP_IsDead => _healthModel?.RORP_IsDead;
        public bool IsDead => _healthModel?.RORP_IsDead.CurrentValue ?? false;
        public float HpRatio => _healthModel?.GetHpRatio() ?? 0f;

        // 읽기 전용 프로퍼티
        public ReadOnlyReactiveProperty<string> RORP_HealthText { get; private set; }
        public ReadOnlyReactiveProperty<float> RORP_HealthRatio { get; private set; }
        public ReadOnlyReactiveProperty<bool> RORP_IsLowHealth { get; private set; }


        private CompositeDisposable _disposable = new();

        // 초기화 HealthView에서 호출
        public void Initialize(object healthModel) {
            _healthModel = healthModel as HealthModel;

            if (_healthModel == null) {
                GameDebug.LogError("healthModel에 잘못된 Model을 넣었습니다.");
            }

            // 체력 텍스트 변경
            RORP_HealthText = Observable.CombineLatest(
                    _healthModel.RORP_CurrentHp,
                    _healthModel.RORP_MaxHp,
                    (current, max) => FormatHealthText(current, max))
                .ThrottleLastFrame(1)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);

            // 체력 비율 변경
            RORP_HealthRatio = Observable.CombineLatest(
                    _healthModel.RORP_CurrentHp,
                    _healthModel.RORP_MaxHp,
                    (current, max) => HpRatio)
                .ThrottleLastFrame(1)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);

            // 저체력 상태 변경
            RORP_IsLowHealth = Observable.CombineLatest(
                    _healthModel.RORP_CurrentHp,
                    _healthModel.RORP_MaxHp,
                    _healthModel.RORP_IsDead,
                    (current, max, isDead) => CalculateIsLowHealth(current, max, isDead))
                .ThrottleLastFrame(1)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);
        }
        // 해제 HealthView에서 호출
        public void Dispose() {
            _disposable?.Dispose();
        }



        /// <summary>
        /// Model 메서드 위임
        /// </summary>
        public void TakeDamage(int damage) {
            _healthModel?.TakeDamage(damage);
        }

        public void Heal(int healAmount) {
            _healthModel?.Heal(healAmount);
        }

        public void SetMaxHp(int maxHp) {
            _healthModel?.SetMaxHp(maxHp);
        }

        public void SetCurrentHp(int hp) {
            _healthModel?.SetCurrentHp(hp);
        }

        public void Revive(int reviveHp = -1) {
            _healthModel?.Revive(reviveHp);
        }

        /// <summary>
        /// 체력 텍스트 포맷팅
        /// </summary>
        private string FormatHealthText(int current, int max) {
            return $"{current}/{max}";
        }

        /// <summary>
        /// 저체력 상태 계산
        /// </summary>
        private bool CalculateIsLowHealth(int current, int max, bool isDead) {
            if (isDead || current <= 0 || max <= 0) return false;

            float ratio = (float)current / max;
            return ratio <= LOW_HEALTH_THRESHOLD;
        }


    }
}