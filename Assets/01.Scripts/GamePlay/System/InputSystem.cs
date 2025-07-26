using Data;
using UnityEngine;
using Zenject;

namespace GamePlay
{
    public class InputSystem : MonoBehaviour
    {
        [Inject] private InputMoveData _inputMoveData;
        private Vector3 _moveDir;

        private void Update() {

            _moveDir.x = Input.GetAxis("Horizontal");
            _moveDir.y = Input.GetAxis("Vertical");

            _inputMoveData.moveDirObservable.Value = _moveDir;
        }
    }
}
