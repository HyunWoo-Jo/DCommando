using UnityEngine;
using UnityEditor;
using Game.Data;
using Game.Core;

[CustomEditor(typeof(SO_SkillData))]
public class SkillDataEditor : Editor {
    private const float PREVIEW_SIZE = 200f;
    private const float GRID_SIZE = 20f; // 1그리드 = 40픽셀
    private const float UNITY_UNITS_PER_GRID = 1f; // 1그리드 = 0.5 Unity 단위

    public override void OnInspectorGUI() {
        SO_SkillData skillData = (SO_SkillData)target;

        // 기본 Inspector 그리기
        DrawDefaultInspector();

        GUILayout.Space(10);

        // 미리보기 라벨
        GUILayout.Label("Range Preview", EditorStyles.boldLabel);

        // 미리보기 영역
        Rect previewRect = GUILayoutUtility.GetRect(PREVIEW_SIZE, PREVIEW_SIZE);
        DrawRangePreview(previewRect, skillData);

        GUILayout.Space(10);

        // 정보 표시
        EditorGUILayout.HelpBox($"Range Type: {skillData.RangeType}\n" +
                               $"Range: {skillData.Range} Unity Units\n" +
                               $"Width/Angle: {skillData.Width}\n" +
                               $"Damage Multiplier: {skillData.DamageMultiplier}x\n" +
                               $"Additional Damage: +{skillData.AdditionalDamage}",
                               MessageType.Info);
    }

