using UnityEngine;
using Game.Data;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "StageConfig", menuName = "Game/Config/StageConfig")]
    public class SO_StageConfig : ScriptableObject
    {
        [Header("Stage 설정")]
        public StageData[] stages;

        [Header("Stage 크기")]
        public Rect stageSize = new Rect(-6, -10, 6, 10);
    }
}