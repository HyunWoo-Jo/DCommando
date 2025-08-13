using UnityEngine;
using Game.Data;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "StageConfig", menuName = "Game/Config/StageConfig")]
    public class SO_StageConfig : ScriptableObject
    {
        [Header("Stage 설정")]
        public StageData[] stages;
        
        [Header("보상 배수")]
        public float hardModeDifficultyMultiplier = 1.5f;
        public float hardModeRewardMultiplier = 2.0f;
    }
}