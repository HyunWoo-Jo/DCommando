using UnityEngine;
using Zenject;
namespace Game.Systems
{

    /// <summary>
    /// PlayScene���� Update�� �̷������ �ý����� �߾��������� ��Ʈ��
    /// </summary>
    public class PlaySceneUpdateSystem : MonoBehaviour
    {
        [Inject] public InputSystem _inputSystem;

        private void Update() {
            _inputSystem.Update();
        }
    }
}
