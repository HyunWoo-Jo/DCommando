using Game.Models;
using Game.Core.Event;
using Zenject;
using R3;

namespace Game.Systems {
    /// <summary>
    /// 플레이어 경험치 및 레벨 관리 시스템
    /// 경험치 획득, 레벨업, 최대 레벨 처리를 담당
    /// </summary>
    public class ExpSystem {
        private readonly ExpModel _expModel;
        private readonly SO_ExpConfig _config;
        private CompositeDisposable _disposables = new();

        /// <summary>
        /// ExpSystem 생성자
        /// </summary>
        public ExpSystem(ExpModel expModel, SO_ExpConfig config) {
            _expModel = expModel;
            _config = config;

            // 최대 레벨 제한 설정
            _expModel.SetMaxLevel(_config.maxLevel);
            Initialize();

            // 외부 경험치 획득 이벤트 구독
            SubscribeToEvents();
        }

        /// <summary>
        /// 외부 이벤트 구독 설정
        /// </summary>
        private void SubscribeToEvents() {
            // 몬스터 처치, 퀘스트 완료 등에서 발생하는 경험치 획득 이벤트 구독
            EventBus.Subscribe<ExpRewardEvent>(OnExpReward).AddTo(_disposables);
        }

        /// <summary>
        /// 외부에서 발생한 경험치 보상 처리
        /// </summary>
        private void OnExpReward(ExpRewardEvent evt) {
            GainExp(evt.amount);
        }

        /// <summary>
        /// 초기 경험치 및 레벨 설정
        /// 게임 시작 시 또는 데이터 초기화 시 호출
        /// </summary>
        public void Initialize() {
            var startingLevel = _config.startingLevel;
            var startingExp = _config.startingExp;

            // 시작 레벨과 경험치 적용
            _expModel.SetLevel(startingLevel);
            _expModel.SetExp(startingExp);

            // 시작 레벨에 맞는 최대 경험치 설정
            UpdateMaxExpForCurrentLevel();
        }

        /// <summary>
        /// 경험치 획득 처리
        /// 자동으로 레벨업 체크 및 이벤트 발행 수행
        /// </summary>
        public bool GainExp(int amount) {
            // 유효성 검사
            if (amount <= 0) return false;
            if (_expModel.IsMaxLevel()) return false;

            var currentExp = _expModel.RORP_CurrentExp.CurrentValue;
            var newExp = currentExp + amount;

            // 새 경험치 적용
            _expModel.SetExp(newExp);

            // 레벨업 조건 체크 및 처리
            CheckLevelUp();

            // 경험치 획득 및 변경 이벤트 발행
            EventBus.Publish(new ExpGainedEvent(amount, newExp));
            EventBus.Publish(new ExpChangedEvent(newExp, _expModel.RORP_MaxExp.CurrentValue, _expModel.GetExpProgress()));

            return true;
        }

        /// <summary>
        /// 연속 레벨업 체크
        /// 한번에 여러 레벨이 올라갈 수 있는 상황 처리
        /// </summary>
        private void CheckLevelUp() {
            while (_expModel.CanLevelUp()) {
                LevelUp();
            }
        }

        /// <summary>
        /// 단일 레벨업 처리
        /// 초과 경험치 계산, 새 레벨 설정, 이벤트 발행
        /// </summary>
        private void LevelUp() {
            var currentLevel = _expModel.RORP_CurrentLevel.CurrentValue;
            var currentExp = _expModel.RORP_CurrentExp.CurrentValue;
            var maxExp = _expModel.RORP_MaxExp.CurrentValue;

            // 현재 레벨을 넘어선 초과 경험치 계산
            var remainingExp = currentExp - maxExp;
            var newLevel = currentLevel + 1;

            // 레벨업 적용
            _expModel.SetLevel(newLevel);
            _expModel.SetExp(remainingExp);

            // 새 레벨의 최대 경험치 적용
            UpdateMaxExpForCurrentLevel();

            // 레벨업 이벤트 발행
            EventBus.Publish(new LevelUpEvent(newLevel, currentLevel));

            // 최대 레벨 도달 체크 및 처리
            if (_expModel.IsMaxLevel()) {
                _expModel.SetExp(0); // 최대 레벨에서는 경험치 0으로 고정
                EventBus.Publish(new MaxLevelReachedEvent(newLevel));
            }
        }

        /// <summary>
        /// 현재 레벨에 맞는 최대 경험치 갱신
        /// 레벨업 시 또는 레벨 변경 시 호출
        /// </summary>
        private void UpdateMaxExpForCurrentLevel() {
            var currentLevel = _expModel.RORP_CurrentLevel.CurrentValue;
            var maxExpForLevel = CalculateMaxExpForLevel(currentLevel);
            _expModel.SetMaxExp(maxExpForLevel);
        }

        /// <summary>
        /// 특정 레벨의 최대 경험치 계산
        /// 공식: 기본 경험치 + (레벨 * 증가량)
        /// </summary>
        private int CalculateMaxExpForLevel(int level) {
            return _config.baseExpPerLevel + (level * _config.expIncreasePerLevel);
        }

        /// <summary>
        /// 특정 레벨로 직접 설정
        /// 치트, 디버그, 또는 특별한 게임 로직용
        /// </summary>
        public bool SetLevel(int targetLevel) {
            // 유효 범위 체크
            if (targetLevel < 1 || targetLevel > _config.maxLevel) return false;

            var previousLevel = _expModel.RORP_CurrentLevel.CurrentValue;

            // 레벨 직접 설정
            _expModel.SetLevel(targetLevel);
            _expModel.SetExp(0); // 경험치는 0으로 초기화
            UpdateMaxExpForCurrentLevel();

            // 레벨 변경 이벤트 발행
            EventBus.Publish(new LevelUpEvent(targetLevel, previousLevel));
            return true;
        }

        /// <summary>
        /// 다음 레벨까지 필요한 경험치 계산
        /// UI 표시나 진행률 계산에 사용
        /// </summary>
        public int GetExpToNextLevel() {
            if (_expModel.IsMaxLevel()) return 0;

            var currentExp = _expModel.RORP_CurrentExp.CurrentValue;
            var maxExp = _expModel.RORP_MaxExp.CurrentValue;

            return maxExp - currentExp;
        }

        /// <summary>
        /// 현재 경험치 정보 조회
        /// 디버그나 UI 표시용
        /// </summary>
        public int GetCurrentExp() {
            return _expModel.RORP_CurrentExp.CurrentValue;
        }

        /// <summary>
        /// 현재 레벨 정보 조회
        /// </summary>
        public int GetCurrentLevel() {
            return _expModel.RORP_CurrentLevel.CurrentValue;
        }

        /// <summary>
        /// 경험치 진행률 조회
        /// 프로그레스바 표시용
        /// </summary>
        public float GetExpProgress() {
            return _expModel.GetExpProgress();
        }

        /// <summary>
        /// 현재 레벨의 최대 경험치 조회
        /// </summary>
        public int GetMaxExp() {
            return _expModel.RORP_MaxExp.CurrentValue;
        }

        /// <summary>
        /// 최대 레벨인지 확인
        /// </summary>
        public bool IsMaxLevel() {
            return _expModel.IsMaxLevel();
        }

        /// <summary>
        /// 레벨업 가능한지 확인
        /// </summary>
        public bool CanLevelUp() {
            return _expModel.CanLevelUp();
        }

        /// <summary>
        /// 시스템 정리 및 구독 해제
        /// </summary>
        public void Dispose() {
            _disposables?.Dispose();
        }
    }
}