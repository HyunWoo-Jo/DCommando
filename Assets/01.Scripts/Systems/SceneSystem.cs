using Zenject;
using Game.Core;
using Game.Services;
using Cysharp.Threading.Tasks;
using Game.Core.Event;
using System;

namespace Game.Systems {
    /// <summary>
    /// 씬 시스템 - 간단한 씬 전환 로직
    /// </summary>
    public class SceneSystem  {
        [Inject] private ISceneService _sceneService;

        private const float MINIMUM_LOADING_TIME = 2f;

        private SceneName _curSceneName = SceneName.None;
        private SceneName _preSceneName = SceneName.None;

        private bool _isLoading = false;


        #region 초기화
        [Inject]
        public void Initialize() {
            _curSceneName = GetCurrentScene();
        }
        #endregion
        /// <summary>
        /// 씬을 로드
        /// </summary>
        public async void LoadScene(SceneName targetScene, float delay = 0f) {
            if (_isLoading) {
                GameDebug.LogError( $"{targetScene.ToString()}:요청 무시 / 이미 씬 로딩중");
                return;
            }
            _isLoading = true;
            EventBus.Publish(new SceneLoadingEvent(_curSceneName, targetScene, delay));
            await _sceneService.LoadSceneAsync(targetScene, delay);
            _isLoading = false;
            _preSceneName = _curSceneName;
            _curSceneName = targetScene;
        }

        /// <summary>
        /// 로딩 씬을 거쳐 씬을 로드 + Effect 까지 발생
        /// </summary>
        public async void LoadSceneWithLoading(SceneName targetScene, float delay = 0f) {
            if (_isLoading) {
                GameDebug.LogError($"{targetScene.ToString()}:요청 무시 / 이미 씬 로딩중");
                return;
            }
            _isLoading = true;
            EventBus.Publish(new SceneLoadingEvent(_curSceneName, targetScene, delay));
            await _sceneService.LoadSceneWithLoadingAsync(targetScene, delay, MINIMUM_LOADING_TIME);
            _isLoading = false;
            _preSceneName = _curSceneName;
            _curSceneName = targetScene;
        }

        /// <summary>
        /// 현재 씬 이름 가져오기
        /// </summary>
        public SceneName GetCurrentScene() {
            return _sceneService.GetCurrentSceneName();
        }

        /// <summary>
        /// 로딩 진행률 가져오기
        /// </summary>
        public float GetLoadingProgress() {
            return _sceneService.GetLoadingProgress();
        }

     
    }
}