using System.Collections.Generic;
using UnityEngine;

namespace CustomUtilis
{
    // �׻� �ٸ� ����
    public sealed class AlwaysFalseComparer<T> : IEqualityComparer<T> {
        public static readonly AlwaysFalseComparer<T> Instance = new();
        public bool Equals(T x, T y) => false; // ������ �ٸ��ٰ� ��ȯ
        public int GetHashCode(T obj) => 0;    // ĳ�� �� �� �Ŷ� 0�̸� ���
    }
}
