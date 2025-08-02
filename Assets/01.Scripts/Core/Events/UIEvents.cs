using Game.Core;
using Microsoft.Cci;
using System;
using UnityEngine;

namespace Game.Core.Event {
    /// <summary>
    /// UI 생성 요청 이벤트
    /// </summary>
    public readonly struct UICreationEvent {
        public readonly UIName uiName;
        public readonly Action<GameObject> OnCreation; // 생성 되면 수행될 작업
        public UICreationEvent(UIName name, Action<GameObject> onCreatedHandle) {
            uiName = name;
            OnCreation = onCreatedHandle;
        }
    }
    /// <summary>
    /// UI 제거 요청 이벤트
    /// </summary>
    public readonly struct UICloseEvent {
        public readonly UIName uiName;
        public readonly GameObject uiObj;
        /// <summary>
        /// UI Type이 HUD의 경우 다중 생성 UI로 obj를 넣어줘야함
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public UICloseEvent(UIName name, GameObject obj = null) {
            uiName = name;
            uiObj = obj;
        }
    }

    /// <summary>
    /// UI가 성공적으로 열렸을 때 발생하는 이벤트
    /// </summary>  
    public readonly struct UIOpenedNotificationEvent {
        public readonly UIName uiName;
        public readonly UIType uiType;
        public readonly GameObject uiObject;

        public UIOpenedNotificationEvent(UIName uiName, UIType uiType, GameObject uiObject) {
            this.uiName = uiName;
            this.uiType = uiType;
            this.uiObject = uiObject;
        }
    }

    /// <summary>
    /// UI가 성공적으로 닫혔을 때 발생하는 이벤트
    /// </summary>
    public readonly struct UIClosedNotificationEvent {
        public readonly UIName uiName;

        public UIClosedNotificationEvent(UIName uiName) {
            this.uiName = uiName;
        }
    }

}
