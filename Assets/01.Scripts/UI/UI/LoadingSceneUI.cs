using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Zenject;
using Game.ViewModels;
using Game.Core;
namespace Game.UI {
    /// <summary>
    /// 로딩씬의 Fill Image, Text 처리
    /// </summary>
    public class LoadSceneUI : MonoBehaviour {

        [Inject] private SceneViewModel _viewModel;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _loadProgressText;

        [SerializeField] private float _fakeLoadTime = 2f; // 최소 로딩 시간
        private float _startTime = 0; // 시작시간
        private float _fakeProgress = 0; // 가짜 로딩 


        private void Update() {
            // _startTime이 0이면 현재 시간으로 초기화
            if (_startTime <= 0f) _startTime = Time.time;

            // 실제 로딩 진행률
            float realProgress = _viewModel.GetLoadingProgress();
            // 경과 시간 계산
            float elapsedTime = Time.time - _startTime;

            // 최소 로딩 시간보다 짧을 경우 가짜 로딩 적용
            bool isFakeLoading = elapsedTime < _fakeLoadTime;

            // 진행률 결정
            float progress;

            if (isFakeLoading) {
                _fakeProgress = Mathf.MoveTowards(_fakeProgress, 1f, Time.deltaTime / _fakeLoadTime);
                progress = Mathf.Min(realProgress, _fakeProgress);
            } else {
                progress = realProgress;
            }

            // 로드가 일정량 보다 높으면 1로 표시
            if (progress >= 0.89f) progress = 1f;

            // UI에 적용
            _fillImage.fillAmount = progress;
            _loadProgressText.text = $"{Mathf.RoundToInt(progress * 100f)}%";
        }

        #region Test
        [ContextMenu("0.5초 뒤 -> Loading -> Play이동")]
        private void TestMovePlayScene() {
            _viewModel.LoadSceneWithLoading(SceneName.PlayScene, 0.5f);
        }


        #endregion
    }
}