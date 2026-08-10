using UnityEngine;
using UnityEngine.SceneManagement;
public class StartMenuController : MonoBehaviour
{
    [SerializeField] private string cutsceneSceneName;
    [SerializeField] private string firstLevelSceneName; 

    public void PlayTestLevel()
    {
        var save = SaveManager.Instance.data;

        if (save.hasSeenIntroCutscene)
        {
            string target = string.IsNullOrEmpty(save.lastScene) ? firstLevelSceneName : save.lastScene;
            LevelManager.Instance.LoadScene(target);
        }
        else
        {
            LevelManager.Instance.LoadScene(cutsceneSceneName);
        }
    }
}
