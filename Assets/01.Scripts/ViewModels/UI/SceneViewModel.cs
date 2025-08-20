using UnityEngine;
using Zenject;
using Game.Systems;
using Game.Core;
namespace Game.ViewModels
{
    /// <summary>
    /// SceneService와 연결 해주는 ViewModel
    /// </summary>
    public class SceneViewModel
    {
        [Inject] private SceneSystem _sceneSystem;
        public float GetLoadingProgress() {
            return _sceneSystem.GetLoadingProgress();
        }

        public void LoadSceneWithLoading(SceneName targetScene, float delay ) {
            _sceneSystem.LoadSceneWithLoading(targetScene, delay);
        }
    }
}
