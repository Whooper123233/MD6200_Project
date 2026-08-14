using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private string menuSceneName;

    private void Start()
    {
        SaveManager.Instance.SetGameCompleted();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.LoadScene(menuSceneName);
            }
            else
            {
                SceneManager.LoadScene(menuSceneName);
            }
        }
    }
}
