using System.Collections.Generic;
using UnityEngine;
using Game.Data;
using Game.Core;

namespace Game.Services {
    public interface IUpgradeService {
        /// <summary>
        /// 모든 업그레이드 데이터 조회
        /// </summary>
        List<UpgradeData> GetUpgradeData();

        /// <summary>
        /// 업그레이드 타입별 스프라이트 조회
        /// </summary>
        Sprite GetSprite(int upgradeId);

        /// <summary>
        /// 특정 타입의 스프라이트 언로드
        /// </summary>
        void UnloadSprite(int upgradeId);

        /// <summary>
        /// 특정 이름의 업그레이드 데이터 조회
        /// </summary>
        UpgradeData GetUpgradeDataByName(string upgradeName);

        /// <summary>
        /// 특정 타입의 업그레이드 데이터 리스트 조회
        /// </summary>
        List<UpgradeData> GetUpgradeDataByType(UpgradeType upgradeType);
    }
}