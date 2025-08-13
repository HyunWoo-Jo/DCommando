using UnityEngine;
using UnityEditor;
using Game.Data;
using Game.Core;
using System.Linq;
using System.IO;

public class StageConfigWindowEditor : EditorWindow {
    private SO_StageConfig _currentConfig;
    private Vector2 _scrollPosition;
    private Vector2 _stageScrollPosition;

    // Tab 관리
    private int _selectedTab = 0;
    private readonly string[] _nonSelectedTabNames = { "Config 관리" };
    private readonly string[] _tabNames = { "Config 관리", "Stage 편집", "Enemy 배치" };

    // Stage 관리
    private bool[] _stageExpanded;
    private int _selectedStageIndex = -1;

    // Enemy 배치용
    private Vector2 _enemyMapScrollPosition;
    private float _mapZoom = 1.0f;
    private Vector2 _mapOffset = Vector2.zero;
    private bool _isDragging = false;
    private Vector2 _lastMousePosition;

    // 생성 설정
    private string _newConfigName = "NewStageConfig";
    private string _configSavePath = "Assets/04.Datas/StageConfigs/";


    // Enemy 생성용 
    private EnemyName _selectedEnemyType = EnemyName.WormBlack;
    private int _selectedEnemyHealth = 100;
    private int _selectedEnemyPower = 10;
    private int _selectedEnemyExpReward = 10;
    private int _selectedEnemyGoldReward = 5;

    [MenuItem("Tools/Stage Config Manager")]
    public static void ShowWindow() {
        StageConfigWindowEditor window = GetWindow<StageConfigWindowEditor>("Stage Config Manager");
        window.minSize = new Vector2(800, 600);
        window.Show();
    }

    private void OnGUI() {
        DrawHeader();
        DrawTabs();

        EditorGUILayout.Space(10);

        switch (_selectedTab) {
            case 0: DrawConfigManagementTab(); break;
            case 1: DrawStageEditTab(); break;
            case 2: DrawEnemyPlacementTab(); break;
        }
    }

