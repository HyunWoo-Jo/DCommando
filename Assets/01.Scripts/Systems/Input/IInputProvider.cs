using UnityEngine;
using R3;

namespace Game.Systems
{
    public interface IInputProvider {
        // 이동 입력
        Observable<Vector2> OnDragEvent { get; }
        Observable<Vector2> OnDragStartEvent { get; }
        Observable<Vector2> OnDragEndEvent { get; }
        // 클릭 입력
        Observable<Vector2> OnClickEvent { get; }
        
        bool IsDragging { get; }
    }
}
