using Cysharp.Threading.Tasks;

namespace Game.Services
{
    public interface IGoldService
    {
        UniTask<int> LoadGoldAsync();
        UniTask SaveGoldAsync(int gold);
    }
}