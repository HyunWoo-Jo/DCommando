using UnityEngine;

namespace Game.Core
{
    public enum UIType {
        Screen,     // 전체 화면 UI
        Popup,      // 팝업 UI
        HUD,        // HUD UI (HUD 로 생성하는 UI는 다중생성 UI)
        Overlay     // 오버레이 UI
    }
}
