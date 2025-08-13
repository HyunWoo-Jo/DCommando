using UnityEngine;

namespace Game.Core.Event
{
    /// <summary>
    /// UI 생성 요청 이벤트 (비동기)
    /// </summary>
    public readonly struct UICreationEventAsync
    {
        public readonly int id;
        public readonly UIName uiName;
        public UICreationEventAsync(int id, UIName name)
        {
            this.id = id;
            uiName = name;
        }
    }
    
    /// <summary>
    /// UI 생성 요청 이벤트
    /// </summary>
    public readonly struct UICreationEvent
    {
        public readonly int id;
        public readonly UIName uiName;
        public UICreationEvent(int id, UIName name)
        {
            this.id = id;
            uiName = name;
        }
    }
    
    /// <summary>
    /// UI 제거 요청 이벤트
    /// </summary>
    public readonly struct UICloseEvent
    {
        public readonly int id;
        public readonly UIName uiName;
        public readonly GameObject uiObj;
        
        /// <summary>
        /// UI Type이 HUD의 경우 다중 생성 UI로 obj를 넣어줘야함
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public UICloseEvent(int id, UIName name, GameObject obj = null)
        {
            this.id = id;
            uiName = name;
            uiObj = obj;
        }
    }

    /// <summary>
    /// UI가 성공적으로 열렸을 때 발생하는 이벤트
    /// </summary>  
    public readonly struct UIOpenedNotificationEvent
    {
        public readonly int id; // 누가 발행한 이벤트인지
        public readonly UIName uiName;
        public readonly UIType uiType;
        public readonly GameObject uiObject;

        public UIOpenedNotificationEvent(int id, UIName uiName, UIType uiType, GameObject uiObject)
        {
            this.id = id;
            this.uiName = uiName;
            this.uiType = uiType;
            this.uiObject = uiObject;
        }
    }

    /// <summary>
    /// UI가 성공적으로 닫혔을 때 발생하는 이벤트
    /// </summary>
    public readonly struct UIClosedNotificationEvent
    {
        public readonly int id; // 누가 발행한 이벤트인지
        public readonly UIName uiName;

        public UIClosedNotificationEvent(int id, UIName uiName)
        {
            this.id = id;
            this.uiName = uiName;
        }
    }

    #region 이동
    /// <summary>
    /// 특정위치로 UI 이동을 명령하는 Event
    /// </summary>
    public readonly struct UIGoToEvent
    {
        public readonly UIMoveName targetName;

        public UIGoToEvent(UIMoveName targetName)
        {
            this.targetName = targetName;
        }
    }
    #endregion
}