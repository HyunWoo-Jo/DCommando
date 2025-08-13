namespace Game.Core {
    // Stage 시작 요청 이벤트
    public readonly struct StartStageEvent {
        public readonly StageName stageNage;

        public StartStageEvent(StageName stageNage) {
            this.stageNage = stageNage;
        }
    }

    // Stage 시작 완료 이벤트
    public readonly struct StageStartedEvent {
        public readonly int stageId;
        public readonly object stageData; // StageData를 object로 변경 (순환 참조 방지)

        public StageStartedEvent(int stageId, object stageData) {
            this.stageId = stageId;
            this.stageData = stageData;
        }
    }

    // Stage 종료 이벤트
    public readonly struct StageEndedEvent {
        public readonly int stageId;
        public readonly object stageData; // StageData를 object로 변경 (순환 참조 방지)

        public StageEndedEvent(int stageId, object stageData) {
            this.stageId = stageId;
            this.stageData = stageData;
        }
    }

    // Enemy 처치 이벤트
    public readonly struct EnemyDefeatedEvent {
        public readonly int enemyId;
        public readonly int expReward;
        public readonly int goldReward;

        public EnemyDefeatedEvent(int enemyId, int expReward, int goldReward) {
            this.enemyId = enemyId;
            this.expReward = expReward;
            this.goldReward = goldReward;
        }
    }

    // Stage 언락 이벤트
    public readonly struct StageUnlockedEvent {
        public readonly int stageId;

        public StageUnlockedEvent(int stageId) {
            this.stageId = stageId;
        }
    }



}