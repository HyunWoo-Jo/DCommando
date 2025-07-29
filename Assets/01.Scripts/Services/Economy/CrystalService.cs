using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Core;
using Game.Data;
namespace Game.Services
{
    public class CrystalService : ICrystalService
    {
        public CrystalService()
        {
            InitializeFirebase();
        }
        
        private void InitializeFirebase()
        {
            GameDebug.Log("Firebase 초기화 준비됨 (구현 예정)");
        }
        
        public async UniTask<CrystalData> LoadCrystalDataAsync()
        {
            await UniTask.Delay(500);
            return new CrystalData(100, 50);
        }
        
        public async UniTask SaveCrystalDataAsync(CrystalData crystalData)
        {
            await UniTask.Delay(300);
        }
        
        public async UniTask LogCrystalGainAsync(int amount, string source, bool isPaid)
        {
            await UniTask.Delay(100);
            GameDebug.Log($"크리스탈 획득 로그: +{amount} ({source}), 유료: {isPaid}");
        }
        
        public async UniTask LogCrystalSpendAsync(int amount, string purpose)
        {
            await UniTask.Delay(100);
            GameDebug.Log($"크리스탈 소모 로그:  -{amount} ({purpose})");
        }
        
        public async UniTask<bool> ValidateCrystalTransactionAsync(int amount, string transactionId)
        {
            await UniTask.Delay(200);
            GameDebug.Log($"거래 검증 시뮬레이션: {amount}, {transactionId}");
            return true;
        }
        
        public async UniTask<bool> SyncCrystalWithServerAsync()
        {
            await UniTask.Delay(1000);
            GameDebug.Log($"서버 동기화 시뮬레이션");
            return true;
        }
    }
}