using R3;
using System;
using Zenject;
using Game.Core;
using Game.Models;
using Game.Core.Event;

namespace Game.ViewModels {
    public class ExpViewModel : IInitializable, IDisposable {

        [Inject] private ExpModel _expModel;


        // model 데이터
        public ReadOnlyReactiveProperty<int> RORP_CurrentLevel => _expModel?.RORP_CurrentLevel;
        public bool IsMaxLevel => _expModel?.IsMaxLevel() ?? false;
        public float Progress => _expModel?.GetExpProgress() ?? 0f;

        // 읽기 전용 프로퍼티
        public ReadOnlyReactiveProperty<string> RORP_LevelText { get; private set; }
        public ReadOnlyReactiveProperty<float> RORP_ExpProgress { get; private set; }

        // 해제용

        private CompositeDisposable _disposable = new();

        /// <summary>
        /// 초기화 ExpView에서 호출 zenject에서 관리
        /// </summary>
        public void Initialize() {
            // 레벨 변경
            RORP_LevelText = _expModel.RORP_CurrentLevel
                .ThrottleLastFrame(1)
                .Select(FormatLevelText)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);

            // Exp 변경
            RORP_ExpProgress = _expModel.RORP_CurrentExp
                .ThrottleLastFrame(1)
                .Select(_ => Progress)
                .ToReadOnlyReactiveProperty()
                .AddTo(_disposable);
        }

        /// <summary>
        /// 해제 zenject에서 관리
        /// </summary>
        public void Dispose() {
            _disposable?.Dispose();
        }



        /// <summary>
        /// 다음 레벨까지 필요한 경험치 조회
        /// </summary>
        public int GetExpToNextLevel() {
            if (_expModel == null || _expModel.IsMaxLevel()) return 0;

            var currentExp = _expModel.RORP_CurrentExp.CurrentValue;
            var maxExp = _expModel.RORP_MaxExp.CurrentValue;

            return maxExp - currentExp;
        }

        private string FormatLevelText(int level) {
            return $"Level {level}";
        }
    }
}