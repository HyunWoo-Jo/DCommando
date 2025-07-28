using Cysharp.Threading.Tasks;

namespace Game.Services
{
    public interface IGoldService
    {
        UniTask CheckGoldAsync(int gold);
    }
}