using UnityEngine;
using System;
using Game.Core;
namespace Game.Data {
    public class UpgradeData {
        // 업그레이드
        public string upgradeName; // 업그레이드 이름
        public int upgradeCount;// 업그레이드 개수
        public UpgradeType[] upgradeTypes; // 업그레이드 종류
        public float[] values; // 증가 수치

        // 해금
        public UpgradeConditionType[] upgradeConditionTypes; // 해금 조건
        public ConditionOperator[] conditionOperators; // 조건 연산자
        public float[] conditionValues; // 해금 조건 수치

        public string spriteAddressableKey; // 이미지 키
    }
}
