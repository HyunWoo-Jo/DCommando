using UnityEngine;

namespace Data
{
    public enum InputType {
        None, // 터치가 없는상태
        First, // Down First 프레임
        Push, // 누르고 잇는중
        End, // Up End 프레임
    }
}
