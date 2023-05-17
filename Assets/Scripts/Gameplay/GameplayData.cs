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

    public static GameplayData LoadData()
    {
        string saveFilePath = "Assets/GameData/GameplayData.json";

        if (File.Exists(saveFilePath))
        {
            string jsonData = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<GameplayData>(jsonData);
        }
        else
        {
            Debug.LogWarning("No GameplayData found. Creating a new file with default values.");
            GameplayData newData = new GameplayData();
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

    public TowerTemplate GetTemplateFromName(string name)
    {
        foreach (TowerTemplate template in towerTemplates)
        {
            if (template.name == name)
            {
                return template;
            }
        }

        return null;
    }

    public void AddTowerTemplate(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            name = "Default";
        }

        string safeNewTowerName = name;

        int counter = 1;
        while (GetTemplateFromName(safeNewTowerName) != null)
        {
            safeNewTowerName = name + counter;
            ++counter;
        }

        TowerTemplate newTower = new TowerTemplate();
        newTower.name = safeNewTowerName;
        towerTemplates.Add(newTower);
    }

    public void RemoveTowerTemplate(TowerTemplate template)
    {
        towerTemplates.Remove(template);
    }
}
