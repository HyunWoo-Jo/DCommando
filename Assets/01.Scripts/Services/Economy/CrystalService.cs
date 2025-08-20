using Cysharp.Threading.Tasks;
using UnityEngine;
using Game.Core;
using Game.Data;
using Zenject;
namespace Game.Services {
    public class CrystalService : ICrystalService {
        [Inject] private INetworkService _networkService;

        public async UniTask<CrystalData> LoadCrystalDataAsync() {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return new CrystalData();
                }

                return await _networkService.LoadCrystalDataAsync();
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 데이터 로드 실패: {ex.Message}");
                return new CrystalData();
            }
        }

        public async UniTask SaveCrystalDataAsync(CrystalData crystalData) {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return;
                }

                await _networkService.SaveCrystalDataAsync(crystalData);
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 데이터 저장 실패: {ex.Message}");
            }
        }

        public async UniTask<bool> AddFreeCrystalAsync(int amount, string source) {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return false;
                }

                bool success = await _networkService.UpdateFreeCrystalAsync(amount);
                if (success) {
                    await LogCrystalGainAsync(amount, source, false);
                }
                return success;
            } catch (System.Exception ex) {
                GameDebug.LogError($"무료 크리스탈 추가 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> AddPaidCrystalAsync(int amount, string source) {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return false;
                }

                bool success = await _networkService.UpdatePaidCrystalAsync(amount);
                if (success) {
                    await LogCrystalGainAsync(amount, source, true);
                }
                return success;
            } catch (System.Exception ex) {
                GameDebug.LogError($"유료 크리스탈 추가 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> SpendFreeCrystalAsync(int amount, string purpose) {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return false;
                }

                bool success = await _networkService.UpdateFreeCrystalAsync(-amount);
                if (success) {
                    await LogCrystalSpendAsync(amount, purpose);
                }
                return success;
            } catch (System.Exception ex) {
                GameDebug.LogError($"무료 크리스탈 소모 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> SpendPaidCrystalAsync(int amount, string purpose) {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return false;
                }

                bool success = await _networkService.UpdatePaidCrystalAsync(-amount);
                if (success) {
                    await LogCrystalSpendAsync(amount, purpose);
                }
                return success;
            } catch (System.Exception ex) {
                GameDebug.LogError($"유료 크리스탈 소모 실패: {ex.Message}");
                return false;
            }
        }
        #region 추후 수정해야하는 로직 (로그를 실제로 넘겨야함)
        public async UniTask LogCrystalGainAsync(int amount, string source, bool isPaid) {
            try {
                string crystalType = isPaid ? "유료" : "무료";
                GameDebug.Log($"크리스탈 획득: +{amount} {crystalType} ({source})");
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 획득 로그 실패: {ex.Message}");
            }
        }


        public async UniTask LogCrystalSpendAsync(int amount, string purpose) {
            try {
                GameDebug.Log($"크리스탈 소모: -{amount} ({purpose})");
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 소모 로그 실패: {ex.Message}");
            }
        }
        #endregion
        public async UniTask<bool> ValidateCrystalTransactionAsync(int amount, string transactionId) {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return false;
                }

                GameDebug.Log($"거래 검증: {amount} 크리스탈, ID={transactionId}");

                bool isConnected = await _networkService.IsConnectedAsync();
                if (!isConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                // 거래 검증 로직 (임시로 성공 반환)
                await UniTask.Delay(200);
                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"거래 검증 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> SyncCrystalWithServerAsync() {
            try {
                if (_networkService == null || !_networkService.IsInitialized) {
                    GameDebug.LogError("네트워크 서비스 초기화되지 않음");
                    return false;
                }

                bool isConnected = await _networkService.IsConnectedAsync();
                if (!isConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                var crystalData = await _networkService.LoadCrystalDataAsync();
                GameDebug.Log($"서버 동기화 완료: 무료={crystalData.freeCrystal}, 유료={crystalData.paidCrystal}");

                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"서버 동기화 실패: {ex.Message}");
                return false;
            }
        }
    }
}