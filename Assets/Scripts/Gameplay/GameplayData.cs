using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class GameplayData
{
    public string version;
    public int startingGold;
    public int startingIncome;
    public List<TowerTemplate> towerTemplates;

    

    public GameplayData()
    {
        version = "0.0.1";
        startingGold = 100;
        startingIncome = 10;
        towerTemplates = new List<TowerTemplate>();
    }

    private static GameplayData _singleton = null;
    public static GameplayData Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = LoadData();
            }

            return _singleton;
        }
    }

    public static void ReloadSingleton()
    {
        _singleton = LoadData();
    }

    public static GameplayData LoadData()
    {
        string saveFilePath = "Assets/GameData/GameplayData.json";

        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            GameplayData data = JsonUtility.FromJson<GameplayData>(jsonData);
            TowerTemplate.EnsureAllTowers(data);
            return data;
        }
        else
        {
            Debug.LogWarning("No GameplayData found. Creating a new file with default values.");
            GameplayData newData = new GameplayData();
            TowerTemplate.EnsureAllTowers(newData);
            newData.SaveData();
            return newData;
        }
    }

    public void SaveData()
    {
        string saveFilePath = "Assets/GameData/GameplayData.json";
        string jsonData = JsonUtility.ToJson(this, true);
        File.WriteAllText(saveFilePath, jsonData);
        Debug.Log("GameplayData saved successfully.");
    }

    public TowerTemplate GetTemplateFromType(TowerTypeID type)
    {
        foreach (TowerTemplate template in towerTemplates)
        {
            if (template.type == type)
            {
                return template;
            }
        }

        return null;
    }
}
