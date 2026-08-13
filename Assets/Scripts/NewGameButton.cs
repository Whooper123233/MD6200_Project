using UnityEngine;

public class NewGameButton : MonoBehaviour
{
    [SerializeField] private string cutsceneSceneName;

    public void StartNewGame()
    {
        SaveManager.Instance.StartNewGame();
        LevelManager.Instance.LoadScene(cutsceneSceneName); 
    }
}