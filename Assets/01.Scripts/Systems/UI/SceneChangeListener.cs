using Game.Core;
using Game.Core.Event;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Systems {
    /// <summary>
    /// 씬 변경을 감지하고 반응
    /// </summary>
    public class SceneChangeListener : IInitializable, IDisposable {
        private const float CHANGED_SCENE_WIPE_DURATION = 0.5f;

        private WipeDirection _wipeDirection;
        private float _wipeDuration;
        private bool _isWipeAutoClose;

        private IDisposable _loadingDisposable;
        private IDisposable _openedDisposable;

        #region 초기화 Zenject에서 관리
        public void Initialize() {
            SceneManager.activeSceneChanged += ChangedScene;
            _loadingDisposable = EventBus.Subscribe<SceneLoadingEvent>(LoadingScene);

        }
        public void Dispose() {
            SceneManager.activeSceneChanged -= ChangedScene;
            _loadingDisposable?.Dispose();
        }
        #endregion


        /// <summary>
        /// 씬이 변경될때 호출 (고정된 Wipe시간)
        /// </summary>
        private void ChangedScene(Scene oldScene, Scene newScene) {
            // 로딩씬은 이펙트 패스
            if (newScene.name == SceneName.LoadingScene.ToString()) return; 
            _wipeDirection = WipeDirection.FillRight;
            _wipeDuration = CHANGED_SCENE_WIPE_DURATION;
            _isWipeAutoClose = true;
            OpenWipeEffect();
        }

        /// <summary>
        /// 씬 로딩이 요청되었을때 호출 (delay를 기준으로 Wipe)
        /// </summary>
        /// <param name="sceneLoadingEvent"></param>
        private void LoadingScene(SceneLoadingEvent sceneLoadingEvent) {
            if (sceneLoadingEvent.curSceneName == SceneName.LoadingScene) return;
            Debug.Log(sceneLoadingEvent.curSceneName.ToString());
            _wipeDirection = WipeDirection.Left;
            _wipeDuration = sceneLoadingEvent.delay;
            _isWipeAutoClose = false;
            OpenWipeEffect();
        }

        /// <summary>
        /// Wipe Effect 발생
        /// </summary>
        private void OpenWipeEffect() {
            // 이벤트 등록
            _openedDisposable?.Dispose();
            _openedDisposable = EventBus.Subscribe<UIOpenedNotificationEvent>(OnOpenWipeUI);
            // 생성 요청  
            EventBus.Publish(new UICreationEvent(-1, UIName.WipeEffect_UI));
        }

        private void OnOpenWipeUI(UIOpenedNotificationEvent openedEvent) {
            if(openedEvent.uiName == UIName.WipeEffect_UI) {
                IWipeUI wipeUI = openedEvent.uiObject.GetComponent<IWipeUI>();
                wipeUI.Wipe(_wipeDirection, _wipeDuration, _isWipeAutoClose);

                _openedDisposable?.Dispose();
                _openedDisposable = null;
            }
        }
       
    }
}