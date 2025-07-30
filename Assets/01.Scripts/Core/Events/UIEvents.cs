using Game.Core;
using Microsoft.Cci;
using System;
using UnityEngine;

namespace Game.Core.Event
{
    public readonly struct UICreationEvent {
        public readonly UIName uiName;
        public readonly UIType uiType;
        public readonly Action<GameObject> OnCreation; // 생성 되면 수행될 작업
        public UICreationEvent(UIName name, UIType type, Action<GameObject> onCreatedHandle){
            uiName = name;
            uiType = type;
            OnCreation = onCreatedHandle;
        }
    }

    public readonly struct UICloseEvent {
        public readonly UIName uiName;
        public UICloseEvent(UIName name) {
            uiName = name;
        }

    }
}
