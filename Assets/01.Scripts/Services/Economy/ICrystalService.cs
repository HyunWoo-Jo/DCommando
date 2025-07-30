using Cysharp.Threading.Tasks;
using Game.Data;
namespace Game.Services
{
    /// <summary>
    /// 크리스탈 서비스 인터페이스
    /// </summary>
    public interface ICrystalService
    {
        /// <summary>
        /// Firebase에서 크리스탈 데이터 로드
        /// </summary>
        UniTask<CrystalData> LoadCrystalDataAsync();
        
        /// <summary>
        /// Firebase에 크리스탈 데이터 저장
        /// </summary>
        UniTask SaveCrystalDataAsync(CrystalData crystalData);
        
        /// <summary>
        /// 크리스탈 획득 로그 저장
        /// </summary>
        UniTask LogCrystalGainAsync(int amount, string source, bool isPaid);
        
        /// <summary>
        /// 크리스탈 소모 로그 저장
        /// </summary>
        UniTask LogCrystalSpendAsync(int amount, string purpose);
        
        /// <summary>
        /// 크리스탈 거래 검증
        /// </summary>
        UniTask<bool> ValidateCrystalTransactionAsync(int amount, string transactionId);
        
        /// <summary>
        /// 서버에서 크리스탈 동기화
        /// </summary>
        UniTask<bool> SyncCrystalWithServerAsync();
    }
    
}