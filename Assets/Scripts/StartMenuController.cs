using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenuController : MonoBehaviour
{
    [SerializeField] private string cutsceneSceneName;
    [SerializeField] private string firstLevelSceneName;

    public void StartNewGame()
    {
        SaveManager.Instance.StartNewGame(); 
        LevelManager.Instance.LoadScene(cutsceneSceneName);
    }

    public void ContinueGame()
    {
        var save = SaveManager.Instance.data;
        string target = string.IsNullOrEmpty(save.lastScene) ? firstLevelSceneName : save.lastScene;
        LevelManager.Instance.LoadScene(target);
    }
}

