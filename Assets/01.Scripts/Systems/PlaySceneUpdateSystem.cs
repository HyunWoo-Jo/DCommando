using UnityEngine;
using Zenject;
namespace Game.Systems
{

    /// <summary>
    /// PlayScene에서 Update가 이루어지는 시스템을 중앙집권으로 컨트롤
    /// </summary>
    public class PlaySceneUpdateSystem : MonoBehaviour
    {
        [Inject] public InputSystem _inputSystem;

        private void Update() {
            _inputSystem.Update();
        }
    }
}
