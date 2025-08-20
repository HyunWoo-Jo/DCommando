namespace Game.Core.Event {
    /// <summary>
    /// 업그레이드 리롤 요청 이벤트
    /// </summary>
    public readonly struct UpgradeRerollEvent {
        public readonly int selectedIndex;

        public UpgradeRerollEvent(int selectedIndex) {
            this.selectedIndex = selectedIndex;
        }
    }

    /// <summary>
    /// 업그레이드 적용 완료 이벤트
    /// </summary>
    public readonly struct UpgradeAppliedEvent {
        public readonly string upgradeName;
        public readonly UpgradeType[] upgradeTypes;
        public readonly float[] values;

        public UpgradeAppliedEvent(string upgradeName, UpgradeType[] upgradeTypes, float[] values) {
            this.upgradeName = upgradeName;
            this.upgradeTypes = upgradeTypes;
            this.values = values;
        }
    }

    public readonly struct StatChangeEvent {
        public readonly UpgradeType upgradeType;
        public readonly float value;

        public StatChangeEvent(UpgradeType upgradeType, float value) {
            this.upgradeType = upgradeType;
            this.value = value;
        }
    }


}