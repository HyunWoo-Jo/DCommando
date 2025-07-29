using Game.Core;
using System;
using UnityEngine;

namespace Game.Systems
{
    public class UICreationEvent {
        public readonly UI_Name ui_Name;
        public readonly Action<GameObject> OnCreation; // ���� �Ǹ� ����� �۾�
        public UICreationEvent(UI_Name ui_Name, Action<GameObject> onCreatedHandle) {
            this.ui_Name = ui_Name;
            OnCreation = onCreatedHandle;
        }
    }
}
