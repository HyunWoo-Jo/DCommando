using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Services
{
    public class GoldService : IGoldService
    {

        /// <summary>
        /// 추후 로그 등 활용 가능
        /// </summary>
        public async UniTask CheckGoldAsync(int gold) {
            await UniTask.Delay(100); // 비동기 시뮬레이션
        }
    }
}