using UnityEngine;
using UnityEngine.EventSystems;
using Data;

namespace GamePlay
{
   
    /// <summary>
    /// Input 상태를 판별하기위한 interface
    /// </summary>
    public interface IInputStrategy {

        void UpdateInput(); // 인풋 갱신
    }

}
