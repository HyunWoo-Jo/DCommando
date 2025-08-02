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

        // ReactiveProperty: R
        private readonly ReactiveProperty<string> RP_healthText = new();
        private readonly ReactiveProperty<float> RP_healthRatio = new();
        private readonly ReactiveProperty<bool> RP_isLowHealth = new();

        // 읽기 전용 프로퍼티
        public ReadOnlyReactiveProperty<string> HealthText => RP_healthText;
        public ReadOnlyReactiveProperty<float> HealthRatio => RP_healthRatio;
        public ReadOnlyReactiveProperty<bool> IsLowHealth => RP_isLowHealth;

        // Model의 데이터를 직접 노출
        public ReadOnlyReactiveProperty<int> CurrentHp => _healthModel?.RORP_CurrentHp;
        public ReadOnlyReactiveProperty<int> MaxHp => _healthModel?.RORP_MaxHp;
        public ReadOnlyReactiveProperty<bool> IsDead => _healthModel?.RORP_IsDead;


        private CompositeDisposable _compositeDisposable = new();

        // 초기화 HealthView에서 호출
        public void Initialize(object healthModel) {
            _healthModel = healthModel as HealthModel;

            if (_healthModel == null) {
                GameDebug.LogError("healthModel에 잘못된 Model을 넣었습니다.");
            }

            // 데이터 변화 구독
            SubscribeToHealthChanges();

            // 초기 UI 업데이트
            UpdateUI();
        }
        // 해제 HealthView에서 호출
        public void Dispose() {
            _compositeDisposable?.Dispose();
        }

        // 체력 변화 구독
        private void SubscribeToHealthChanges() {
            // 현재 체력 변화 구독
            _healthModel.RORP_CurrentHp
                .Subscribe(_ => UpdateUI())
                .AddTo(_compositeDisposable);

            // 최대 체력 변화 구독
            _healthModel.RORP_MaxHp.
                Subscribe(_ => UpdateUI())
                .AddTo(_compositeDisposable);

            // 사망 상태 변화 구독
            _healthModel.RORP_IsDead
                .Subscribe(_ => UpdateUI())
                .AddTo(_compositeDisposable);
        }

        // Model 메서드 위임
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

        public float GetHpRatio() {
            return _healthModel?.GetHpRatio() ?? 0f;
        }

        // UI 업데이트
        private void UpdateUI() {
            if (_healthModel == null) return;

            var current = _healthModel.RORP_CurrentHp.CurrentValue;
            var max = _healthModel.RORP_MaxHp.CurrentValue;
            var ratio = _healthModel.GetHpRatio();

            // 체력 텍스트 업데이트
            RP_healthText.Value = $"{current}/{max}";

            // 체력 비율 업데이트
            RP_healthRatio.Value = ratio;

            // 저체력 상태 업데이트
            RP_isLowHealth.Value = ratio <= LOW_HEALTH_THRESHOLD && current > 0 && !_healthModel.RORP_IsDead.CurrentValue;
        }

        // 데이터 변경 알림
        public void Notify() {
            UpdateUI();
        }

       
    }
}