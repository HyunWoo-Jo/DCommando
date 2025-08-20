using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Game.Core;
using Game.Data;

namespace Game.Services {
    /// <summary>
    /// 씬 서비스 구현체
    /// </summary>
    public class SceneService : ISceneService {

        private AsyncOperation _currentLoadOperation;

        public async UniTask LoadSceneWithLoadingAsync(SceneName targetScene, float delay, float minLoadingDelay) {
            await LoadSceneAsync(SceneName.LoadingScene, delay); // Loading Scene으로 이동
            await LoadSceneAsync(targetScene, minLoadingDelay);
        }

        
        public async UniTask LoadSceneAsync(SceneName sceneName, float delay) {


            _currentLoadOperation = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Single);
            _currentLoadOperation.allowSceneActivation = false; // 자동 로그인 취소
            await UniTask.WaitForSeconds(delay); // 대기
            _currentLoadOperation.allowSceneActivation = true;
            _currentLoadOperation = null;
        }

        public SceneName GetCurrentSceneName() {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (Enum.TryParse<SceneName>(currentSceneName, out SceneName sceneName)) {
                return sceneName;
            }

            GameDebug.LogWarning($"알 수 없는 씬 이름: {currentSceneName}");
            return SceneName.None;
        }

        public float GetLoadingProgress() {
            return _currentLoadOperation == null ? 0 : _currentLoadOperation.progress;
        }
    }
}