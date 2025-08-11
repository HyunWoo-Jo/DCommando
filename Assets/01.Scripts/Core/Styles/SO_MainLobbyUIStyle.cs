using UnityEngine;
using DG.Tweening;
namespace Game.Core
{
    [CreateAssetMenu(fileName = "MainLobbyUIStyle", menuName = "Styles/MainLobbyUIStyle")]
    public class SO_MainLobbyUIStyle : ScriptableObject {
        [Header("Panel Navigation")]
        [SerializeField] private float moveDuration = 0.3f;
        [SerializeField] private float referenceInterval = 1440f;
        [SerializeField] private Ease moveEase = Ease.InOutCirc;
        [Header("Panel Positions")]
        [SerializeField] private float mainPanelPosX = 0f;
        [SerializeField] private float invenPanelPosX = -1440f; // 미리 계산된 값

        public float MoveDuration => moveDuration;
        public float ReferenceInterval => referenceInterval;
        public Ease MoveEase => moveEase;
        public float MainPanelPosX => mainPanelPosX;
        public float InvenPanelPosX => invenPanelPosX;
    }
}