    private void DrawRangePreview(Rect rect, SO_SkillData skillData) {
        // 배경
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f, 1f));

        Vector2 center = rect.center;

        // Unity 단위를 픽셀로 변환하는 비율 계산
        float pixelsPerUnityUnit = GRID_SIZE / UNITY_UNITS_PER_GRID; // 80 픽셀 = 1 Unity 단위

        // 격자 그리기
        DrawGrid(rect);

        // 중심점 (캐릭터 위치)
        Rect centerDot = new Rect(center.x - 4, center.y - 4, 8, 8);
        EditorGUI.DrawRect(centerDot, Color.white);

        // 방향 표시 (위쪽이 앞쪽)
        Vector2 directionEnd = center + Vector2.up * 25;
        DrawArrow(center, directionEnd, Color.yellow);

        // 범위 그리기 - Unity 단위를 직접 픽셀로 변환
        DrawSkillRange(rect, center, skillData, pixelsPerUnityUnit);

        // 범위 정보 텍스트
        Rect labelRect = new Rect(rect.x + 5, rect.y + 5, rect.width - 10, 20);
        EditorGUI.LabelField(labelRect, $"{skillData.RangeType} - Range: {skillData.Range:F1} Unity Units",
                           new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.white } });

        // 격자 정보 (우하단)
        Rect scaleRect = new Rect(rect.x + 5, rect.y + rect.height - 25, rect.width - 10, 20);
        EditorGUI.LabelField(scaleRect, $"1 Grid = {UNITY_UNITS_PER_GRID:F1} Unity Units",
                           new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.gray } });
    }

    private void DrawGrid(Rect rect) {
        Color gridColor = new Color(0.25f, 0.25f, 0.25f, 0.6f);

        // 세로선
        for (float x = GRID_SIZE; x < rect.width; x += GRID_SIZE) {
            EditorGUI.DrawRect(new Rect(rect.x + x, rect.y, 1, rect.height), gridColor);
        }

        // 가로선
        for (float y = GRID_SIZE; y < rect.height; y += GRID_SIZE) {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + y, rect.width, 1), gridColor);
        }

        // 중심 십자선 (더 진하게)
        Color centerLineColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        Vector2 center = rect.center;

        // 가로 중심선
        EditorGUI.DrawRect(new Rect(rect.x, center.y - 1, rect.width, 2), centerLineColor);
        // 세로 중심선
        EditorGUI.DrawRect(new Rect(center.x - 1, rect.y, 2, rect.height), centerLineColor);
    }

    private void DrawSkillRange(Rect rect, Vector2 center, SO_SkillData skillData, float pixelsPerUnityUnit) {
        Color rangeColor = new Color(1f, 0.3f, 0.3f, 0.6f);

        switch (skillData.RangeType) {
            case SkillRangeType.Circle:
            DrawCircle(center, skillData.Range * pixelsPerUnityUnit, rangeColor);
            break;
            case SkillRangeType.Sector:
            DrawSector(center, skillData.Range * pixelsPerUnityUnit, skillData.Angle, rangeColor);
            break;
            case SkillRangeType.Rectangle:
            DrawRectangle(center, skillData.Range * pixelsPerUnityUnit, skillData.Width * pixelsPerUnityUnit, rangeColor);
            break;
            case SkillRangeType.Line:
            DrawLine(center, skillData.Range * pixelsPerUnityUnit, skillData.Width * pixelsPerUnityUnit, rangeColor);
            break;
        }
    }

    private void DrawCircle(Vector2 center, float radius, Color color) {
        // 더 조밀한 원 그리기
        int segments = Mathf.Max(30, (int)(radius * 0.3f));

        // 원 채우기 - 더 조밀하게
        for (int ring = 0; ring < radius; ring += 2) {
            for (int i = 0; i < segments; i++) {
                float angle = (float)i / segments * Mathf.PI * 2f;
                Vector2 point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ring;
                EditorGUI.DrawRect(new Rect(point.x - 1, point.y - 1, 2, 2),
                    new Color(color.r, color.g, color.b, color.a * 0.4f));
            }
        }

        // 테두리 (더 두껍게)
        for (int i = 0; i < segments; i++) {
            float angle1 = (float)i / segments * Mathf.PI * 2f;
            float angle2 = (float)(i + 1) / segments * Mathf.PI * 2f;

            Vector2 p1 = center + new Vector2(Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;
            Vector2 p2 = center + new Vector2(Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;

            DrawThickLine(p1, p2, Color.red, 4f);
        }
    }

    private void DrawSector(Vector2 center, float radius, float angle, Color color) {
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;

        // 부채꼴 채우기 - 훨씬 더 조밀하게
        int radialSegments = Mathf.Max(8, (int)(radius * 0.15f));
        int angularSegments = Mathf.Max(8, (int)(angle * 0.3f));

        for (int r = 1; r < radialSegments; r++) {
            float currentRadius = (float)r / radialSegments * radius;
            for (int a = 0; a <= angularSegments; a++) {
                float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, (float)a / angularSegments);
                Vector2 point = center + new Vector2(-Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * currentRadius;

                EditorGUI.DrawRect(new Rect(point.x - 1.5f, point.y - 1.5f, 3f, 3f),
                    new Color(color.r, color.g, color.b, color.a * 0.4f));
            }
        }

        // 추가 중간점들로 더 조밀하게 채우기
        for (int r = 0; r < radialSegments - 1; r++) {
            float currentRadius = ((float)r + 0.5f) / radialSegments * radius;
            for (int a = 0; a < angularSegments; a++) {
                float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, ((float)a + 0.5f) / angularSegments);
                Vector2 point = center + new Vector2(-Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * currentRadius;

                EditorGUI.DrawRect(new Rect(point.x - 1f, point.y - 1f, 2f, 2f),
                    new Color(color.r, color.g, color.b, color.a * 0.2f));
            }
        }

        // 경계선 (더 두껍게)
        Vector2 left = center + new Vector2(-Mathf.Sin(halfAngle), Mathf.Cos(halfAngle)) * radius;
        Vector2 right = center + new Vector2(Mathf.Sin(halfAngle), Mathf.Cos(halfAngle)) * radius;

        DrawThickLine(center, left, Color.red, 4f);
        DrawThickLine(center, right, Color.red, 4f);

        // 호 (더 조밀하게)
        int segments = Mathf.Max(15, (int)(angle * 0.4f));
        for (int i = 0; i < segments; i++) {
            float a1 = Mathf.Lerp(-halfAngle, halfAngle, (float)i / segments);
            float a2 = Mathf.Lerp(-halfAngle, halfAngle, (float)(i + 1) / segments);

            Vector2 p1 = center + new Vector2(-Mathf.Sin(a1), Mathf.Cos(a1)) * radius;
            Vector2 p2 = center + new Vector2(-Mathf.Sin(a2), Mathf.Cos(a2)) * radius;

            DrawThickLine(p1, p2, Color.red, 4f);
        }
    }

    private void DrawRectangle(Vector2 center, float length, float width, Color color) {
        float halfWidth = width * 0.5f;

        // 사각형 채우기
        Rect rangeRect = new Rect(center.x - halfWidth, center.y - length, width, length);
        EditorGUI.DrawRect(rangeRect, color);

        // 테두리 (더 두껍게)
        Vector2[] corners = {
            new Vector2(center.x - halfWidth, center.y),
            new Vector2(center.x - halfWidth, center.y - length),
            new Vector2(center.x + halfWidth, center.y - length),
            new Vector2(center.x + halfWidth, center.y)
        };

        for (int i = 0; i < corners.Length; i++) {
            DrawThickLine(corners[i], corners[(i + 1) % corners.Length], Color.red, 4f);
        }
    }

    private void DrawLine(Vector2 center, float length, float width, Color color) {
        if (width > 0) {
            float halfWidth = width * 0.5f;
            Rect lineRect = new Rect(center.x - halfWidth, center.y - length, width, length);
            EditorGUI.DrawRect(lineRect, color);

            // 테두리 (더 두껍게)
            Vector2[] corners = {
                new Vector2(center.x - halfWidth, center.y),
                new Vector2(center.x - halfWidth, center.y - length),
                new Vector2(center.x + halfWidth, center.y - length),
                new Vector2(center.x + halfWidth, center.y)
            };

            for (int i = 0; i < corners.Length; i++) {
                DrawThickLine(corners[i], corners[(i + 1) % corners.Length], Color.red, 4f);
            }
        } else {
            DrawThickLine(center, new Vector2(center.x, center.y - length), Color.red, 5f);
        }
    }

    private void DrawArrow(Vector2 start, Vector2 end, Color color) {
        DrawThickLine(start, end, color, 2f);

        Vector2 direction = (end - start).normalized;
        Vector2 right = new Vector2(-direction.y, direction.x);

        Vector2 arrowHead1 = end - direction * 8 + right * 5;
        Vector2 arrowHead2 = end - direction * 8 - right * 5;

        DrawThickLine(end, arrowHead1, color, 2f);
        DrawThickLine(end, arrowHead2, color, 2f);
    }

    private void DrawThickLine(Vector2 start, Vector2 end, Color color, float thickness) {
        Vector2 direction = (end - start).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * thickness * 0.5f;

        Vector2[] points = {
            start - perpendicular,
            start + perpendicular,
            end + perpendicular,
            end - perpendicular
        };

        // 더 부드러운 선 그리기
        for (int i = 0; i < points.Length; i++) {
            Vector2 current = points[i];
            Vector2 next = points[(i + 1) % points.Length];

            Vector2 lineDir = next - current;
            int segments = Mathf.Max(2, (int)(lineDir.magnitude * 0.5f));

            for (int j = 0; j < segments; j++) {
                Vector2 point = Vector2.Lerp(current, next, (float)j / segments);
                EditorGUI.DrawRect(new Rect(point.x - 1f, point.y - 1f, 2f, 2f), color);
            }
        }
    }
}