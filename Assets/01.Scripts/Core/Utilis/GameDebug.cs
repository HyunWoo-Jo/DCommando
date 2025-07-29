using UnityEngine;
using System.Diagnostics;
namespace Game.Core
{
    public static class GameDebug {
        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void Log(object message) {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogWarning(object message) {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("DEVELOPMENT_BUILD")]
        public static void LogError(object message) {
            UnityEngine.Debug.LogError(message);
        }
    }
}
