using Game.Core;
using System;
using UnityEngine;

namespace Game.Core.Event
{
    public class UICreationEvent {
        public readonly UIName ui_Name;
        public readonly UIType ui_Type;
        public readonly Action<GameObject> OnCreation; // ���� �Ǹ� ����� �۾�
        public UICreationEvent(UIName name, UIType type, Action<GameObject> onCreatedHandle){
            ui_Name = name;
            ui_Type = type;
            OnCreation = onCreatedHandle;
        }
    }
}
