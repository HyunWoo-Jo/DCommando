using UnityEngine;
using R3;
namespace Data
{
    public class InputMoveData
    {
        public ReactiveProperty<Vector3> moveDirObservable = new(); // 이동 방향
    }
}
