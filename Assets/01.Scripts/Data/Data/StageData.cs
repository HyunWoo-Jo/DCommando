using UnityEngine;
using Game.Core;
namespace Game.Data
{
    [System.Serializable]
    public class StageData
    {
        public int stageId;
        public string stageName;
        public string description;

        // Enemy Wave 데이터
        public EnemyData[] enemyDatas;
        
        // 보상 데이터
        public int goldReward;
        public int expReward;
        
        // 스테이지 설정
        public bool autoStartNextStage = true;
        public float timeLimit = 0f; // 0이면 제한 없음
    }
    
    [System.Serializable]
    public class EnemyData
    {
        public EnemyName enemyName; // Enemy Name
        public Vector2 spawnPosition;
        public int enemyHealth;
        public int power;
        public int expReward;
        public int goldReward;
    }
}