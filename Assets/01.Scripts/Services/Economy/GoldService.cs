using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Services
{
    public class GoldService : IGoldService
    {
        private const string GOLD_KEY = "PlayerGold";
        
        public async UniTask<int> LoadGoldAsync()
        {
            // 실제로는 서버나 로컬 저장소에서 로드
            await UniTask.Delay(100); // 비동기 시뮬레이션
            return PlayerPrefs.GetInt(GOLD_KEY, 1000);
        }
        
        public async UniTask SaveGoldAsync(int gold)
        {
            // 실제로는 서버나 로컬 저장소에 저장
            await UniTask.Delay(100); // 비동기 시뮬레이션
            PlayerPrefs.SetInt(GOLD_KEY, gold);
            PlayerPrefs.Save();
        }
    }
}