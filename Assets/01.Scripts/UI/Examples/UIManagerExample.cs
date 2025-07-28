using UnityEngine;
using UI;
using Cysharp.Threading.Tasks;

namespace Game.UI
{
    /// <summary>
    /// UI Manager 사용 예시
    /// </summary>
    public class UIManagerExample : MonoBehaviour
    {
        private async void Start()
        {
            // 사용 예시들
            await ExampleUsage();
        }
        
        private async UniTask ExampleUsage()
        {
            // UI Manager가 준비될 때까지 대기
            await UniTask.WaitUntil(() => UI_Manager.Instance != null);
            
            // 골드 UI 열기
            var goldView = await UI_Manager.Instance.OpenGoldUIAsync();
            if (goldView != null)
            {
                Debug.Log("골드 UI가 성공적으로 열었습니다!");
            }
            
            // 컨트롤러 UI 열기
            var controllerView = await UI_Manager.Instance.OpenControllerUIAsync();
            if (controllerView != null)
            {
                Debug.Log("컨트롤러 UI가 성공적으로 열었습니다!");
            }
            
            // 3초 후 UI 닫기
            await UniTask.Delay(3000);
            UI_Manager.Instance.CloseUI("GoldUI");
            UI_Manager.Instance.CloseUI("ControllerUI");
        }
        
        // 버튼에서 호출할 수 있는 메서드들
        public async void OnOpenGoldUIClicked()
        {
            await UI_Manager.Instance.OpenGoldUIAsync();
        }
        
        public async void OnOpenControllerUIClicked()
        {
            await UI_Manager.Instance.OpenControllerUIAsync();
        }
        
        public void OnCloseAllUIClicked()
        {
            UI_Manager.Instance.CloseUI("GoldUI");
            UI_Manager.Instance.CloseUI("ControllerUI");
        }
    }
}