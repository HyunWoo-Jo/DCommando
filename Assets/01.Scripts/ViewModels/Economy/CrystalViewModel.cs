using Zenject;
using R3;
using Game.Systems;
using Game.Core.Event;
using Game.Core.Styles;
using Game.Models;
using UnityEngine;
using System;
namespace Game.ViewModels
{
    public class CrystalViewModel : IInitializable, IDisposable {
        [Inject] private CrystalModel _crystalModel;
        [Inject] private CrystalSystem _crystalSystem;
        [Inject] private SO_CrystalStyle _crystalStyle;
        
        private CompositeDisposable _disposables = new();
        
        // Model 
        public ReadOnlyReactiveProperty<int> RORP_TotalCrystal => _crystalModel.RORP_CurrentCrystal; 
        public ReadOnlyReactiveProperty<int> RORP_FreeCrystal => _crystalModel.RORP_FreeCrystal;
        public ReadOnlyReactiveProperty<int> RORP_PaidCrystal => _crystalModel.RORP_PaidCrystal;

        // 내부 변환 데이터
        public ReadOnlyReactiveProperty<string> RORP_TotalCrystalDisplayText { get; private set; }
        public ReadOnlyReactiveProperty<string> RORP_FreeCrystalDisplayText { get; private set; }
        public ReadOnlyReactiveProperty<string> RORP_PaidCrystalDisplayText { get; private set; }

        public ReadOnlyReactiveProperty<Color> RORP_TotalCrystalColor { get; private set; }
        public ReadOnlyReactiveProperty<Color> RORP_FreeCrystalColor { get; private set; }
        public ReadOnlyReactiveProperty<Color> RORP_PaidCrystalColor { get; private set; }

        
        public readonly Subject<string> OnCrystalNotification = new();

        /// <summary>
        /// Zenject에서 관리
        /// </summary>
        public void Initialize() {
            // Economy Style을 사용
            RORP_TotalCrystalDisplayText = RORP_TotalCrystal
                .Select(FormatCrystalChange)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            RORP_TotalCrystalColor = RORP_TotalCrystal
                .Select(GetColorForTotalAmount)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);


            // 무료 크리스탈
            RORP_FreeCrystalDisplayText = RORP_FreeCrystal
                .Select(FormatCrystalChange)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            RORP_FreeCrystalColor = RORP_FreeCrystal
                .Select(GetColorForFreeAmount)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);

            // 유료 크리스탈
            RORP_PaidCrystalDisplayText = RORP_PaidCrystal
                .Select(FormatCrystalChange)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
            RORP_PaidCrystalColor = RORP_PaidCrystal
                .Select(GetColorForPaidAmount)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);
        }
        /// <summary>
        /// Zenject에서 관리
        /// </summary>
        public void Dispose() {
            RORP_TotalCrystalDisplayText?.Dispose();
            RORP_FreeCrystalDisplayText?.Dispose();
            RORP_PaidCrystalDisplayText?.Dispose();
            RORP_TotalCrystalColor?.Dispose();
            RORP_FreeCrystalColor?.Dispose();
            RORP_PaidCrystalColor?.Dispose();
            _disposables?.Dispose();
            _disposables = null;
        }

        /// <summary>
        /// 변화량을 스타일로 포맷팅
        /// </summary>
        public string FormatCrystalChange(int changeAmount) => _crystalStyle.FormatCrystalAmount(changeAmount);

        /// <summary>
        /// 값을 컬러로 가지고옴
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Color GetColorForTotalAmount(int amount) => _crystalStyle.GetColorForTotalAmount(amount);

        public Color GetColorForFreeAmount(int amount) => _crystalStyle.GetColorForFreeAmount(amount);

        public Color GetColorForPaidAmount(int amount) => _crystalStyle.GetColorForPaidAmount(amount);


        public bool CanSpend(int amount) {
            return _crystalSystem.CanSpend(amount);
        }

        
    }
}