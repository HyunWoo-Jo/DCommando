using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// 애니메이션에서 발생하는 이벤트를 전달받기 위한 인터페이스
    /// 공격 부분
    /// </summary>
    public interface IAnimAttackReceiver
    {
        void OnAttackStart(); // 어택 시작시 호출
        void OnAttackEnd(); // 어택 종료시 호출
    }
}
