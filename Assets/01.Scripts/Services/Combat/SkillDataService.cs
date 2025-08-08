using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;
using Game.Core;
using Game.Data;

namespace Game.Services {
    public class SkillDataService : ISkillDataService, IInitializable {
        [Inject] private IAddressableService<SkillName, SO_SkillData> _addressableService;

        #region 초기화
        public void Initialize() {
            try {
                var skillAddressMap = CSVReader.ReadToDictionary("AddressKey/SkillAddressKey");

                // SkillName으로 변환하여 등록
                var convertedMap = new Dictionary<SkillName, string>();
                foreach (var kvp in skillAddressMap) {
                    if (System.Enum.TryParse<SkillName>(kvp.Key, out var skillName)) {
                        convertedMap[skillName] = kvp.Value;
                    }
                }

                _addressableService.RegisterAddressKeys(convertedMap);
                GameDebug.Log($"스킬 주소 키 {convertedMap.Count}개 등록 완료");
            } catch {
                GameDebug.LogError("Skill CSV 파일 로드에 실패했습니다.");
            }
        }
        #endregion

        public async UniTask<SO_SkillData> LoadSkillDataAsync(SkillName skillName) {
            var skillData = await _addressableService.LoadAssetAsync(skillName);

            if (skillData != null) {
                GameDebug.Log($"스킬 데이터 로드 완료: {skillName}");
            } else {
                GameDebug.LogError($"스킬 데이터 로드 실패: {skillName}");
            }

            return skillData;
        }

        public bool HasSkill(SkillName skillName) {
            return _addressableService.HasAddressKey(skillName);
        }

        public void UnloadSkill(SkillName skillName) {
            _addressableService.UnloadAsset(skillName);
            GameDebug.Log($"스킬 데이터 언로드: {skillName}");
        }

        public SO_SkillData GetLoadedSkillData(SkillName skillName) {
            return _addressableService.GetLoadedAsset(skillName);
        }
    }
}