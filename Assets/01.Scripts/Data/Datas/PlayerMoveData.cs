using UnityEngine;
using R3;
using CustomUtilis;
namespace Data
{
    /// <summary>
    /// 플레이어의 이동 위치 Data
    /// </summary>
    public class PlayerMoveData
    {
        public ReactiveProperty<Vector2> moveDirObservable = new(Vector2.zero, AlwaysFalseComparer<Vector2>.Instance); // 이동 방향 , 같은 값이 들어와도 갱신
    }
}
