using UnityEngine;
using Game.Data;
using Game.Core;

namespace Game.Systems {
    public class SkillRangeVisualizer : MonoBehaviour {
        [SerializeField] private SO_SkillData _skillData;
        [SerializeField] private Color _rangeColor = Color.red;

        // 하이라키에서 오브젝트 선택했을 때만 표시

        private void OnDrawGizmos() {
            if (_skillData == null) return;

            Gizmos.color = _rangeColor;

            switch (_skillData.RangeType) {
                case SkillRangeType.Circle:
                DrawCircleRange();
                break;
                case SkillRangeType.Sector:
                DrawSectorRange();
                break;
                case SkillRangeType.Rectangle:
                DrawRectangleRange();
                break;
                case SkillRangeType.Line:
                DrawLineRange();
                break;
            }
        }

        private void DrawCircleRange() {
            Gizmos.DrawWireSphere(transform.position, _skillData.Range);
        }

        private void DrawSectorRange() {
            float halfAngle = _skillData.Width * 0.5f;
            Vector3 forward = transform.up; // 2D에서는 up이 앞쪽

            // 부채꼴 테두리 그리기
            Vector3 leftBound = Quaternion.Euler(0, 0, halfAngle) * forward * _skillData.Range;
            Vector3 rightBound = Quaternion.Euler(0, 0, -halfAngle) * forward * _skillData.Range;

            // 중심에서 경계선까지
            Gizmos.DrawLine(transform.position, transform.position + leftBound);
            Gizmos.DrawLine(transform.position, transform.position + rightBound);

            // 호 그리기 (근사치)
            int segments = 20;
            for (int i = 0; i < segments; i++) {
                float angle1 = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
                float angle2 = Mathf.Lerp(-halfAngle, halfAngle, (float)(i + 1) / segments);

                Vector3 point1 = Quaternion.Euler(0, 0, angle1) * forward * _skillData.Range;
                Vector3 point2 = Quaternion.Euler(0, 0, angle2) * forward * _skillData.Range;

                Gizmos.DrawLine(transform.position + point1, transform.position + point2);
            }
        }

        private void DrawRectangleRange() {
            Vector3 forward = transform.up;
            Vector3 right = transform.right;

            float halfWidth = _skillData.Width * 0.5f;

            // 사각형 네 모서리
            Vector3 topLeft = transform.position + forward * _skillData.Range - right * halfWidth;
            Vector3 topRight = transform.position + forward * _skillData.Range + right * halfWidth;
            Vector3 bottomLeft = transform.position - right * halfWidth;
            Vector3 bottomRight = transform.position + right * halfWidth;

            // 사각형 그리기
            Gizmos.DrawLine(bottomLeft, topLeft);
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
        }

        private void DrawLineRange() {
            Vector3 forward = transform.up;
            Vector3 endPoint = transform.position + forward * _skillData.Range;

            Gizmos.DrawLine(transform.position, endPoint);

            // 선 두께 표현 (선택적)
            if (_skillData.Width > 0) {
                Vector3 right = transform.right;
                float halfWidth = _skillData.Width * 0.5f;

                Vector3 startLeft = transform.position - right * halfWidth;
                Vector3 startRight = transform.position + right * halfWidth;
                Vector3 endLeft = endPoint - right * halfWidth;
                Vector3 endRight = endPoint + right * halfWidth;

                Gizmos.DrawLine(startLeft, endLeft);
                Gizmos.DrawLine(endLeft, endRight);
                Gizmos.DrawLine(endRight, startRight);
                Gizmos.DrawLine(startRight, startLeft);
            }
        }
    }
}