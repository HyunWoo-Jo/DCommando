using Game.Core;
using Game.Core.Event;
using UnityEngine;
using UnityEngine.Assertions;
using DG.Tweening;
using DG.Tweening.Core.Easing;
namespace Game.UI
{
    public class MainLobbyUI : MonoBehaviour {

        [SerializeField] private SO_MainLobbyUIStyle _style;
        [SerializeField] private RectTransform panelParent;

        private UIMoveName curPanel = UIMoveName.Main;

        public void Awake() {
#if UNITY_EDITOR
            Assert.IsNotNull(panelParent, "panelParent가 존재하지 않습니다");
            Assert.IsNotNull(_style, "style 존재하지 않습니다");
#endif
            EventBus.Subscribe<UIGoToEvent>(OnNavigate);
        }

        public void OnNavigate(UIGoToEvent goToEvent) {
            if (curPanel == goToEvent.targetName) return;
            switch (goToEvent.targetName) {
                case UIMoveName.Main:    
                panelParent.DOAnchorPosX(_style.MainPanelPosX, _style.MoveDuration).SetEase(Ease.InOutCirc);
                break;
                case UIMoveName.Inven:
                panelParent.DOAnchorPosX(_style.ReferenceInterval * -1, _style.MoveDuration).SetEase(Ease.InOutCirc);
                break;
            }
            curPanel = goToEvent.targetName;
        }
    }
}
