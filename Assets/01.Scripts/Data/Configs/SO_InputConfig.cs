using UnityEngine;

namespace Game.Data
{
    /// <summary>
    /// Input 관련 설정값
    /// </summary>
    [CreateAssetMenu(fileName = "InputConfig", menuName = "Config/Input")]
    public class SO_InputConfig : ScriptableObject
    {
        [Header("Input 설정")]
        public float clickThreshold = 0.1f;
        public float dragThreshold = 10f;
        public bool enableMultiTouch = false;
    }
}