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
                var skillAddressMap = CSVReader.ReadToDictionary("AddressKey/SkillAddressKey.csv");
                _addressableService.RegisterAddressKeys(skillAddressMap.ToEnumKey<SkillName>());
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