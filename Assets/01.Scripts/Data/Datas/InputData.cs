using UnityEngine;
using R3;
using System;
using CustomUtilis;
namespace Data
{
    /// <summary>
    /// Input 판별용 
    /// </summary>
    public class InputData
    {
        public ReactiveProperty<InputType> inputTypeObservable = new(InputType.None, AlwaysFalseComparer<InputType>.Instance); // 같은 값이여도 알림 발생
        public Func<Vector2> GetFirstFramePointPosition;
        public Func<Vector2> GetCurrentPointPosition;

    }
}
