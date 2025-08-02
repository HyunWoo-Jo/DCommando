using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Game.ViewModels;
using R3;
using System;

namespace Game.UI
{
    public class HealthUIView : MonoBehaviour
    {
        [Inject] private HealthUIViewModel _viewModel;

        // UnityReference
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Text healthText;
        [SerializeField] private Image healthFillImage;
        [SerializeField] private GameObject lowHealthWarning;
        
        // 상수
        private const string HEALTH_TEXT_FORMAT = "{0}/{1}";
        
        // 속성
        public bool IsVisible { get; private set; }
        
        // Disposables
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake() 
        {
#if UNITY_EDITOR
            RefAssert();
#endif
            Bind();
        }
        
        private void Start() 
        {
            _viewModel.Notify();
        }
        
        private void OnDestroy()
        {
            _disposables?.Dispose();
        }

#if UNITY_EDITOR
        // 검증
        private void RefAssert() 
        {
            Debug.Assert(healthSlider != null, "healthSlider is null");
            Debug.Assert(healthText != null, "healthText is null");
            Debug.Assert(healthFillImage != null, "healthFillImage is null");
            Debug.Assert(lowHealthWarning != null, "lowHealthWarning is null");
        }
#endif
        
        private void Bind()
        {
            // 현재 체력 바인딩
            _viewModel.CurrentHealth
                .Subscribe(OnCurrentHealthChanged)
                .AddTo(_disposables);
                
            // 최대 체력 바인딩
            _viewModel.MaxHealth
                .Subscribe(OnMaxHealthChanged)
                .AddTo(_disposables);
                
            // 체력 비율 바인딩
            _viewModel.HealthRatio
                .Subscribe(OnHealthRatioChanged)
                .AddTo(_disposables);
                
            // 저체력 상태 바인딩
            _viewModel.IsLowHealth
                .Subscribe(OnLowHealthChanged)
                .AddTo(_disposables);
        }
        
        // UI 갱신
        private void UpdateUI() 
        {
            if (_viewModel == null) return;
            
            UpdateHealthSlider();
            UpdateHealthText();
            UpdateHealthColor();
            UpdateLowHealthWarning();
        }
        
        // 이벤트 핸들러
        private void OnCurrentHealthChanged(int currentHealth)
        {
            UpdateHealthSlider();
            UpdateHealthText();
        }
        
        private void OnMaxHealthChanged(int maxHealth)
        {
            UpdateHealthSlider();
            UpdateHealthText();
        }
        
        private void OnHealthRatioChanged(float ratio)
        {
            UpdateHealthSlider();
            UpdateHealthColor();
        }
        
        private void OnLowHealthChanged(bool isLowHealth)
        {
            UpdateLowHealthWarning();
        }
        
        // UI 업데이트 메서드들
        private void UpdateHealthSlider()
        {
            if (healthSlider != null)
            {
                healthSlider.value = _viewModel.HealthRatio.CurrentValue;
            }
        }
        
        private void UpdateHealthText()
        {
            if (healthText != null)
            {
                healthText.text = string.Format(HEALTH_TEXT_FORMAT, 
                    _viewModel.CurrentHealth.CurrentValue, 
                    _viewModel.MaxHealth.CurrentValue);
            }
        }
        
        private void UpdateHealthColor()
        {
            if (healthFillImage != null)
            {
                float ratio = _viewModel.HealthRatio.CurrentValue;
                
                if (ratio > 0.6f)
                    healthFillImage.color = Color.green;
                else if (ratio > 0.3f)
                    healthFillImage.color = Color.yellow;
                else
                    healthFillImage.color = Color.red;
            }
        }
        
        private void UpdateLowHealthWarning()
        {
            if (lowHealthWarning != null)
            {
                lowHealthWarning.SetActive(_viewModel.IsLowHealth.CurrentValue);
            }
        }
        
        // 공개 메서드
        public void SetVisible(bool visible)
        {
            IsVisible = visible;
            gameObject.SetActive(visible);
        }
////////////////////////////////////////////////////////////////////////////////////
        // your logic here
    }
}