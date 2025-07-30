using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    // 항상 다른 비교자
    public sealed class AlwaysFalseComparer<T> : IEqualityComparer<T> {
        public static readonly AlwaysFalseComparer<T> Instance = new();
        public bool Equals(T x, T y) => false; // 언제나 다르다고 반환
        public int GetHashCode(T obj) => 0;    // 캐시 안 쓸 거라 0이면 충분
    }
}
