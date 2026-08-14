using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoMenuButton : MonoBehaviour
{
    [SerializeField] private string menuSceneName;

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SaveManager.Instance.SetReturnScene(SceneManager.GetActiveScene().name);
        LevelManager.Instance.LoadScene(menuSceneName);
    }
}
