using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Game.Systems;
using Game.Core;
[CustomEditor(typeof(SO_BehaviourTree))]
public class SO_BehaviourTreeInspector : UnityEditor.Editor {
    private SO_BehaviourTree _tree;
    private bool _showTreeStructure = true;
    private Dictionary<BehaviourNodeBase, bool> _foldoutStates = new Dictionary<BehaviourNodeBase, bool>();

    private GUIStyle _headerStyle;
    private GUIStyle _nodeBoxStyle;
    private GUIStyle _runningStyle;
    private GUIStyle _successStyle;
    private GUIStyle _failureStyle;

    private void OnEnable() {
        _tree = target as SO_BehaviourTree;
        InitializeStyles();
    }

    private void InitializeStyles() {
        _headerStyle = new GUIStyle(EditorStyles.boldLabel) {
            fontSize = 12,
            alignment = TextAnchor.MiddleLeft
        };
    }

    public override void OnInspectorGUI() {
        // 스타일 초기화 (GUI 컨텍스트 내에서)
        if (_nodeBoxStyle == null) {
            _nodeBoxStyle = new GUIStyle(GUI.skin.box) {
                padding = new RectOffset(8, 8, 4, 4),
                margin = new RectOffset(0, 0, 2, 2)
            };

            _runningStyle = new GUIStyle(EditorStyles.label) {
                normal = { textColor = Color.yellow }
            };

            _successStyle = new GUIStyle(EditorStyles.label) {
                normal = { textColor = Color.green }
            };

            _failureStyle = new GUIStyle(EditorStyles.label) {
                normal = { textColor = Color.red }
            };
        }

        serializedObject.Update();

        // 기본 정보
        EditorGUILayout.Space();
        DrawInspectorHeader();

        EditorGUILayout.Space(10);

        // 트리 구조 표시
        _showTreeStructure = EditorGUILayout.BeginFoldoutHeaderGroup(_showTreeStructure, "트리 구조");
        if (_showTreeStructure) {
            DrawTreeStructure();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // 액션 버튼들
        DrawActionButtons();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawInspectorHeader() {
        EditorGUILayout.BeginVertical(_nodeBoxStyle);

        // 루트 노드 상태
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("루트 노드", GUILayout.Width(60));

        if (_tree.GetRootNode() != null) {
            GUI.enabled = false;
            EditorGUILayout.LabelField(GetNodeTypeName(_tree.GetRootNode()), EditorStyles.boldLabel);
            GUI.enabled = true;
        } else {
            EditorGUILayout.LabelField("없음", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawTreeStructure() {
        EditorGUILayout.BeginVertical(_nodeBoxStyle);

        if (_tree.GetRootNode() != null) {
            DrawNodeHierarchy(_tree.GetRootNode(), 0);
        } else {
            EditorGUILayout.HelpBox("루트 노드가 설정되지 않았습니다.\nBehaviour Tree Editor에서 노드를 추가하세요.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawNodeHierarchy(BehaviourNodeBase node, int depth) {
        if (node == null) return;

        EditorGUILayout.BeginHorizontal();

        // 들여쓰기
        GUILayout.Space(depth * 15);

        // 노드가 자식을 가질 수 있는 경우 foldout 표시
        bool hasChildren = false;
        List<BehaviourNodeBase> children = null;

        if (node is CompositeNodeBase composite) {
            children = GetCompositeChildren(composite);
            hasChildren = children != null && children.Count > 0;
        } else if (node is DecoratorNodeBase decorator) {
            var child = GetDecoratorChild(decorator);
            if (child != null) {
                children = new List<BehaviourNodeBase> { child };
                hasChildren = true;
            }
        }

        // Foldout 또는 아이콘
        if (hasChildren) {
            if (!_foldoutStates.ContainsKey(node))
                _foldoutStates[node] = true;

            _foldoutStates[node] = EditorGUILayout.Foldout(_foldoutStates[node], "");
        } else {
            GUILayout.Space(15);
        }

        // 노드 타입 아이콘
        string icon = GetNodeIcon(node);
        GUILayout.Label(icon, GUILayout.Width(20));

        // 노드 이름
        string nodeName = GetNodeTypeName(node);
        EditorGUILayout.LabelField(nodeName, GUILayout.MinWidth(100));

        // 노드 상태 표시
        GUIStyle stateStyle = GetStateStyle(node.NodeState);
        EditorGUILayout.LabelField($"[{node.NodeState}]", stateStyle, GUILayout.Width(70));

        // 노드 타입 표시
        string typeLabel = GetNodeTypeLabel(node);
        EditorGUILayout.LabelField(typeLabel, EditorStyles.miniLabel, GUILayout.Width(80));

        EditorGUILayout.EndHorizontal();

        // 자식 노드 그리기
        if (hasChildren && _foldoutStates.ContainsKey(node) && _foldoutStates[node]) {
            if (children != null) {
                foreach (var child in children) {
                    DrawNodeHierarchy(child, depth + 1);
                }
            }
        }
    }

    private void DrawActionButtons() {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("에디터 열기", GUILayout.Height(30))) {
            BehaviourTreeWindowEditor.ShowWindow();
            var window = EditorWindow.GetWindow<BehaviourTreeWindowEditor>();

            // 현재 트리를 에디터에 설정
            var treeField = typeof(BehaviourTreeWindowEditor).GetField("_targetTree",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (treeField != null) {
                treeField.SetValue(window, _tree);
            }

            window.Repaint();
        }

        GUI.enabled = _tree.GetRootNode() != null;

        if (GUILayout.Button("트리 초기화", GUILayout.Height(30))) {
            if (EditorUtility.DisplayDialog("트리 초기화",
                "모든 노드 상태를 초기화하시겠습니까?", "초기화", "취소")) {
                _tree.Reset();
                EditorUtility.SetDirty(_tree);
            }
        }

        if (GUILayout.Button("트리 실행", GUILayout.Height(30))) {
            var result = _tree.Update();
            GameDebug.Log($"BehaviourTree 실행 결과: {result}");
            Repaint();
        }

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 디버그 옵션
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("모두 펼치기")) {
            SetAllFoldouts(true);
        }

        if (GUILayout.Button("모두 접기")) {
            SetAllFoldouts(false);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void SetAllFoldouts(bool value) {
        _foldoutStates.Clear();
        if (value && _tree.GetRootNode() != null) {
            AddAllNodesToFoldout(_tree.GetRootNode(), value);
        }
        Repaint();
    }

    private void AddAllNodesToFoldout(BehaviourNodeBase node, bool value) {
        if (node == null) return;

        _foldoutStates[node] = value;

        if (node is CompositeNodeBase composite) {
            var children = GetCompositeChildren(composite);
            foreach (var child in children) {
                AddAllNodesToFoldout(child, value);
            }
        } else if (node is DecoratorNodeBase decorator) {
            var child = GetDecoratorChild(decorator);
            if (child != null) {
                AddAllNodesToFoldout(child, value);
            }
        }
    }

    private string GetNodeIcon(BehaviourNodeBase node) {
        if (node is CompositeNodeBase) return "◆";
        if (node is DecoratorNodeBase) return "◇";
        if (node is ActionNodeBase) return "▶";
        if (node is ConditionNodeBase) return "?";
        return "●";
    }

    private string GetNodeTypeName(BehaviourNodeBase node) {
        string typeName = node.GetType().Name;

        // Node 접미사 제거
        if (typeName.EndsWith("Node"))
            typeName = typeName.Substring(0, typeName.Length - 4);
        if (typeName.EndsWith("Base"))
            typeName = typeName.Substring(0, typeName.Length - 4);

        return typeName;
    }

    private string GetNodeTypeLabel(BehaviourNodeBase node) {
        if (node is CompositeNodeBase) return "[Composite]";
        if (node is DecoratorNodeBase) return "[Decorator]";
        if (node is ActionNodeBase) return "[Action]";
        if (node is ConditionNodeBase) return "[Condition]";
        return "[Unknown]";
    }

    private GUIStyle GetStateStyle(NodeState state) {
        switch (state) {
            case NodeState.Running:
            return _runningStyle ?? EditorStyles.label;
            case NodeState.Success:
            return _successStyle ?? EditorStyles.label;
            case NodeState.Failure:
            return _failureStyle ?? EditorStyles.label;
            default:
            return EditorStyles.label;
        }
    }

    private List<BehaviourNodeBase> GetCompositeChildren(CompositeNodeBase composite) {
        var field = typeof(CompositeNodeBase).GetField("_children",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null) {
            return field.GetValue(composite) as List<BehaviourNodeBase> ?? new List<BehaviourNodeBase>();
        }

        return new List<BehaviourNodeBase>();
    }

    private BehaviourNodeBase GetDecoratorChild(DecoratorNodeBase decorator) {
        var field = typeof(DecoratorNodeBase).GetField("_child",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null) {
            return field.GetValue(decorator) as BehaviourNodeBase;
        }

        return null;
    }

}