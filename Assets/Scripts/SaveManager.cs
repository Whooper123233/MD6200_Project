using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class LevelSaveData
{
    public string levelId;
    public bool isUnlocked;
    public bool isCompleted;
    public List<int> collectedGemIds = new List<int>();
}

[Serializable]
public class SaveData
{
    public List<LevelSaveData> levels = new List<LevelSaveData>();
    public bool hasSeenIntroCutscene;
    public string lastScene;
    public string returnScene;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public SaveData data = new SaveData();

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public LevelSaveData GetLevelData(string levelId)
    {
        var entry = data.levels.Find(l => l.levelId == levelId);
        if (entry == null)
        {
            entry = new LevelSaveData { levelId = levelId };
            data.levels.Add(entry);
        }
        return entry;
    }

    public void UnlockLevel(string levelId)
    {
        GetLevelData(levelId).isUnlocked = true;
        Save();
    }

    public void CollectGem(string levelId, int gemId)
    {
        var entry = GetLevelData(levelId);
        if (!entry.collectedGemIds.Contains(gemId))
        {
            entry.collectedGemIds.Add(gemId);
            Save();
        }
    }

    public void CompleteLevel(string levelId)
    {
        GetLevelData(levelId).isCompleted = true;
        Save();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            data = new SaveData();
        }
    }

    public void SetIntroCutsceneSeen()
    {
        data.hasSeenIntroCutscene = true;
        Save();
    }

    public void SetLastScene(string sceneName)
    {
        data.lastScene = sceneName;
        Save();
    }

    public void SetReturnScene(string sceneName)
    {
        data.returnScene = sceneName;
        Save();
    }
}