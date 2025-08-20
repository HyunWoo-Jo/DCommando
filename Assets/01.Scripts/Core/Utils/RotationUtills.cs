using UnityEngine;

namespace Game.Core
{
    public static class RotationUtills
    {
        /// <summary>
        /// 방향 으로 게임에 맞게 회전
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Quaternion ToRotateQuaternion(this Transform tr, Vector2 dir, float speed, float deltaTime) {
            // 회전
            Vector3 currentRot = tr.eulerAngles;
            float targetY = dir.x >= 0 ? 180f : 0f;
            float zFromDir = -Mathf.Atan2(dir.y, Mathf.Abs(dir.x)) * Mathf.Rad2Deg;
            float newZ = Mathf.LerpAngle(currentRot.z, zFromDir, speed * deltaTime);

            return Quaternion.Euler(0, targetY, newZ);
        }
    }
}
