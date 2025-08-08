using Cysharp.Threading.Tasks;
using Game.Services;
using UnityEngine;
using Zenject;
using Game.Core;
namespace Game.Systems
{
    /// <summary>
    /// 게임을 초기화 하는 System 
    /// </summary>
    public class GameInitSystem : MonoBehaviour {
        [Inject] private INetworkService _networkService;
        [Inject] private CrystalSystem _crystalSystem;
        [Inject] private EquipSystem _equipSystem;
        private void Start() {
            _ = InitializeGameAsync();
        }

        private async UniTask InitializeGameAsync() {
            GameDebug.Log("초기화 시작");

            try {
                // Network 초기화
                await _networkService.InitializeAsync();

                await _crystalSystem.InitializeAsync();

                await _equipSystem.InitializeAsync();

            } catch (System.Exception ex) {
                GameDebug.LogError($"초기화 실패: {ex.Message}");
                // 초기화 실패 처리 로직
            }
        }

    }
}
