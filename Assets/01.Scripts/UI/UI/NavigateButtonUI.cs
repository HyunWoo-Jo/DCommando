using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using Game.Core.Event;
using System;
using Game.Core;
using DG.Tweening;
using UnityEngine.UIElements;
namespace Game.UI
{
    public class NavigateButtonUI : MonoBehaviour {
        
        private EventTrigger _eventTrigger;

        [SerializeField] private SO_NavigateButtonStyle _style;
        [SerializeField] private UIMoveName _targetName;
        [SerializeField] private bool _isAnimation = false;
        [SerializeField] private bool _isSizeUP = false;
        private void Awake() {
            _eventTrigger = GetComponent<EventTrigger>();
#if UNITY_EDITOR
            Assert.IsNotNull(_eventTrigger, $"{gameObject.name}: Navigrate 버튼이 존재하지 않습니다.");
            Assert.IsNotNull(_style, $"{gameObject.name}: Style 존재하지 않습니다.");
#endif
            EventTrigger.Entry entry = new();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(OnClick);
            _eventTrigger.triggers.Add(entry);

            if( _isAnimation) {
                EventBus.Subscribe<UIGoToEvent>(OnCloseAnimation);
            }
        }
        /// <summary>
        /// 이동 Event 발행
        /// </summary>
        private void OnClick(BaseEventData arg0) {
            EventBus.Publish(new UIGoToEvent(_targetName));

        }

        private void OnCloseAnimation(UIGoToEvent goToEvent) {
            if (goToEvent.targetName == _targetName && !_isSizeUP) {
                _eventTrigger.GetComponent<RectTransform>().DOSizeDelta(_style.SelectedSize, _style.AnimationDuration).SetEase(_style.AnimationEase);
                _isSizeUP = true;
            } else if (_isSizeUP) { 
                _eventTrigger.GetComponent<RectTransform>().DOSizeDelta(_style.NormalSize, _style.AnimationDuration).SetEase(_style.AnimationEase);
                _isSizeUP = false;
            }
        }

#if UNITY_EDITOR
        private void OnValidate() {
            gameObject.name = $"GoTo{_targetName.ToString()}Button";      
        }

#endif
    }
}
