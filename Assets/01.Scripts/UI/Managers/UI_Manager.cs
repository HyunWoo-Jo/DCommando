using UnityEngine;
using Zenject;
using Game.ViewModels;
using Cysharp.Threading.Tasks;
using Game.Core;

namespace Game.UI
{
    /// <summary>
    /// UI 매니저 - ViewModel을 통한 UI 생성 및 관리
    /// </summary>
    public class UI_Manager : MonoBehaviour
    {
        [Inject] private UIViewModel _viewModel;
        
        private static UI_Manager _instance;
        public static UI_Manager Instance => _instance;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _viewModel.Notify();
        }
        
        /// <summary>
        /// Screen UI 열기
        /// </summary>
        public async UniTask<T> OpenScreenAsync<T>(UI_Name uiName) where T : Component
        {
            return await _viewModel.OpenScreenAsync<T>(uiName);
        }
        
        /// <summary>
        /// Popup UI 열기
        /// </summary>
        public async UniTask<T> OpenPopupAsync<T>(UI_Name uiName) where T : Component
        {
            return await _viewModel.OpenPopupAsync<T>(uiName);
        }
        
        /// <summary>
        /// UI 닫기
        /// </summary>
        public void CloseUI(UI_Name uiName)
        {
            _viewModel.CloseUI(uiName);
        }
        
        /// <summary>
        /// 골드 UI 열기 (예시)
        /// </summary>
        public async UniTask<MonoBehaviour> OpenGoldUIAsync()
        {
            return await OpenScreenAsync<MonoBehaviour>(UI_Name.Gold_UI);
        }
        
        /// <summary>
        /// 컨트롤러 UI 열기 (예시)
        /// </summary>
        public async UniTask<MonoBehaviour> OpenControllerUIAsync()
        {
            return await OpenPopupAsync<MonoBehaviour>(UI_Name.Controller_UI);
        }
    }
}