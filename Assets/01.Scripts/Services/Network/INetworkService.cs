using Cysharp.Threading.Tasks;
using Game.Data;
using UnityEngine;

namespace Game.Services {
    public interface INetworkService {
        bool IsInitialized { get; }
        bool IsConnected { get; }
        string CurrentUserId { get; }

        UniTask InitializeAsync();
        UniTask<bool> IsConnectedAsync();
        void SignOut();


        // Crystal Data 관련 메서드
        UniTask<CrystalData> LoadCrystalDataAsync();
        UniTask SaveCrystalDataAsync(CrystalData crystalData);
        UniTask<bool> UpdateFreeCrystalAsync(int amount);
        UniTask<bool> UpdatePaidCrystalAsync(int amount);
    }
}