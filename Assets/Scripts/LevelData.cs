using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]

public class LevelData : ScriptableObject
{
    public string levelId; 
    public string sceneName;
    public int totalGems;
    public LevelData unlockRequirement;
}
