using UnityEngine;
using UnityEngine.UI;
using Game.ViewModels;
using R3;
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using Zenject;
using UnityEngine.Assertions;
namespace Game.UI
{
    public class GoldView : MonoBehaviour
    {
        [Inject] private GoldViewModel _viewModel;
        private CompositeDisposable _disposables = new();

        [Header("UI 컴포넌트")]
        [SerializeField] private TextMeshProUGUI _goldText;

#if UNITY_EDITOR
        [SerializeField] private Button _testAddButton;
        [SerializeField] private Button _testSpendButton;
#endif

        private void Awake() {
#if UNITY_EDITOR // Assertion
            RefAssert();
            TestBind();
#endif

        }
        private void Start() {
            Bind();
        }


#if UNITY_EDITOR
        // 검증
        private void RefAssert() {
            Assert.IsNotNull(_goldText, "GoldText가 할당되지 않았습니다!");
            
        }
#endif

        private void Bind()
        {
            // 골드 텍스트 바인딩
            _viewModel.RORP_GoldText
                .Subscribe(UpdateGoldTest)
                .AddTo(this);
        }


        private void UpdateGoldTest(string goldStr) {
            _goldText.text = goldStr;
        }

        private void UpdateGoldColor(Color color) {
            _goldText.color = color;
        }


#if UNITY_EDITOR
        private void TestBind() {
            // 버튼 이벤트 바인딩
            _testAddButton?.onClick.AddListener(() => OnTestAddGoldClicked());
            _testSpendButton?.onClick.AddListener(() => OnTestSpendGoldClicked());
        }

        /// <summary>
        /// Test용 
        /// </summary>
        private void OnTestAddGoldClicked()
        {
             _viewModel.AddGold(100);
        }
        
        private void OnTestSpendGoldClicked()
        {
             _viewModel.SpendGold(50);
        }
#endif
      
    }
}