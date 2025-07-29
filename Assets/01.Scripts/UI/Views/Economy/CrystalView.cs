using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using R3;
using Game.ViewModels;
using Cysharp.Threading.Tasks;

namespace Game.UI
{
    public class CrystalView : MonoBehaviour {
        [Inject] private CrystalViewModel _viewModel;

        [Header("UI 컴포넌트")]
        [SerializeField] private TextMeshProUGUI _totalCrystalText;
        [SerializeField] private TextMeshProUGUI _freeCrystalText;
        [SerializeField] private TextMeshProUGUI _paidCrystalText;
        [SerializeField] private Button _gainTestButton;
        [SerializeField] private Button _spendTestButton;

        private void Start() {
            Bind();
        }

        private void Bind() {
            // Total
            _viewModel.RORP_TotalCrystalDisplayText
                .Subscribe(UpdateTotalCrystalText)
                .AddTo(this);
            
            _viewModel.RORP_TotalCrystalColor
                .Subscribe(UpdateTotalCrystalColor)
                .AddTo(this);
            // Free
            _viewModel.RORP_FreeCrystalDisplayText
                .Subscribe(UpdateFreeCrystalText)
                .AddTo(this);
            _viewModel.RORP_TotalCrystalColor
                .Subscribe(UpdateFreeCrystalColor)
                .AddTo(this);
            // Paid
            _viewModel.RORP_PaidCrystalDisplayText
                .Subscribe(UpdatePaidCrystalText)
                .AddTo(this);
            _viewModel.RORP_PaidCrystalColor
                .Subscribe(UpdatePaidCrystalColor)
                .AddTo(this);
        }

        // UI 업데이트
        private void UpdateTotalCrystalText(string crystalStr) {
            if (_totalCrystalText != null)
                _totalCrystalText.text = crystalStr;
        }

        private void UpdateFreeCrystalText(string crystalStr) {
            if (_freeCrystalText != null)
                _freeCrystalText.text = crystalStr;
        }

        private void UpdatePaidCrystalText(string crystalStr) {
            if (_paidCrystalText != null)
                _paidCrystalText.text = crystalStr;
        }

        public void UpdateTotalCrystalColor(Color color) {
            if(_totalCrystalText != null) {
                _totalCrystalText.color = color;
            }
        }
        public void UpdateFreeCrystalColor(Color color) {
            if (_freeCrystalText != null) {
                _freeCrystalText.color = color;
            }
        }
        public void UpdatePaidCrystalColor(Color color) {
            if (_paidCrystalText != null) {
                _paidCrystalText.color = color;
            }
        }

    }
}