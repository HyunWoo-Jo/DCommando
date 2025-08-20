using UnityEngine;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Core;
namespace Game.Services {
    public interface ISkillDataService {
        UniTask<SO_SkillData> LoadSkillDataAsync(SkillName skillName);
        bool HasSkill(SkillName skillName);
        void UnloadSkill(SkillName skillName);
    }
}
