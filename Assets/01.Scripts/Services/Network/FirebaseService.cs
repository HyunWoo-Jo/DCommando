using UnityEngine;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System;
using Zenject;
using Game.Core;
using Game.Data;
namespace Game.Services {
    public class FirebaseService : INetworkService {
        private FirebaseAuth _auth;
        private FirebaseUser _user;
        private DatabaseReference _databaseReference;

        public bool IsInitialized { get; private set; }
        public bool IsConnected => _user != null && _auth != null;
        public string CurrentUserId => _user?.UserId ?? string.Empty;

        public async UniTask InitializeAsync() {
            try {
                GameDebug.Log("Firebase 초기화 시작");
                // Firebase 서비스 초기화
                _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                _auth = FirebaseAuth.DefaultInstance;

                string uid = SystemInfo.deviceUniqueIdentifier;
                // 게스트 계정 정보 // 임시 
                string guestEmail = $"guest{uid}@game.com";

                // 로그인 시도
                GameDebug.Log("Firebase 로그인 시도");
                bool loginSuccess = await TryLoginAsync(guestEmail, uid);

                if (!loginSuccess) {
                    // 로그인 실패시 계정 생성 시도
                    await TryCreateAccountAsync(guestEmail, uid);
                }

                // 연결 상태 최종 확인
                if (!await IsConnectedAsync()) {
                    throw new Exception("Firebase 연결 실패");
                }

                IsInitialized = true;
                GameDebug.Log("Firebase 초기화 성공");
            } catch (Exception ex) {
                GameDebug.LogError($"Firebase 초기화 실패: {ex.Message}");
                throw;
            }
        }

        public async UniTask<bool> IsConnectedAsync() {
            if (_user == null || _auth == null) return false;

            try {
                await _user.TokenAsync(true).AsUniTask(); // 토큰 갱신 요청
                return true;
            } catch {
                return false;
            }
        }
        #region 인증 관련 매서드
        public void SignOut() {
            if (_auth != null) {
                _auth.SignOut();
                _user = null;
                GameDebug.Log("Firebase 로그아웃");
            }
        }

        private async UniTask<bool> TryLoginAsync(string email, string password) {
            try {
                var authResult = await _auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
                _user = authResult.User;
                GameDebug.Log($"Login 성공: {_user.UserId}");
                return true;
            } catch (Exception ex) {
                GameDebug.Log($"Login 실패: {ex.Message}");
                return false;
            }
        }

        private async UniTask TryCreateAccountAsync(string email, string password) {
            try {
                var authResult = await _auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
                _user = authResult.User;
                GameDebug.Log($"Account 생성 성공: {_user.UserId}");
            } catch (Exception ex) {
                GameDebug.LogError($"Account 생성 실패: {ex.Message}");
                throw new Exception("Network Error: Unable to create account");
            }
        }
        #endregion

        #region 크리스탈 관련 매서드
        public async UniTask<CrystalData> LoadCrystalDataAsync() {
            try {
                if (!IsConnected) {
                    throw new System.Exception("Firebase not connected");
                }

                // UserData/{uid}/CrystalData 경로에서 데이터 로드
                var snapshot = await _databaseReference
                    .Child("UserData")
                    .Child(CurrentUserId)
                    .Child("CrystalData")
                    .GetValueAsync()
                    .AsUniTask();

                if (snapshot.Exists) {
                    string jsonData = snapshot.GetRawJsonValue();
                    var crystalData = JsonUtility.FromJson<CrystalData>(jsonData);
                    GameDebug.Log($"크리스탈 데이터 로드완료: 무료={crystalData.freeCrystal}, 유료={crystalData.paidCrystal}");
                    return crystalData;
                } else {
                    // 데이터가 없으면 기본값 반환
                    GameDebug.Log("크리스탈 데이터 없음, 기본값 반환");
                    return new CrystalData();
                }
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 데이터 로드실패: {ex.Message}");
                throw;
            }
        }

        public async UniTask SaveCrystalDataAsync(CrystalData crystalData) {
            try {
                if (!IsConnected) {
                    throw new System.Exception("Firebase not connected");
                }

                string jsonData = JsonUtility.ToJson(crystalData);

                // UserData/{uid}/CrystalData 경로에 데이터 저장
                await _databaseReference
                    .Child("UserData")
                    .Child(CurrentUserId)
                    .Child("CrystalData")
                    .SetRawJsonValueAsync(jsonData)
                    .AsUniTask();

                GameDebug.Log($"크리스탈 데이터 저장완료: 무료={crystalData.freeCrystal}, 유료={crystalData.paidCrystal}");
            } catch (System.Exception ex) {
                GameDebug.LogError($"크리스탈 데이터 저장실패: {ex.Message}");
                throw;
            }
        }

        public async UniTask<bool> UpdateFreeCrystalAsync(int amount) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("연결안됨");
                    return false;
                }

                // 현재 데이터 로드
                var currentData = await LoadCrystalDataAsync();

                // 무료 크리스탈 업데이트
                currentData.freeCrystal += amount;

                // 음수 방지
                if (currentData.freeCrystal < 0) {
                    currentData.freeCrystal = 0;
                }

                // 저장
                await SaveCrystalDataAsync(currentData);

