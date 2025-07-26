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

            _moveDir.x = Input.GetAxisRaw("Horizontal");
            _moveDir.y = Input.GetAxisRaw("Vertical");

            _inputMoveData.moveDirObservable.Value = _moveDir;
        }
    }
}
