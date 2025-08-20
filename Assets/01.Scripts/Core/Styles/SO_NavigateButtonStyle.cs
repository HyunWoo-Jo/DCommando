using DG.Tweening;
using UnityEngine;

namespace Game.Core
{
    [CreateAssetMenu(fileName = "NavigateButtonStyle", menuName = "Styles/NavigateButtonStyle")]
    public class SO_NavigateButtonStyle: ScriptableObject {
        [Header("Size Animation")]
        [SerializeField] private Vector2 normalSize = new Vector2(330, 220);
        [SerializeField] private Vector2 selectedSize = new Vector2(400, 300);
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private Ease animationEase = Ease.InOutElastic;

        public Vector2 NormalSize => normalSize;
        public Vector2 SelectedSize => selectedSize;
        public float AnimationDuration => animationDuration;
        public Ease AnimationEase => animationEase;
    }
}