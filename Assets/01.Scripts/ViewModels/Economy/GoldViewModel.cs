using Zenject;
using System;
using Game.Models;
using Game.Systems;
using R3;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Core.Styles;

namespace Game.ViewModels
{
    public class GoldViewModel : IInitializable, IDisposable {
        [Inject] private GoldModel _goldModel;
        [Inject] private GoldSystem _goldSystem;
        [Inject] private SO_GoldStyle _goldStyle;

        private readonly CompositeDisposable _disposables = new();

        // UI 바인딩용 프로퍼티
        public ReadOnlyReactiveProperty<int> RORP_CurrentGold => _goldModel.RORP_CurrentGold;
        public ReadOnlyReactiveProperty<string> RORP_GoldText { get; private set; }
        public ReadOnlyReactiveProperty<Color> RORP_GoldColor { get; private set; }


        #region Zenject 관리
        public void Initialize() {
            RORP_GoldText = _goldModel.RORP_CurrentGold
                .ThrottleLastFrame(1)
                .Select(FormatGoldAmount)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);


            RORP_GoldColor = _goldModel.RORP_CurrentGold
                .ThrottleLastFrame(1)
                .Select(GetColorForAmount)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposables);

        }
        public void Dispose() {
            RORP_GoldText?.Dispose();
            RORP_GoldColor?.Dispose();
            _disposables?.Dispose();
        }
        #endregion
        public Color GetColorForAmount(int amount) => _goldStyle.GetColorForAmount(amount);

        public string FormatGoldAmount(int amount) => _goldStyle.FormatGoldAmount(amount);


        public bool AddGold(int amount) {
            return _goldSystem.AddGold(amount);
        }

        public bool SpendGold(int amount) {
            var success = _goldSystem.SpendGold(amount);
            return success;
        }

        public bool CanSpend(int amount) {
            return _goldSystem.CanSpend(amount);
        }

      
    }
}