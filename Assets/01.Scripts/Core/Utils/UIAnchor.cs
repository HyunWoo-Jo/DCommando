using UnityEngine;

namespace Game.Core {
    /// <summary>
    /// UI 생성 위치를 미리 지정하는 마커
    /// MainCanvas 하위에 배치하여 Addressable UI의 생성 위치를 정의
    /// </summary>
    public class UIAnchor : MonoBehaviour {
        [Header("UI 앵커 설정")]
        [SerializeField] private UIName _ui_Name;
        [SerializeField] private string _customName = "";
#if UNITY_EDITOR
#pragma warning disable CS0414
        [Header("디버그 정보")]
        [SerializeField, TextArea(2, 4)] 
        private string _debugInfo = "";
#pragma warning restore
#endif
        public UIName Ui_Name => _ui_Name;
        public string CustomName => _customName;
        public Transform AnchorTransform => transform;

        /// <summary>
        /// 씬에서 표시될 이름 반환
        /// </summary>
        public string GetDisplayName() {
            if (!string.IsNullOrEmpty(_customName))
                return $"[{_ui_Name}] {_customName}";

            return $"[{_ui_Name}]";
        }

        private void OnValidate() {
            // 씬에서 GameObject 이름 자동 업데이트
            gameObject.name = GetDisplayName();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            // 씬에서 앵커 위치를 시각적으로 표시
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 50f);
            
            // 앵커 타입 텍스트 표시
            UnityEditor.Handles.Label(transform.position + Vector3.up * 30f, GetDisplayName());
        }
#endif
    }
}