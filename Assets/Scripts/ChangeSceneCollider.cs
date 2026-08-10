using UnityEngine;

public class ChangeSceneCollider : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private LevelData currentLevel;
    [SerializeField] private LevelData nextLevel;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SaveManager.Instance.CompleteLevel(currentLevel.levelId);

        if (nextLevel != null)
        {
            SaveManager.Instance.UnlockLevel(nextLevel.levelId);
        }
    }
    public void ChangeScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}
