using Cysharp.Threading.Tasks;
using Game.Core;
using System.Threading;
using UnityEngine;

namespace Game.Services
{
    public interface ISceneService {

        /// <summary>
        /// 씬을 로드합니다

        UniTask LoadSceneAsync(SceneName sceneName, float delay);

        /// <summary>
        /// 로딩 씬을 거쳐 씬을 로드합니다
        /// </summary>
        UniTask LoadSceneWithLoadingAsync(SceneName targetScene, float delay, float minLoadingDelay);

        /// <summary>
        /// 현재 씬 이름을 가져옵니다
        /// </summary>
        SceneName GetCurrentSceneName();

        /// <summary>
        /// 씬 로딩 진행률을 가져옵니다
        /// </summary>
        float GetLoadingProgress();


    }
}
