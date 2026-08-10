using UnityEngine;

public class GotoMenuButton : MonoBehaviour
{
    [SerializeField] private string menuSceneName;

    public void GoToMenu()
    {
        LevelManager.Instance.LoadScene(menuSceneName);
    }
}