                GameDebug.Log($"무료크리스탈 업데이트: {amount:+#;-#;0}, 현재={currentData.freeCrystal}");
                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"무료크리스탈 업데이트 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> UpdatePaidCrystalAsync(int amount) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("연결안됨");
                    return false;
                }

                // 현재 데이터 로드
                var currentData = await LoadCrystalDataAsync();

                // 유료 크리스탈 업데이트
                currentData.paidCrystal += amount;

                // 음수 방지
                if (currentData.paidCrystal < 0) {
                    currentData.paidCrystal = 0;
                }

                // 저장
                await SaveCrystalDataAsync(currentData);

                GameDebug.Log($"유료크리스탈 업데이트: {amount:+#;-#;0}, 현재={currentData.paidCrystal}");
                return true;
            } catch (System.Exception ex) {
                GameDebug.LogError($"유료크리스탈 업데이트 실패: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region 장비 관련 매서드
        public async UniTask<EquipData> LoadEquipDataAsync() {
            try {
                if (!IsConnected) {
                    throw new Exception("Firebase not connected");
                }

                // UserData/{uid}/Equip 경로에서 데이터 로드
                var snapshot = await _databaseReference
                    .Child("UserData")
                    .Child(CurrentUserId)
                    .Child("Equip")
                    .GetValueAsync()
                    .AsUniTask();

                if (snapshot.Exists) {
                    string jsonData = snapshot.GetRawJsonValue();
                    var equipData = JsonUtility.FromJson<EquipData>(jsonData);
                    GameDebug.Log($"장비 데이터 로드 완료: 무기={equipData.equippedWeapon}, 방어구={equipData.equippedArmor}, 보유={equipData.ownedEquipments.Count}개");
                    return equipData;
                } else {
                    // 데이터가 없으면 기본값 반환
                    GameDebug.Log("장비 데이터 없음, 기본값 반환");
                    return new EquipData();
                }
            } catch (Exception ex) {
                GameDebug.LogError($"장비 데이터 로드 실패: {ex.Message}");
                throw;
            }
        }

        public async UniTask SaveEquipDataAsync(EquipData equipData) {
            try {
                if (!IsConnected) {
                    throw new Exception("Firebase not connected");
                }

                string jsonData = JsonUtility.ToJson(equipData);

                // UserData/{uid}/Equip 경로에 데이터 저장
                await _databaseReference
                    .Child("UserData")
                    .Child(CurrentUserId)
                    .Child("Equip")
                    .SetRawJsonValueAsync(jsonData)
                    .AsUniTask();

                GameDebug.Log($"장비 데이터 저장 완료: 무기={equipData.equippedWeapon}, 방어구={equipData.equippedArmor}");
            } catch (Exception ex) {
                GameDebug.LogError($"장비 데이터 저장 실패: {ex.Message}");
                throw;
            }
        }

        public async UniTask<bool> UpdateEquippedWeaponAsync(EquipName equipName) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                var currentData = await LoadEquipDataAsync();

                // 장비를 보유하고 있는지 확인
                if (equipName != EquipName.None && !currentData.HasEquipment(equipName)) {
                    GameDebug.LogError($"보유하지 않은 장비입니다: {equipName}");
                    return false;
                }

                currentData.EquipWeapon(equipName);
                await SaveEquipDataAsync(currentData);

                GameDebug.Log($"무기 장착 업데이트: {equipName}");
                return true;
            } catch (Exception ex) {
                GameDebug.LogError($"무기 장착 업데이트 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> UpdateEquippedArmorAsync(EquipName equipName) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                var currentData = await LoadEquipDataAsync();

                if (equipName != EquipName.None && !currentData.HasEquipment(equipName)) {
                    GameDebug.LogError($"보유하지 않은 장비입니다: {equipName}");
                    return false;
                }

                currentData.EquipArmor(equipName);
                await SaveEquipDataAsync(currentData);

                GameDebug.Log($"방어구 장착 업데이트: {equipName}");
                return true;
            } catch (Exception ex) {
                GameDebug.LogError($"방어구 장착 업데이트 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> UpdateEquippedAccessoryAsync(EquipName equipName) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                var currentData = await LoadEquipDataAsync();

                if (equipName != EquipName.None && !currentData.HasEquipment(equipName)) {
                    GameDebug.LogError($"보유하지 않은 장비입니다: {equipName}");
                    return false;
                }

                currentData.EquipAccessory(equipName);
                await SaveEquipDataAsync(currentData);

                GameDebug.Log($"악세사리 장착 업데이트: {equipName}");
                return true;
            } catch (Exception ex) {
                GameDebug.LogError($"악세사리 장착 업데이트 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> AddOwnedEquipmentAsync(EquipName equipName) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                var currentData = await LoadEquipDataAsync();

                if (currentData.HasEquipment(equipName)) {
                    GameDebug.Log($"이미 보유한 장비입니다: {equipName}");
                    return true;
                }

                currentData.AddEquipment(equipName);
                await SaveEquipDataAsync(currentData);

                GameDebug.Log($"장비 획득: {equipName}");
                return true;
            } catch (Exception ex) {
                GameDebug.LogError($"장비 획득 실패: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> RemoveOwnedEquipmentAsync(EquipName equipName) {
            try {
                if (!IsConnected) {
                    GameDebug.LogError("네트워크 연결 안됨");
                    return false;
                }

                var currentData = await LoadEquipDataAsync();

                if (!currentData.HasEquipment(equipName)) {
                    GameDebug.LogError($"보유하지 않은 장비입니다: {equipName}");
                    return false;
                }

                currentData.RemoveEquipment(equipName);
                await SaveEquipDataAsync(currentData);

                GameDebug.Log($"장비 제거: {equipName}");
                return true;
            } catch (Exception ex) {
                GameDebug.LogError($"장비 제거 실패: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}