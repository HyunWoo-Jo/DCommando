using UnityEngine;

namespace Game.Core {
    /// <summary>
    /// 조건 연산자
    /// </summary>
    public enum ConditionOperator {
        Equal,              // ==
        NotEqual,           // !=
        Greater,            // >
        GreaterOrEqual,     // >=
        Less,               // <
        LessOrEqual,        // <=
        Has,                // 보유 
        NotHas,             // 미보유
    }
}