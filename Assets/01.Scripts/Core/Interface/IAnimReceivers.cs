using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// �ִϸ��̼ǿ��� �߻��ϴ� �̺�Ʈ�� ���޹ޱ� ���� �������̽�
    /// ���� �κ�
    /// </summary>
    public interface IAnimAttackReceiver
    {
        void OnAttackStart(); // ���� ���۽� ȣ��
        void OnAttackEnd(); // ���� ����� ȣ��
    }
}