    private void DrawHeader() {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Stage Config Manager", EditorStyles.boldLabel);

        if (_currentConfig != null) {
            EditorGUILayout.LabelField($"현재 Config: {_currentConfig.name}", EditorStyles.miniLabel);
        } else {
            EditorGUILayout.LabelField("Config가 로드되지 않음", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawTabs() {

        var names = _currentConfig == null ? _nonSelectedTabNames : _tabNames;

        _selectedTab = GUILayout.Toolbar(_selectedTab, names , GUILayout.Width(150 * names.Length));
    }

    #region Config 관리 Tab
    private void DrawConfigManagementTab() {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Config 파일 관리", EditorStyles.boldLabel);

        // 현재 Config 표시
        DrawCurrentConfigSection();

        EditorGUILayout.Space(10);

        // 새 Config 생성
        DrawSection();


        EditorGUILayout.EndVertical();
    }

    private void DrawCurrentConfigSection() {
        EditorGUILayout.LabelField("현재 Config", EditorStyles.boldLabel);

        SO_StageConfig newConfig = (SO_StageConfig)EditorGUILayout.ObjectField("Stage Config", _currentConfig, typeof(SO_StageConfig), false);

        if (newConfig != _currentConfig) {
            _currentConfig = newConfig;
            RefreshStageData();
        }

        if (_currentConfig != null) {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Config")) {
                SaveCurrentConfig();
            }

            if (GUILayout.Button("Reload Config")) {
                ReloadCurrentConfig();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawSection() {
        EditorGUILayout.LabelField("새 Config 생성", EditorStyles.boldLabel);

        _newConfigName = EditorGUILayout.TextField("Config 이름", _newConfigName);
        _configSavePath = EditorGUILayout.TextField("저장 경로", _configSavePath);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Browse", GUILayout.Width(80))) {
            string selectedPath = EditorUtility.OpenFolderPanel("Config 저장 폴더 선택", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath)) {
                _configSavePath = "Assets" + selectedPath.Substring(Application.dataPath.Length) + "/";
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("생성", GUILayout.Height(30), GUILayout.Width(100))) {
            CreateNewStageConfig();
        }

        if (GUILayout.Button("로드", GUILayout.Height(30), GUILayout.Width(100))) {
            string configPath = EditorUtility.OpenFilePanel("Stage Config 선택", "Assets", "asset");
            if (!string.IsNullOrEmpty(configPath)) {
                string relativePath = "Assets" + configPath.Substring(Application.dataPath.Length);
                SO_StageConfig config = AssetDatabase.LoadAssetAtPath<SO_StageConfig>(relativePath);
                if (config != null) {
                    _currentConfig = config;
                    RefreshStageData();
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        // 최근 Config 목록 표시
        DrawRecentConfigs();
    }

    private void DrawRecentConfigs() {
        EditorGUILayout.LabelField("최근 Config 목록", EditorStyles.boldLabel);

        string[] configGuids = AssetDatabase.FindAssets("t:SO_StageConfig");

        if (configGuids.Length == 0) {
            EditorGUILayout.HelpBox("SO_StageConfig 파일이 없습니다.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginVertical("box");

        foreach (string guid in configGuids.Take(5)) // 최근 5개만 표시
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            SO_StageConfig config = AssetDatabase.LoadAssetAtPath<SO_StageConfig>(path);

            if (config != null) {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(config.name, GUILayout.Width(200));
                EditorGUILayout.LabelField($"({config.stages?.Length ?? 0} stages)", GUILayout.Width(100));

                if (GUILayout.Button("Load", GUILayout.Width(60))) {
                    _currentConfig = config;
                    RefreshStageData();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();
    }
    #endregion

    #region Stage 편집 Tab
    private void DrawStageEditTab() {
        if (_currentConfig == null) {
            EditorGUILayout.HelpBox("Config를 먼저 로드하세요.", MessageType.Warning);
            return;
        }

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Stage 편집", EditorStyles.boldLabel);

        // Global Settings
        DrawGlobalSettings();

        EditorGUILayout.Space(10);

        // Stage List
        DrawStageList();

        // Control Buttons
        DrawStageControlButtons();

        EditorGUILayout.EndVertical();
    }

    private void DrawGlobalSettings() {
        EditorGUILayout.LabelField("Global Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");

        // Stage 크기 설정
        EditorGUILayout.LabelField("Stage 크기", EditorStyles.boldLabel);
        _currentConfig.stageSize = EditorGUILayout.RectField("Stage Size", _currentConfig.stageSize , GUILayout.Width(300));

        EditorGUILayout.Space(5);

        // 크기 정보 표시
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Width: {_currentConfig.stageSize.width}", GUILayout.Width(100));
        EditorGUILayout.LabelField($"Height: {_currentConfig.stageSize.height}", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        EditorGUILayout.EndVertical();
    }

    private void DrawStageList() {
        if (_currentConfig.stages == null || _currentConfig.stages.Length == 0) {
            EditorGUILayout.HelpBox("Stage가 없습니다. 'Add Stage' 버튼을 눌러 추가하세요.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField($"Stage 목록 ({_currentConfig.stages.Length}개)", EditorStyles.boldLabel);

        _stageScrollPosition = EditorGUILayout.BeginScrollView(_stageScrollPosition, GUILayout.MaxHeight(300));

        for (int i = 0; i < _currentConfig.stages.Length; i++) {
            DrawStageListElement(i);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawStageListElement(int index) {
        StageData stage = _currentConfig.stages[index];

        EditorGUILayout.BeginVertical("box");

        // Stage 헤더
        EditorGUILayout.BeginHorizontal();

        bool isSelected = _selectedStageIndex == index;
        bool newSelected = EditorGUILayout.Toggle(isSelected, GUILayout.Width(20));

        if (newSelected != isSelected) {
            _selectedStageIndex = newSelected ? index : -1;
        }

        string stageTitle = $"Stage {stage.stageId}: {stage.stageName}";
        EditorGUILayout.LabelField(stageTitle, isSelected ? EditorStyles.boldLabel : EditorStyles.label);

        EditorGUILayout.LabelField($"({stage.enemyDatas?.Length ?? 0} enemies)", GUILayout.Width(100));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Edit", GUILayout.Width(50))) {
            _selectedStageIndex = index;
            _selectedTab = 2; // Enemy 배치 탭으로 이동
        }

        if (GUILayout.Button("X", GUILayout.Width(25))) {
            if (EditorUtility.DisplayDialog("Stage 삭제", $"Stage {stage.stageId}를 삭제하시겠습니까?", "삭제", "취소")) {
                RemoveStage(index);
            }
        }

        EditorGUILayout.EndHorizontal();

        // 선택된 Stage의 간단한 정보 표시
        if (isSelected) {
            EditorGUI.indentLevel++;
            DrawStageQuickEdit(index);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawStageQuickEdit(int index) {
        StageData stage = _currentConfig.stages[index];

        stage.stageId = EditorGUILayout.IntField("Stage ID", stage.stageId);
        stage.stageName = EditorGUILayout.TextField("Stage 이름", stage.stageName);
        stage.description = EditorGUILayout.TextArea(stage.description, GUILayout.Height(30));

        EditorGUILayout.BeginHorizontal();
        stage.goldReward = EditorGUILayout.IntField("Gold", stage.goldReward, GUILayout.Width(100));
        stage.expReward = EditorGUILayout.IntField("Exp", stage.expReward, GUILayout.Width(100));
        stage.autoStartNextStage = EditorGUILayout.Toggle("Auto Next", stage.autoStartNextStage, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        _currentConfig.stages[index] = stage;
    }

    private void DrawStageControlButtons() {
        EditorGUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Stage", GUILayout.Height(30))) {
            AddNewStage();
        }

        if (GUILayout.Button("Clear All", GUILayout.Height(30))) {
            if (EditorUtility.DisplayDialog("모든 Stage 삭제", "모든 Stage를 삭제하시겠습니까?", "삭제", "취소")) {
                ClearAllStages();
            }
        }

        if (GUILayout.Button("Sort by ID", GUILayout.Height(30))) {
            SortStagesByID();
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Enemy 배치 Tab
    private void DrawEnemyPlacementTab() {
        if (_currentConfig == null) {
            EditorGUILayout.HelpBox("Config를 먼저 로드하세요.", MessageType.Warning);
            return;
        }

        if (_selectedStageIndex < 0 || _selectedStageIndex >= _currentConfig.stages.Length) {
            EditorGUILayout.HelpBox("Stage를 먼저 선택하세요.", MessageType.Warning);
            DrawStageSelector();
            return;
        }

        EditorGUILayout.BeginVertical("box");

        StageData currentStage = _currentConfig.stages[_selectedStageIndex];
        EditorGUILayout.LabelField($"Enemy 배치 - Stage {currentStage.stageId}: {currentStage.stageName}", EditorStyles.boldLabel);

        DrawEnemyCreationSettings();

        // 맵 컨트롤
        DrawMapControls();

        EditorGUILayout.Space(5);

        // Enemy 배치 맵
        DrawEnemyPlacementMap(currentStage);

        EditorGUILayout.Space(10);

        // Enemy 리스트
        DrawEnemyList(currentStage);

        EditorGUILayout.EndVertical();
    }
    private void DrawEnemyCreationSettings() {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("새 Enemy 생성 설정", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        // Enemy 타입 선택 (색상 표시 포함)
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Enemy Type", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();

        // 선택된 Enemy 색상 표시
        Color selectedColor = GetEnemyColor(_selectedEnemyType);
        Rect colorRect = GUILayoutUtility.GetRect(25, 25, GUILayout.Width(25), GUILayout.Height(25));
        EditorGUI.DrawRect(colorRect, selectedColor);

        // 테두리
        EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.y, colorRect.width, 1), Color.black);
        EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.y + colorRect.height - 1, colorRect.width, 1), Color.black);
        EditorGUI.DrawRect(new Rect(colorRect.x, colorRect.y, 1, colorRect.height), Color.black);
        EditorGUI.DrawRect(new Rect(colorRect.x + colorRect.width - 1, colorRect.y, 1, colorRect.height), Color.black);

        _selectedEnemyType = (EnemyName)EditorGUILayout.EnumPopup(_selectedEnemyType, GUILayout.Width(120));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // 스탯 설정
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Stats", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("HP:", GUILayout.Width(25));
        _selectedEnemyHealth = EditorGUILayout.IntField(_selectedEnemyHealth, GUILayout.Width(60));

        EditorGUILayout.LabelField("ATK:", GUILayout.Width(30));
        _selectedEnemyPower = EditorGUILayout.IntField(_selectedEnemyPower, GUILayout.Width(60));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);

        // 보상 설정
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Rewards", EditorStyles.miniLabel);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Exp:", GUILayout.Width(30));
        _selectedEnemyExpReward = EditorGUILayout.IntField(_selectedEnemyExpReward, GUILayout.Width(50));

        EditorGUILayout.LabelField("Gold:", GUILayout.Width(35));
        _selectedEnemyGoldReward = EditorGUILayout.IntField(_selectedEnemyGoldReward, GUILayout.Width(50));

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();

        // 사용법 안내
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("좌클릭: 설정된 Enemy 추가 | 우클릭: Enemy 삭제", MessageType.Info);

        EditorGUILayout.EndVertical();
    }

    private void DrawStageSelector() {
        EditorGUILayout.LabelField("Stage 선택", EditorStyles.boldLabel);

        if (_currentConfig.stages != null) {
            string[] stageOptions = _currentConfig.stages.Select((s, i) => $"Stage {s.stageId}: {s.stageName}").ToArray();
            int newIndex = EditorGUILayout.Popup("Stage", _selectedStageIndex, stageOptions);

            if (newIndex != _selectedStageIndex) {
                _selectedStageIndex = newIndex;
            }
        }
    }

    private void DrawMapControls() {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Map Controls", GUILayout.Width(100));

        if (GUILayout.Button("Reset View", GUILayout.Width(80))) {
            _mapZoom = 1.0f;
            _mapOffset = Vector2.zero;
        }

        EditorGUILayout.LabelField("Zoom:", GUILayout.Width(50));
        _mapZoom = EditorGUILayout.Slider(_mapZoom, 0.5f, 3.0f, GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
    }

    private void DrawEnemyPlacementMap(StageData stage) {
        float mapSize = 400f;
        Rect mapRect = GUILayoutUtility.GetRect(mapSize, mapSize);

        // 배경
        EditorGUI.DrawRect(mapRect, new Color(0.2f, 0.2f, 0.2f, 1f));

        // 경계 그리기 
        DrawStageBounds(mapRect);
        // 격자 그리기
        DrawGrid(mapRect);

        // 마우스 이벤트 처리
        HandleMapMouseEvents(mapRect, stage);

        // Enemy 그리기
        DrawEnemiesOnMap(mapRect, stage);

        // 중심점 표시
        Vector2 center = mapRect.center;
        EditorGUI.DrawRect(new Rect(center.x - 2, center.y - 2, 4, 4), Color.yellow);
    }
    private void DrawStageBounds(Rect mapRect) {
        if (_currentConfig == null) return;

        Rect stageSize = _currentConfig.stageSize;

        // Stage 영역을 맵 좌표계로 변환
        Vector2 stageMin = WorldToScreenPosition(new Vector2(stageSize.x, stageSize.y), mapRect);
        Vector2 stageMax = WorldToScreenPosition(new Vector2(stageSize.x + stageSize.width, stageSize.y + stageSize.height), mapRect);

        // Stage 경계 사각형 계산
        float left = Mathf.Min(stageMin.x, stageMax.x);
        float right = Mathf.Max(stageMin.x, stageMax.x);
        float top = Mathf.Min(stageMin.y, stageMax.y);
        float bottom = Mathf.Max(stageMin.y, stageMax.y);

        // 맵 영역 내에서만 그리기
        left = Mathf.Max(left, mapRect.x);
        right = Mathf.Min(right, mapRect.x + mapRect.width);
        top = Mathf.Max(top, mapRect.y);
        bottom = Mathf.Min(bottom, mapRect.y + mapRect.height);

        if (left < right && top < bottom) {
            Rect stageBoundsRect = new Rect(left, top, right - left, bottom - top);

            // Stage 영역 반투명 채우기
            Color stageAreaColor = new Color(0.3f, 0.6f, 0.3f, 0.2f);
            EditorGUI.DrawRect(stageBoundsRect, stageAreaColor);

            // Stage 경계선 그리기 (더 두껍게)
            Color stageBorderColor = new Color(0.3f, 0.8f, 0.3f, 1f);
            float borderThickness = 2f;

            // 상단
            EditorGUI.DrawRect(new Rect(left, top, right - left, borderThickness), stageBorderColor);
            // 하단
            EditorGUI.DrawRect(new Rect(left, bottom - borderThickness, right - left, borderThickness), stageBorderColor);
            // 좌측
            EditorGUI.DrawRect(new Rect(left, top, borderThickness, bottom - top), stageBorderColor);
            // 우측
            EditorGUI.DrawRect(new Rect(right - borderThickness, top, borderThickness, bottom - top), stageBorderColor);
        }

        // Stage 크기 정보 표시
        Vector2 labelPosition = new Vector2(mapRect.x + 5, mapRect.y + mapRect.height - 40);
        string sizeInfo = $"Stage Size: {stageSize.width:F1} x {stageSize.height:F1}";
        GUI.Label(new Rect(labelPosition.x, labelPosition.y, 200, 20), sizeInfo,
                  new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.white } });

        string boundsInfo = $"Bounds: ({stageSize.x:F1}, {stageSize.y:F1}) to ({stageSize.x + stageSize.width:F1}, {stageSize.y + stageSize.height:F1})";
        GUI.Label(new Rect(labelPosition.x, labelPosition.y + 15, 300, 20), boundsInfo,
                  new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.gray } });
    }

    private void DrawGrid(Rect mapRect) {
        float gridSize = 20f * _mapZoom; // 1 Unity 단위 = 20픽셀 * 줌
        Color gridColor = new Color(0.4f, 0.4f, 0.4f, 0.5f);
        Color centerLineColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        // 월드 좌표 범위 계산 (화면에 보이는 월드 영역)
        Vector2 worldMin = ScreenToWorldPosition(new Vector2(mapRect.x, mapRect.y), mapRect);
        Vector2 worldMax = ScreenToWorldPosition(new Vector2(mapRect.x + mapRect.width, mapRect.y + mapRect.height), mapRect);

        // 정수 월드 좌표 범위 계산
        int minX = Mathf.FloorToInt(worldMin.x) - 1;
        int maxX = Mathf.CeilToInt(worldMax.x) + 1;
        int minY = Mathf.FloorToInt(worldMin.y) - 1;
        int maxY = Mathf.CeilToInt(worldMax.y) + 1;

        // 세로선 그리기 (X = 정수)
        for (int x = minX; x <= maxX; x++) {
            Vector2 worldPosTop = new Vector2(x, worldMin.y);
            Vector2 worldPosBottom = new Vector2(x, worldMax.y);

            Vector2 screenPosTop = WorldToScreenPosition(worldPosTop, mapRect);
            Vector2 screenPosBottom = WorldToScreenPosition(worldPosBottom, mapRect);

            // 화면 영역 내에 있는 선만 그리기
            if (screenPosTop.x >= mapRect.x && screenPosTop.x <= mapRect.x + mapRect.width) {
                Color lineColor = (x == 0) ? centerLineColor : gridColor;
                float lineWidth = (x == 0) ? 2f : 1f;

                EditorGUI.DrawRect(new Rect(screenPosTop.x - lineWidth / 2, mapRect.y, lineWidth, mapRect.height), lineColor);
            }
        }

        // 가로선 그리기 (Y = 정수)
        for (int y = minY; y <= maxY; y++) {
            Vector2 worldPosLeft = new Vector2(worldMin.x, y);
            Vector2 worldPosRight = new Vector2(worldMax.x, y);

            Vector2 screenPosLeft = WorldToScreenPosition(worldPosLeft, mapRect);
            Vector2 screenPosRight = WorldToScreenPosition(worldPosRight, mapRect);

            // 화면 영역 내에 있는 선만 그리기
            if (screenPosLeft.y >= mapRect.y && screenPosLeft.y <= mapRect.y + mapRect.height) {
                Color lineColor = (y == 0) ? centerLineColor : gridColor;
                float lineWidth = (y == 0) ? 2f : 1f;

                EditorGUI.DrawRect(new Rect(mapRect.x, screenPosLeft.y - lineWidth / 2, mapRect.width, lineWidth), lineColor);
            }
        }
    }

    private void HandleMapMouseEvents(Rect mapRect, StageData stage) {
        Event currentEvent = Event.current;
        Vector2 mousePos = currentEvent.mousePosition;

        if (mapRect.Contains(mousePos)) {
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0) {
                // 월드 좌표로 변환
                Vector2 worldPos = ScreenToWorldPosition(mousePos, mapRect);

                // 새 Enemy 추가
                AddEnemyAtPosition(stage, worldPos);
                Repaint();
            } else if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1) {
                // 우클릭 - Enemy 삭제
                Vector2 worldPos = ScreenToWorldPosition(mousePos, mapRect);
                RemoveEnemyAtPosition(stage, worldPos);
                Repaint();
            }
        }
    }

    private Vector2 ScreenToWorldPosition(Vector2 screenPos, Rect mapRect) {
        Vector2 localPos = screenPos - mapRect.center;
        Vector2 worldPos = localPos / (20f * _mapZoom); // 20픽셀 = 1 Unity 단위
        worldPos = new Vector2(Mathf.Round(worldPos.x), Mathf.Round(worldPos.y)); 
        return worldPos + _mapOffset;
    }

    private Vector2 WorldToScreenPosition(Vector2 worldPos, Rect mapRect) {
        Vector2 offsetWorldPos = worldPos - _mapOffset;
        Vector2 screenPos = offsetWorldPos * (20f * _mapZoom);
        return screenPos + mapRect.center;
    }

    private void DrawEnemiesOnMap(Rect mapRect, StageData stage) {
        if (stage.enemyDatas == null) return;

        for (int i = 0; i < stage.enemyDatas.Length; i++) {
            EnemyData enemy = stage.enemyDatas[i];
            Vector2 screenPos = WorldToScreenPosition(enemy.spawnPosition, mapRect);

            if (mapRect.Contains(screenPos)) {
                Color enemyColor = GetEnemyColor(enemy.enemyName);
                float size = 8f * _mapZoom;

                Rect enemyRect = new Rect(screenPos.x - size / 2, screenPos.y - size / 2, size, size);
                EditorGUI.DrawRect(enemyRect, enemyColor);

                // Enemy 정보 표시
                if (_mapZoom > 1.5f) {
                    GUI.Label(new Rect(screenPos.x + size / 2 + 2, screenPos.y - 10, 100, 20),
                             $"{enemy.enemyName}\nHP:{enemy.enemyHealth}",
                             new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.white } });
                }
            }
        }
    }

    private Color GetEnemyColor(EnemyName enemyName) {
        int enumIndex = (int)enemyName;

        // HSV 색상환을 이용해 enum 인덱스별 고유 색상 생성
        float hue = (enumIndex * 0.618033988749f) % 1.0f; // 황금비율로 색상 분산
        float saturation = 0.8f; // 채도 고정
        float value = 0.9f; // 명도 고정

        return Color.HSVToRGB(hue, saturation, value);
    }

    private void AddEnemyAtPosition(StageData stage, Vector2 position) {
        if (stage.enemyDatas == null) {
            stage.enemyDatas = new EnemyData[0];
        }

        EnemyData newEnemy = new EnemyData {
            enemyName = _selectedEnemyType,
            spawnPosition = position,
            enemyHealth = _selectedEnemyHealth,
            power = _selectedEnemyPower,
            expReward = _selectedEnemyExpReward,
            goldReward = _selectedEnemyGoldReward
        };

        System.Array.Resize(ref stage.enemyDatas, stage.enemyDatas.Length + 1);
        stage.enemyDatas[stage.enemyDatas.Length - 1] = newEnemy;

        _currentConfig.stages[_selectedStageIndex] = stage;
        EditorUtility.SetDirty(_currentConfig);
    }

    private void RemoveEnemyAtPosition(StageData stage, Vector2 position) {
        if (stage.enemyDatas == null) return;

        for (int i = 0; i < stage.enemyDatas.Length; i++) {
            if (Vector2.Distance(stage.enemyDatas[i].spawnPosition, position) < 0.5f) {
                var enemyList = stage.enemyDatas.ToList();
                enemyList.RemoveAt(i);
                stage.enemyDatas = enemyList.ToArray();

                _currentConfig.stages[_selectedStageIndex] = stage;
                EditorUtility.SetDirty(_currentConfig);
                break;
            }
        }
    }

    private void DrawEnemyList(StageData stage) {
        EditorGUILayout.LabelField("Enemy 목록", EditorStyles.boldLabel);

        if (stage.enemyDatas == null || stage.enemyDatas.Length == 0) {
            EditorGUILayout.HelpBox("Enemy가 없습니다. 맵에 좌클릭하여 추가하세요.", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginVertical("box");

        for (int i = 0; i < stage.enemyDatas.Length; i++) {
            DrawEnemyListItem(stage, i);
        }

        EditorGUILayout.EndVertical();

        if (GUILayout.Button("Add Enemy")) {
            AddEnemyAtPosition(stage, Vector2.zero);
        }
    }

    private void DrawEnemyListItem(StageData stage, int index) {
        EnemyData enemy = stage.enemyDatas[index];

        EditorGUILayout.BeginHorizontal("box");

        // Enemy 색상 표시
        Color enemyColor = GetEnemyColor(enemy.enemyName);
        Rect colorRect = GUILayoutUtility.GetRect(15, 15, GUILayout.Width(15), GUILayout.Height(15));
        EditorGUI.DrawRect(colorRect, enemyColor);

        // 번호
        EditorGUILayout.LabelField($"#{index + 1}", GUILayout.Width(25));

        // Type: EnumPopup
        EditorGUILayout.LabelField("Type:", GUILayout.Width(35));
        enemy.enemyName = (EnemyName)EditorGUILayout.EnumPopup(enemy.enemyName, GUILayout.Width(100));

        // Pos: Vector2Field
        EditorGUILayout.LabelField("Pos:", GUILayout.Width(30));
        enemy.spawnPosition = EditorGUILayout.Vector2Field("", enemy.spawnPosition, GUILayout.Width(100));

        // HP: IntField
        EditorGUILayout.LabelField("HP:", GUILayout.Width(25));
        enemy.enemyHealth = EditorGUILayout.IntField(enemy.enemyHealth, GUILayout.Width(50));

        // ATK: IntField
        EditorGUILayout.LabelField("ATK:", GUILayout.Width(30));
        enemy.power = EditorGUILayout.IntField(enemy.power, GUILayout.Width(50));

        // Exp: IntField
        EditorGUILayout.LabelField("Exp:", GUILayout.Width(30));
        enemy.expReward = EditorGUILayout.IntField(enemy.expReward, GUILayout.Width(40));

        // Gold: IntField
        EditorGUILayout.LabelField("Gold:", GUILayout.Width(30));
        enemy.goldReward = EditorGUILayout.IntField(enemy.goldReward, GUILayout.Width(40));

        GUILayout.FlexibleSpace();

        // 삭제 버튼
        if (GUILayout.Button("X", GUILayout.Width(25))) {
            var enemyList = stage.enemyDatas.ToList();
            enemyList.RemoveAt(index);
            stage.enemyDatas = enemyList.ToArray();
            _currentConfig.stages[_selectedStageIndex] = stage;
            EditorUtility.SetDirty(_currentConfig);
            return;
        }

        EditorGUILayout.EndHorizontal();

        // Enemy 데이터 업데이트
        stage.enemyDatas[index] = enemy;
        _currentConfig.stages[_selectedStageIndex] = stage;
    }

    // 대안: 더 컴팩트한 라벨 방식
    private void DrawEnemyListItemCompact(StageData stage, int index) {
        EnemyData enemy = stage.enemyDatas[index];

        EditorGUILayout.BeginHorizontal("box");

        // Enemy 색상 표시
        Color enemyColor = GetEnemyColor(enemy.enemyName);
        Rect colorRect = GUILayoutUtility.GetRect(15, 15, GUILayout.Width(15), GUILayout.Height(15));
        EditorGUI.DrawRect(colorRect, enemyColor);

        // 번호
        EditorGUILayout.LabelField($"#{index + 1}", GUILayout.Width(25));

        // Type: EnumPopup
        EditorGUILayout.LabelField("Type:", GUILayout.Width(35));
        enemy.enemyName = (EnemyName)EditorGUILayout.EnumPopup(enemy.enemyName, GUILayout.Width(100));

        // Pos: Vector2Field
        EditorGUILayout.LabelField("Pos:", GUILayout.Width(30));
        enemy.spawnPosition = EditorGUILayout.Vector2Field("", enemy.spawnPosition, GUILayout.Width(100));

        // HP: IntField
        EditorGUILayout.LabelField("HP:", GUILayout.Width(25));
        enemy.enemyHealth = EditorGUILayout.IntField(enemy.enemyHealth, GUILayout.Width(50));

        // ATK: IntField
        EditorGUILayout.LabelField("ATK:", GUILayout.Width(30));
        enemy.power = EditorGUILayout.IntField(enemy.power, GUILayout.Width(50));

        // Exp: IntField
        EditorGUILayout.LabelField("Exp:", GUILayout.Width(30));
        enemy.expReward = EditorGUILayout.IntField(enemy.expReward, GUILayout.Width(40));

        // Gold: IntField
        EditorGUILayout.LabelField("Gold:", GUILayout.Width(20));
        enemy.goldReward = EditorGUILayout.IntField(enemy.goldReward, GUILayout.Width(40));

        GUILayout.FlexibleSpace();

        // 삭제 버튼
        if (GUILayout.Button("X", GUILayout.Width(25))) {
            var enemyList = stage.enemyDatas.ToList();
            enemyList.RemoveAt(index);
            stage.enemyDatas = enemyList.ToArray();
            _currentConfig.stages[_selectedStageIndex] = stage;
            EditorUtility.SetDirty(_currentConfig);
            return;
        }

        EditorGUILayout.EndHorizontal();

        // Enemy 데이터 업데이트
        stage.enemyDatas[index] = enemy;
        _currentConfig.stages[_selectedStageIndex] = stage;
    }
    #endregion


    #region Utility Methods
    private void CreateNewStageConfig() {
        if (string.IsNullOrEmpty(_newConfigName)) {
            EditorUtility.DisplayDialog("오류", "Config 이름을 입력하세요.", "확인");
            return;
        }

        // 폴더 생성
        if (!Directory.Exists(_configSavePath)) {
            Directory.CreateDirectory(_configSavePath);
        }

        // ScriptableObject 생성
        SO_StageConfig newConfig = CreateInstance<SO_StageConfig>();
        newConfig.stages = new StageData[0];
        // 파일 저장
        string assetPath = _configSavePath + _newConfigName + ".asset";
        AssetDatabase.CreateAsset(newConfig, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 생성된 Config 로드
        _currentConfig = newConfig;
        RefreshStageData();

        EditorUtility.DisplayDialog("성공", $"새 Config가 생성되었습니다: {assetPath}", "확인");
    }

    private void SaveCurrentConfig() {
        if (_currentConfig == null) return;

        EditorUtility.SetDirty(_currentConfig);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("저장 완료", "Config가 저장되었습니다.", "확인");
    }

    private void ReloadCurrentConfig() {
        if (_currentConfig == null) return;

        string path = AssetDatabase.GetAssetPath(_currentConfig);
        _currentConfig = AssetDatabase.LoadAssetAtPath<SO_StageConfig>(path);
        RefreshStageData();

        EditorUtility.DisplayDialog("리로드 완료", "Config가 리로드되었습니다.", "확인");
    }

    private void RefreshStageData() {
        if (_currentConfig == null || _currentConfig.stages == null) {
            _stageExpanded = new bool[0];
            _selectedStageIndex = -1;
            return;
        }

        _stageExpanded = new bool[_currentConfig.stages.Length];

        // 선택된 인덱스 유효성 검사
        if (_selectedStageIndex >= _currentConfig.stages.Length) {
            _selectedStageIndex = -1;
        }
    }

    private void AddNewStage() {
        if (_currentConfig.stages == null) {
            _currentConfig.stages = new StageData[0];
        }

        // 새 Stage ID 계산
        int newStageId = _currentConfig.stages.Length > 0 ? _currentConfig.stages.Max(s => s.stageId) + 1 : 1;

        StageData newStage = new StageData {
            stageId = newStageId,
            stageName = $"Stage {newStageId}",
            description = $"Stage {newStageId} 설명",
            enemyDatas = new EnemyData[0],
            goldReward = 100,
            expReward = 50,
            autoStartNextStage = true,
            timeLimit = 0f
        };

        System.Array.Resize(ref _currentConfig.stages, _currentConfig.stages.Length + 1);
        _currentConfig.stages[_currentConfig.stages.Length - 1] = newStage;

        RefreshStageData();
        EditorUtility.SetDirty(_currentConfig);
    }

    private void RemoveStage(int index) {
        if (_currentConfig.stages == null || index < 0 || index >= _currentConfig.stages.Length)
            return;

        var stageList = _currentConfig.stages.ToList();
        stageList.RemoveAt(index);
        _currentConfig.stages = stageList.ToArray();

        RefreshStageData();
        EditorUtility.SetDirty(_currentConfig);
    }

    private void ClearAllStages() {
        _currentConfig.stages = new StageData[0];
        RefreshStageData();
        EditorUtility.SetDirty(_currentConfig);
    }

    private void SortStagesByID() {
        if (_currentConfig.stages != null && _currentConfig.stages.Length > 1) {
            _currentConfig.stages = _currentConfig.stages.OrderBy(s => s.stageId).ToArray();
            EditorUtility.SetDirty(_currentConfig);
        }
    }

    private void MultiplyAllRewards(float multiplier) {
        if (_currentConfig.stages == null) return;

        for (int i = 0; i < _currentConfig.stages.Length; i++) {
            StageData stage = _currentConfig.stages[i];

            // Stage 보상
            stage.goldReward = Mathf.RoundToInt(stage.goldReward * multiplier);
            stage.expReward = Mathf.RoundToInt(stage.expReward * multiplier);

            // Enemy 보상
            if (stage.enemyDatas != null) {
                for (int j = 0; j < stage.enemyDatas.Length; j++) {
                    EnemyData enemy = stage.enemyDatas[j];
                    enemy.goldReward = Mathf.RoundToInt(enemy.goldReward * multiplier);
                    enemy.expReward = Mathf.RoundToInt(enemy.expReward * multiplier);
                    stage.enemyDatas[j] = enemy;
                }
            }

            _currentConfig.stages[i] = stage;
        }

        EditorUtility.SetDirty(_currentConfig);
    }

    private void ResetAllRewards() {
        if (_currentConfig.stages == null) return;

        for (int i = 0; i < _currentConfig.stages.Length; i++) {
            StageData stage = _currentConfig.stages[i];

            // Stage 기본 보상으로 리셋
            stage.goldReward = 100;
            stage.expReward = 50;

            // Enemy 기본 보상으로 리셋
            if (stage.enemyDatas != null) {
                for (int j = 0; j < stage.enemyDatas.Length; j++) {
                    EnemyData enemy = stage.enemyDatas[j];
                    enemy.goldReward = 5;
                    enemy.expReward = 10;
                    stage.enemyDatas[j] = enemy;
                }
            }

            _currentConfig.stages[i] = stage;
        }

        EditorUtility.SetDirty(_currentConfig);
    }
    #endregion
}