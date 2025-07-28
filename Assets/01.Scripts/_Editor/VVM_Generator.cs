using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.VersionControl;
namespace CA.UI {
    /// <summary>
    /// MVP UI 자동 생성 코드
    /// </summary>
    public class VVM_Generator : EditorWindow {

        private string _vvmName;
        private string _viewContext;
        private string _viewModelContect;
        [MenuItem("Tools/Generator/VVM_UI")]
        public static void OpenWindow() {
            var window = GetWindow<VVM_Generator>("VVM_Generator");
            
            window.maxSize = new Vector2(400, 70);
            window.Show();
           

        }
        private void OnGUI() {
            GUILayout.BeginHorizontal();
            GUILayout.Label("name", EditorStyles.boldLabel, GUILayout.Width(40));
            _vvmName = EditorGUILayout.TextField(_vvmName);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Generate")) {
                SetContext();
                GenerateScript();
            }
        }
        /// <summary>
        /// 내용 작성
        /// </summary>
        private void SetContext() {

            _viewContext = $@"
using UnityEngine;
using Zenject;
using System;
using ViewModel;
////////////////////////////////////////////////////////////////////////////////////
// Auto Generated Code
namespace UI
{{
    public class {_vvmName}View : MonoBehaviour
    {{
        [Inject] private {_vvmName}ViewModel _viewModel;

        // UnityReference

        //
        private void Awake() {{
#if UNITY_EDITOR // Assertion
            RefAssert();
#endif
            Bind();

        }}
        private void Start() {{
            _viewModel.Notify();
        }}

#if UNITY_EDITOR
        // 검증
        private void RefAssert() {{

        }}
        
        private void Bind(){{

        }}   
#endif
        // UI 갱신
        private void UpdateUI() {{
            
        }}
////////////////////////////////////////////////////////////////////////////////////
        // your logic here

    }}
}}
";
            _viewModelContect = $@"
using Zenject;
using System;
namespace ViewModels
{{
    public class {_vvmName}ViewModel 
    {{   

        /// <summary>
        /// 데이터 변경 알림
        /// </summary>
        public void Notify() {{

        }}

    }}
}} 
";

        }
        /// <summary>
        /// 생성
        /// </summary>
        private void GenerateScript() {
            string path = $"{Application.dataPath}/01.Scripts/";
     
            string viewPath = path + $"UI/{_vvmName}View.cs";
            string viewModelPath = path + $"ViewModels/{_vvmName}ViewModel.cs";

            File.WriteAllText(viewPath, _viewContext);
            File.WriteAllText(viewModelPath, _viewModelContect);

            AssetDatabase.Refresh();
            Close();
        }
    }
}
