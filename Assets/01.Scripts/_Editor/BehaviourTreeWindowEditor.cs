using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Game.Systems;
using Game.Core;
using Game.Data;
using System.Reflection;
public class BehaviourTreeWindowEditor : EditorWindow {
    private SO_BehaviourTree _targetTree;
    private Vector2 _scrollPosition;
    private BehaviourNodeBase _selectedNode;
    private bool _showNodeDetails = true;

    // 노드 생성용 타입 리스트
    private readonly string[] _compositeTypes = { "Selector", "Sequence", "Parallel" };
    private readonly string[] _decoratorTypes = { "Inverter", "Repeater", "Succeeder", "Failer" };
    private readonly string[] _actionTypes = { "Wait", "Log", "MoveTo", "Attack" };
    private readonly string[] _conditionTypes = { "HasTarget", "IsHealthLow", "IsInRange" };

    [MenuItem("Tools/AI/Behaviour Tree Editor")]
    public static void ShowWindow() {
        var window = GetWindow<BehaviourTreeWindowEditor>("Behaviour Tree Editor");
        window.minSize = new Vector2(600, 400);
    }

    private void OnGUI() {
        DrawToolbar();

        if (_targetTree == null) {
            DrawEmptyState();
            return;
        }

        EditorGUILayout.BeginHorizontal();

        // 왼쪽: 트리 구조
        DrawTreeStructure();

        // 오른쪽: 노드 상세 정보
        if (_showNodeDetails) {
            DrawNodeDetails();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar() {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // BehaviourTree 선택
        EditorGUI.BeginChangeCheck();
        _targetTree = EditorGUILayout.ObjectField(_targetTree, typeof(SO_BehaviourTree), false, GUILayout.Width(200)) as SO_BehaviourTree;
        if (EditorGUI.EndChangeCheck()) {
            _selectedNode = null;
        }

        GUILayout.FlexibleSpace();

        // 도구 버튼들
        if (GUILayout.Button("새 트리", EditorStyles.toolbarButton, GUILayout.Width(60))) {
            CreateNewTree();
        }

        GUI.enabled = _targetTree != null;
        if (GUILayout.Button("저장", EditorStyles.toolbarButton, GUILayout.Width(50))) {
            SaveTree();
        }

        if (GUILayout.Button("초기화", EditorStyles.toolbarButton, GUILayout.Width(50))) {
            if (EditorUtility.DisplayDialog("트리 초기화", "트리를 초기화하시겠습니까?", "예", "아니오")) {
                ResetTree();
            }
        }
        GUI.enabled = true;

        _showNodeDetails = GUILayout.Toggle(_showNodeDetails, "상세", EditorStyles.toolbarButton, GUILayout.Width(40));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawEmptyState() {
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginVertical(GUILayout.Width(300));
        GUILayout.Label("Behaviour Tree를 선택하거나 생성하세요", EditorStyles.centeredGreyMiniLabel);

        if (GUILayout.Button("새 Behaviour Tree 생성", GUILayout.Height(30))) {
            CreateNewTree();
        }

        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    private void DrawTreeStructure() {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(position.width * 0.6f));

        // 헤더
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("트리 구조", EditorStyles.boldLabel);

        GUI.enabled = _targetTree != null && _targetTree.GetRootNode() == null;
        if (GUILayout.Button("+루트", EditorStyles.toolbarButton, GUILayout.Width(50))) {
            ShowAddNodeMenu(null, true);
        }
        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        // 트리 내용
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        if (_targetTree != null && _targetTree.GetRootNode() != null) {
            DrawNode(_targetTree.GetRootNode(), 0);
        } else {
            EditorGUILayout.HelpBox("루트 노드를 추가하세요", MessageType.Info);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawNode(BehaviourNodeBase node, int depth) {
        if (node == null) return;

        EditorGUILayout.BeginHorizontal();

        // 들여쓰기
        GUILayout.Space(depth * 20);

        // 노드 타입 아이콘
        string icon = GetNodeIcon(node);
        GUILayout.Label(icon, GUILayout.Width(20));

        // 노드 선택 버튼
        bool isSelected = _selectedNode == node;
        GUI.backgroundColor = isSelected ? Color.cyan : Color.white;

        if (GUILayout.Button(GetNodeDisplayName(node), EditorStyles.miniButton)) {
            _selectedNode = node;
        }

        GUI.backgroundColor = Color.white;

        // 노드 조작 버튼
        if (node is CompositeNodeBase || node is DecoratorNodeBase) {
            if (GUILayout.Button("+", EditorStyles.miniButton, GUILayout.Width(20))) {
                ShowAddNodeMenu(node, false);
            }
        }

        if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.Width(20))) {
            if (EditorUtility.DisplayDialog("노드 삭제", "이 노드와 하위 노드를 모두 삭제하시겠습니까?", "삭제", "취소")) {
                DeleteNode(node);
            }
        }

        EditorGUILayout.EndHorizontal();

        // 자식 노드 그리기
        if (node is CompositeNodeBase composite) {
            var children = GetChildren(composite);
            foreach (var child in children) {
                DrawNode(child, depth + 1);
            }
        } else if (node is DecoratorNodeBase decorator) {
            var child = GetChild(decorator);
            if (child != null) {
                DrawNode(child, depth + 1);
            }
        }
    }

    private void DrawNodeDetails() {
        EditorGUILayout.BeginVertical("box", GUILayout.Width(position.width * 0.38f));

        // 헤더
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("노드 상세", EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        if (_selectedNode == null) {
            EditorGUILayout.HelpBox("노드를 선택하세요", MessageType.Info);
        } else {
            DrawSelectedNodeDetails();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSelectedNodeDetails() {
        EditorGUILayout.LabelField("타입", _selectedNode.GetType().Name);
        EditorGUILayout.LabelField("상태", _selectedNode.NodeState.ToString());

        EditorGUILayout.Space();
        
        // 노드별 특수 속성 표시
        if (_selectedNode is CompositeNodeBase composite) {
            var children = GetChildren(composite);
            EditorGUILayout.LabelField("자식 노드 수", children.Count.ToString());

            if (children.Count > 0) {
                EditorGUILayout.LabelField("자식 노드:");
                EditorGUI.indentLevel++;
                foreach (var child in children) {
                    if(child == null) {
                        children.Remove(child);
                    }
                    EditorGUILayout.LabelField("- " + GetNodeDisplayName(child));
                }
                EditorGUI.indentLevel--;
            }
        } else if (_selectedNode is DecoratorNodeBase decorator) {
            var child = GetChild(decorator);
            if (child != null) {
                EditorGUILayout.LabelField("자식 노드", GetNodeDisplayName(child));
            } else {
                EditorGUILayout.HelpBox("자식 노드가 없습니다", MessageType.Warning);
            }
        } else if (_selectedNode is ActionNodeBase action) {
            EditorGUILayout.HelpBox("액션 노드 - 실제 동작을 수행합니다", MessageType.None);
        } else if (_selectedNode is ConditionNodeBase condition) {
            EditorGUILayout.HelpBox("조건 노드 - 조건을 확인합니다", MessageType.None);
        }
        DrawDecoratorDetails();

        DrawConditionDetails();

        DrawActionDetails();

        EditorGUILayout.Space();

        // 노드 이동 버튼
        if (_selectedNode != _targetTree.GetRootNode() && _selectedNode.ParentNode is CompositeNodeBase) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("위로 이동")) {
                MoveNodeUp(_selectedNode);
            }
            if (GUILayout.Button("아래로 이동")) {
                MoveNodeDown(_selectedNode);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    private void DrawDecoratorDetails() {
        if (_selectedNode is Cooldown cooldown) {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            var coolTime = EditorGUILayout.FloatField("쿨 타임", cooldown.CoolTime);
            if (EditorGUI.EndChangeCheck()) {
                cooldown.SetCoolTime(coolTime);
            }
        }

    }


    private void DrawConditionDetails() {
        if (_selectedNode is IsPlayerInRange isPlayerRange) {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            var range = EditorGUILayout.FloatField("범위", isPlayerRange.Range);
            if (EditorGUI.EndChangeCheck()) {
                isPlayerRange.SetRange(range);
            }
        }
    }

    private void DrawActionDetails() {
        // ChasePlayerAction 노드 처리
        if (_selectedNode is ChasePlayerAction chaseAction) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Chase Settings", EditorStyles.boldLabel);

            // struct는 값 타입이므로 복사본을 가져와서 수정 후 다시 설정
            var moveData = chaseAction.MoveData;

            EditorGUI.BeginChangeCheck();

            // struct의 각 필드 편집
            moveData.moveSpeed = EditorGUILayout.FloatField("이동 속도", moveData.moveSpeed);
            moveData.rotationSpeed = EditorGUILayout.FloatField("회전 속도", moveData.rotationSpeed);
            // CharacterMoveData의 다른 필드들도 추가...

            if (EditorGUI.EndChangeCheck()) {
                // 수정된 struct를 다시 설정
                chaseAction.SetMoveData(moveData);
                EditorUtility.SetDirty(_targetTree);
            }
        }
        if (_selectedNode is DelayAction delayAction) {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            var delay = EditorGUILayout.FloatField("시간", delayAction.TargetDelay);
            if (EditorGUI.EndChangeCheck()) {
                delayAction.SetTargetDelay(delay);
            }
        }
        if(_selectedNode is DamageToPlayerAction damageToPlayer) {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            var type = (DamageType)EditorGUILayout.EnumPopup("시간", damageToPlayer.DamageType);
            if (EditorGUI.EndChangeCheck()) {
                damageToPlayer.SetDamageType(type);
            }
        }
    }


    private void ShowAddNodeMenu(BehaviourNodeBase parent, bool isRoot) {
        GenericMenu menu = new GenericMenu();

        if (isRoot || parent is CompositeNodeBase) {
            AddAllTime(menu, parent, isRoot);

        } else if (parent is DecoratorNodeBase decorator) {
            var child = GetChild(decorator);
            if (child == null) {
                // Decorator는 자식이 하나만 가능
                AddAllTime(menu, parent, isRoot);
            } else {
                menu.AddDisabledItem(new GUIContent("Decorator는 자식을 하나만 가질 수 있습니다"));
            }
        }

        menu.ShowAsContext();
    }

    private void AddAllTime(GenericMenu menu, BehaviourNodeBase parent, bool isRoot) {
        // Composite 노드 추가
        menu.AddItem(new GUIContent("Composite/Selector"), false, () => AddNode(parent, new Selector(), isRoot));
        menu.AddItem(new GUIContent("Composite/Sequence"), false, () => AddNode(parent, new Sequence(), isRoot));
        menu.AddItem(new GUIContent("Composite/Parallel"), false, () => AddNode(parent, new Parallel(), isRoot));

        menu.AddSeparator("");

        // Decorator 노드 추가
        menu.AddItem(new GUIContent("Decorator/Inverter"), false, () => AddNode(parent, new Inverter(), isRoot));
        menu.AddItem(new GUIContent("Decorator/Repeater"), false, () => AddNode(parent, new Repeater(), isRoot));
        menu.AddItem(new GUIContent("Decorator/Cooldown"), false, () => AddNode(parent, new Cooldown(1), isRoot));

        menu.AddSeparator("");
        // Condition 노드 추가
        menu.AddItem(new GUIContent("Condition/IsPlayerInRange"), false, () => AddNode(parent, new IsPlayerInRange(1), isRoot));
        menu.AddItem(new GUIContent("Condition/IsPlayerDead"), false, () => AddNode(parent, new IsPlayerDead(), isRoot));


        menu.AddSeparator("");
        // Action 노드 추가
        menu.AddItem(new GUIContent("Action/ChasePlayer"), false, () => AddNode(parent, new ChasePlayerAction(new CharacterMoveData()), isRoot));
        menu.AddItem(new GUIContent("Action/DamageToPlayer"), false, () => AddNode(parent, new DamageToPlayerAction(), isRoot));
        menu.AddItem(new GUIContent("Action/Delay"), false, () => AddNode(parent, new DelayAction(1), isRoot));

        menu.AddSeparator("");
    }

    private void AddNode(BehaviourNodeBase parent, BehaviourNodeBase newNode, bool isRoot) {
        if (isRoot) {
            _targetTree.SetRootNode(newNode);
        } else if (parent is CompositeNodeBase composite) {
            composite.AddChild(newNode);
        } else if (parent is DecoratorNodeBase decorator) {
            decorator.SetChild(newNode);
        }
        if (!isRoot) {
            newNode.SetParentNode(parent);
        }
        EditorUtility.SetDirty(_targetTree);
    }

    private void DeleteNode(BehaviourNodeBase node) {
        if (node == _targetTree.GetRootNode()) {
            _targetTree.SetRootNode(null);
        } else {
            // 부모 노드 찾아서 제거
            RemoveNodeFromParent(node, _targetTree.GetRootNode());
        }

        if (_selectedNode == node) {
            _selectedNode = null;
        }

        EditorUtility.SetDirty(_targetTree);
    }

    private bool RemoveNodeFromParent(BehaviourNodeBase nodeToRemove, BehaviourNodeBase currentNode) {
        if (currentNode is CompositeNodeBase composite) {
            var children = GetChildren(composite);
            if (children.Contains(nodeToRemove)) {
                composite.RemoveChild(nodeToRemove);
                return true;
            }

            foreach (var child in children) {
                if (RemoveNodeFromParent(nodeToRemove, child))
                    return true;
            }
        } else if (currentNode is DecoratorNodeBase decorator) {
            var child = GetChild(decorator);
            if (child == nodeToRemove) {
                decorator.SetChild(null);
                return true;
            }

            if (child != null) {
                return RemoveNodeFromParent(nodeToRemove, child);
            }
        }

        return false;
    }

    private void MoveNodeUp(BehaviourNodeBase node) {
        if (node.ParentNode == null) {
            GameDebug.Log("루트 노드는 이동할 수 없습니다");
            return;
        }

        var parent = node.ParentNode;

        if (parent is CompositeNodeBase composite) {
             var children = GetChildren(composite);

            int currentIndex = children.IndexOf(node);
            if (currentIndex <= 0) {
                GameDebug.Log("이미 가장 위에 있는 노드입니다");
                return;
            }
            // 위치 교체
            children.RemoveAt(currentIndex);
            children.Insert(currentIndex - 1, node); 
        }
    }

    private void MoveNodeDown(BehaviourNodeBase node) {
        if (node.ParentNode == null) {
            GameDebug.Log("루트 노드는 이동할 수 없습니다");
            return;
        }

        var parent = node.ParentNode;

        if (parent is CompositeNodeBase composite) {
            var children = GetChildren(composite);

            int currentIndex = children.IndexOf(node);
            if (currentIndex >= children.Count - 1) {
                GameDebug.Log("이미 가장 아래에 있는 노드입니다");
                return;
            }
            children.RemoveAt(currentIndex);
            children.Insert(currentIndex + 1, node);
        }
    }
    private void CreateNewTree() {
        string path = EditorUtility.SaveFilePanelInProject("새 Behaviour Tree", "NewBehaviourTree", "asset", "Behaviour Tree 저장 위치를 선택하세요");

        if (!string.IsNullOrEmpty(path)) {
            SO_BehaviourTree newTree = ScriptableObject.CreateInstance<SO_BehaviourTree>();
            AssetDatabase.CreateAsset(newTree, path);
            AssetDatabase.SaveAssets();

            _targetTree = newTree;
            _selectedNode = null;
        }
    }

    private void SaveTree() {
        if (_targetTree != null) {
            EditorUtility.SetDirty(_targetTree);
            AssetDatabase.SaveAssets();
            GameDebug.Log("Behaviour Tree 저장 완료");
        }
    }

    private void ResetTree() {
        if (_targetTree != null) {
            _targetTree.Reset();
            _selectedNode = null;
            EditorUtility.SetDirty(_targetTree);
        }
    }

    private string GetNodeIcon(BehaviourNodeBase node) {
        if (node is CompositeNodeBase) return "◆";
        if (node is DecoratorNodeBase) return "◇";
        if (node is ActionNodeBase) return "▶";
        if (node is ConditionNodeBase) return "?";
        return "●";
    }

    private string GetNodeDisplayName(BehaviourNodeBase node) {
        string typeName = node.GetType().Name;

        // Base 제거
        if (typeName.EndsWith("Node"))
            typeName = typeName.Substring(0, typeName.Length - 4);
        if (typeName.EndsWith("Base"))
            typeName = typeName.Substring(0, typeName.Length - 4);

        return typeName;
    }

    private List<BehaviourNodeBase> GetChildren(CompositeNodeBase composite) {
        // 리플렉션으로 _children 필드 접근
        var field = typeof(CompositeNodeBase).GetField("_children",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null) {
            return field.GetValue(composite) as List<BehaviourNodeBase> ?? new List<BehaviourNodeBase>();
        }

        return new List<BehaviourNodeBase>();
    }

    private BehaviourNodeBase GetChild(DecoratorNodeBase decorator) {
        // 리플렉션으로 _child 필드 접근
        var field = typeof(DecoratorNodeBase).GetField("_child",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (field != null) {
            return field.GetValue(decorator) as BehaviourNodeBase;
        }

        return null;
    }


}