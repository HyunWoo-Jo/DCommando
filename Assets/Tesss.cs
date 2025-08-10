using Game.ViewModels;
using UnityEngine;
using Zenject;

public class Tesss : MonoBehaviour
{
    [Inject] private SceneViewModel viewMode;
 
    private void Awake() {
        Debug.Log(Time.time);
    }
    [ContextMenu("GoToPlay")]
    public void GPlayScene() {
        viewMode.LoadSceneWithLoading(Game.Core.SceneName.PlayScene, 0.5f);
    }
    [ContextMenu("GoToMain")]
    public void GMainScene() {
        viewMode.LoadSceneWithLoading(Game.Core.SceneName.MainScene, 0.5f);
    }
}
