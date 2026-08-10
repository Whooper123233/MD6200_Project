using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockedOverlay;

    private void Start()
    {
        var saveEntry = SaveManager.Instance.GetLevelData(levelData.levelId);
        bool unlocked = saveEntry.isUnlocked || levelData.unlockRequirement == null;

        lockedOverlay.SetActive(!unlocked);
        button.interactable = unlocked;

        int collected = saveEntry.collectedGemIds.Count;

        button.onClick.AddListener(() =>
        {
            LevelManager.Instance.LoadScene(levelData.sceneName);
        });
    }
}
