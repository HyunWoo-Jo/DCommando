
using Zenject;
using System;
using Data;
using R3;
using UnityEngine;
namespace UI
{
    public class ControllerViewModel 
    {
        [Inject] private InputMoveData _inputMoveData;


        /// <summary>
        /// RO Data
        /// </summary>
        public ReadOnlyReactiveProperty<Vector3> RO_MoveDir => _inputMoveData.moveDirObservable;

        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() {

        }

    }
} 
