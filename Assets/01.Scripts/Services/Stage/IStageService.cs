using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using UnityEngine;

namespace Game.Services
{
    public interface IStageService
    {
        UniTask<GameObject> GetEnemyObjectAsync(EnemyName enemyName);
        UniTask<SO_StageConfig> GetStageConfigAsync(StageName stageName);
        StageData[] GetAllStageData();
    }
}