using Game.Models;
using Game.Services;
using Game.Policies;
using R3;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core;

namespace Game.Systems {
    public class CrystalSystem {
        [Inject] private CrystalModel _crystalModel;
        [Inject] private ICrystalService _crystalService;
        [Inject] private ICrystalPolicy _crystalPolicy;

        public ReadOnlyReactiveProperty<int> RORP_Crystal => _crystalModel.RORP_CurrentCrystal;
        public ReadOnlyReactiveProperty<int> RORP_FreeCrystal => _crystalModel.RORP_FreeCrystal;
        public ReadOnlyReactiveProperty<int> RORP_PaidCrystal => _crystalModel.RORP_PaidCrystal;

        #region 초기화
        /// <summary>
        /// 크리스탈 시스템 초기화
        /// </summary>
        public async UniTask InitializeAsync() {
            try {
                GameDebug.Log("크리스탈 시스템 초기화 시작");

                // Firebase에서 데이터 로드
                var crystalData = await _crystalService.LoadCrystalDataAsync();
                if (crystalData != null) {
                    _crystalModel.SetFreeCrystal(crystalData.freeCrystal);
                    _crystalModel.SetPaidCrystal(crystalData.paidCrystal);
                    GameDebug.Log($"서버 데이터 로드완료: 무료={crystalData.freeCrystal}, 유료={crystalData.paidCrystal}");
                } else {
                    // 기본값 설정
                    _crystalModel.SetFreeCrystal(0);
                    _crystalModel.SetPaidCrystal(0);
                    GameDebug.Log("기본값으로 초기화: 무료=0, 유료=0");
                }

                GameDebug.Log($"크리스탈 시스템 초기화 완료. 총 크리스탈: {RORP_Crystal.CurrentValue}");
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 시스템 초기화 실패: {ex.Message}");

                // 실패시 기본값 설정
                _crystalModel.SetFreeCrystal(0);
                _crystalModel.SetPaidCrystal(0);
                throw;
            }
        }
        #endregion
        #region 추가소모 로직
        /// <summary>
        /// 무료 크리스탈 추가
        /// </summary>
        public async UniTask<bool> AddFreeCrystalAsync(int amount, string source) {
            try {
                if (!_crystalPolicy.IsValidTransaction(amount, source)) {
                    GameDebug.LogError($"무효한 거래: {amount}, {source}");
                    return false;
                }

                // 서버 업데이트 먼저
                bool serverSuccess = await _crystalService.AddFreeCrystalAsync(amount, source);
                if (!serverSuccess) {
                    GameDebug.LogError("서버 업데이트 실패");
                    return false;
                }

                // 로컬 모델 업데이트
                _crystalModel.AddCrystal(amount, false);

                GameDebug.Log($"무료 크리스탈 획득: +{amount} ({source}), 총: {RORP_Crystal.CurrentValue}");
                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"무료 크리스탈 획득 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 유료 크리스탈 추가
        /// </summary>
        public async UniTask<bool> AddPaidCrystalAsync(int amount, string source) {
            try {
                if (!_crystalPolicy.IsValidTransaction(amount, source)) {
                    GameDebug.LogError($"무효한 거래: {amount}, {source}");
                    return false;
                }

                // 서버 업데이트 먼저
                bool serverSuccess = await _crystalService.AddPaidCrystalAsync(amount, source);
                if (!serverSuccess) {
                    GameDebug.LogError("서버 업데이트 실패");
                    return false;
                }

                // 로컬 모델 업데이트
                _crystalModel.AddCrystal(amount, true);

                GameDebug.Log($"유료 크리스탈 획득: +{amount} ({source}), 총: {RORP_Crystal.CurrentValue}");
                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"유료 크리스탈 획득 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 크리스탈 소모 (무료 우선 사용)
        /// </summary>
        public async UniTask<bool> SpendCrystalAsync(int amount, string purpose) {
            try {
                if (!_crystalModel.CanSpend(amount)) {
                    GameDebug.LogError($"크리스탈 부족: 필요={amount}, 보유={RORP_Crystal.CurrentValue}");
                    return false;
                }

                if (!_crystalPolicy.CanSpendCrystal(RORP_Crystal.CurrentValue, amount)) {
                    GameDebug.LogError($"정책상 소모 불가: {amount}");
                    return false;
                }

                // 서버 업데이트 (무료/유료 분리해서 처리)
                int freeAmount = RORP_FreeCrystal.CurrentValue;
                int paidAmount = RORP_PaidCrystal.CurrentValue;

                bool serverSuccess = false;
                if (freeAmount >= amount) {
                    // 무료 크리스탈만으로 충분
                    serverSuccess = await _crystalService.SpendFreeCrystalAsync(amount, purpose);
                } else if (freeAmount > 0) {
                    // 무료 + 유료 크리스탈 사용
                    int remainingAmount = amount - freeAmount;
                    bool freeSuccess = await _crystalService.SpendFreeCrystalAsync(freeAmount, purpose);
                    bool paidSuccess = await _crystalService.SpendPaidCrystalAsync(remainingAmount, purpose);
                    serverSuccess = freeSuccess && paidSuccess;
                } else {
                    // 유료 크리스탈만 사용
                    serverSuccess = await _crystalService.SpendPaidCrystalAsync(amount, purpose);
                }

                if (!serverSuccess) {
                    GameDebug.LogError("서버 업데이트 실패");
                    return false;
                }

                // 로컬 모델 업데이트
                bool localSuccess = _crystalModel.SpendCrystal(amount);
                if (!localSuccess) {
                    GameDebug.LogError("로컬 모델 업데이트 실패");
                    return false;
                }

                GameDebug.Log($"크리스탈 소모: -{amount} ({purpose}), 총: {RORP_Crystal.CurrentValue}");
                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 소모 실패: {ex.Message}");
                return false;
            }
        }
        #endregion
        #region 검증
        /// <summary>
        /// 크리스탈 소모 가능 여부
        /// </summary>
        public bool CanSpend(int amount) {
            return _crystalModel.CanSpend(amount) && _crystalPolicy.CanSpendCrystal(RORP_Crystal.CurrentValue, amount);
        }
     

        /// <summary>
        /// 서버와 동기화
        /// </summary>
        public async UniTask<bool> SyncWithServerAsync() {
            try {
                bool success = await _crystalService.SyncCrystalWithServerAsync();
                if (success) {
                    // 동기화 후 데이터 다시 로드
                    var crystalData = await _crystalService.LoadCrystalDataAsync();
                    if (crystalData != null) {
                        _crystalModel.SetFreeCrystal(crystalData.freeCrystal);
                        _crystalModel.SetPaidCrystal(crystalData.paidCrystal);
                        GameDebug.Log($"동기화 완료: 무료={crystalData.freeCrystal}, 유료={crystalData.paidCrystal}");
                    }
                }
                return success;
            } catch (System.Exception ex) {
                GameDebug.LogError($"서버 동기화 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 거래 검증
        /// </summary>
        public async UniTask<bool> ValidateTransactionAsync(int amount, string transactionId) {
            try {
                return await _crystalService.ValidateCrystalTransactionAsync(amount, transactionId);
            } catch (System.Exception ex) {
                GameDebug.LogError($"거래 검증 실패: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}