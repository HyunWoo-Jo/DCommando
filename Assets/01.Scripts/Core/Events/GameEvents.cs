using UnityEngine;

namespace Game.Core {

    /// <summary>
    /// 게임 정지 이벤트
    /// </summary>
    public readonly struct PauseGameEvent { }

    /// <summary>
    /// 게임 시작 이벤트
    /// </summary>
    public readonly struct ResumeGameEvent { }
}