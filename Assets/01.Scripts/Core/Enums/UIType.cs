using UnityEngine;

namespace Game.Core
{
    public enum UIType {
        Screen,     // 전체 화면 UI
        DynamicScreen, // 갱신이 많은 UI
        Popup,      // 팝업 UI
        HUD,        // HUD UI (HUD 로 생성하는 UI는 다중생성 UI)
        HUD1,       // HUD 보다 우선순위가 높은 UI
        HUD2,       // ...
        Overlay     // 오버레이 UI
    }
}
