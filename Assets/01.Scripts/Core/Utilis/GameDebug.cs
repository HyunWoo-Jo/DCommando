using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Core {
    /// <summary>
    /// 빌드 시 자동으로 제거되는 디버그 로그 시스템
    /// Release 빌드에서는 모든 로그가 컴파일 단계에서 제거됨
    /// </summary>
    public static class GameDebug {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message) {
            Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message) {
            Debug.LogWarning(message);
        }
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message) {
            Debug.LogError(message);
        }
    }
}