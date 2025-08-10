using UnityEngine;

namespace Game.Core.Event {

    /// <summary>
    /// 게임 정지 이벤트
    /// </summary>
    public readonly struct PauseGameEvent { }

    /// <summary>
    /// 게임 시작 이벤트
    /// </summary>
    public readonly struct ResumeGameEvent { }


    /// <summary>
    /// 씬 전환 요청 이벤트
    /// </summary>
    public readonly struct SceneLoadingEvent {
        public readonly SceneName curSceneName;
        public readonly SceneName targetSceneName;
        public readonly float delay;

        public SceneLoadingEvent(SceneName curSceneName, SceneName targetSceneName, float delay) {
            this.curSceneName = curSceneName;
            this.targetSceneName = targetSceneName;
            this.delay = delay;
        }
    }
}