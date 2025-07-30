using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "CrystalConfig", menuName = "Config/Crystal")]
    public class SO_CrystalConfig : ScriptableObject
    {
        [Header("크리스탈 기본 설정")]
        [SerializeField] private int _maxCrystalAmount = 9999999;
        [SerializeField] private int _dailyFreeCrystalLimit = 1000;
        [SerializeField] private int _startingFreeCrystal = 100;
        [SerializeField] private int _startingPaidCrystal = 0;
        
        [Header("크리스탈 획득 소스별 설정")]
        [SerializeField] private CrystalSourceConfig[] _crystalSources;
        
        [Header("Firebase 설정")]
        [SerializeField] private string _firebaseCollectionName = "crystal_data";
        [SerializeField] private string _firebaseLogsCollection = "crystal_logs";
        [SerializeField] private bool _enableCloudSync = true;
        
        public int MaxCrystalAmount => _maxCrystalAmount;
        public int DailyFreeCrystalLimit => _dailyFreeCrystalLimit;
        public int StartingFreeCrystal => _startingFreeCrystal;
        public int StartingPaidCrystal => _startingPaidCrystal;
        public CrystalSourceConfig[] CrystalSources => _crystalSources;
        public string FirebaseCollectionName => _firebaseCollectionName;
        public string FirebaseLogsCollection => _firebaseLogsCollection;
        public bool EnableCloudSync => _enableCloudSync;
        
        public CrystalSourceConfig GetSourceConfig(string sourceName)
        {
            foreach (var config in _crystalSources)
            {
                if (config.sourceName == sourceName)
                    return config;
            }
            return null;
        }
    }
    
    [System.Serializable]
    public class CrystalSourceConfig
    {
        [Header("소스 정보")]
        public string sourceName;          // "log에 기록될 이름"
        public string displayName;         // UI에 표시될 이름
        public bool isPaidSource;          // 유료 크리스탈 소스인지
        
        [Header("획득 제한")]
        public int maxDailyGain = 10000;          // 일일 최대 획득량
        public int maxSingleGain = 50;         // 한 번에 최대 획득량
        
        [Header("로그 설정")]
        public bool requiresLogging = false;       // 로그가 필요한지
        public bool requiresValidation = false;    // 서버 검증이 필요한지
    }
}