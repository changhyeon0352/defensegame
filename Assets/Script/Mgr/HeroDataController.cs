using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class HeroDataController : Singleton<HeroDataController>
{
    public HeroData heroData;
    public HeroDatas heroDatas;
    private int heroID=0;

    private void Start()
    {
        LoadHeroDataFromJson();
        heroID=heroDatas.heroDataList[heroDatas.heroDataList.Count - 1].id+1;
        int a = 0;
    }
    [ContextMenu("To Json Data")]
    void SavePlayerDataToJson()
    {

        string jsonData = JsonUtility.ToJson(heroDatas, true);
        string path = Path.Combine(Application.dataPath, "heroDataList.json");
        File.WriteAllText(path, jsonData);
    }
    [ContextMenu("From Json Data")]
    void LoadHeroDataFromJson()
    {

        string path = Path.Combine(Application.dataPath, "heroDataList.json");
        string jsonData = File.ReadAllText(path);
        heroDatas=JsonUtility.FromJson<HeroDatas>(jsonData);
    }
    [ContextMenu("Add Hero To List")]
    void AddHeroDataToList()
    {
        if(heroDatas.heroDataList==null)
        {
            heroDatas.heroDataList = new();
        }
        heroData.id = heroID;
        heroDatas.heroDataList.Add(heroData);
        heroID++;   
    }
}
[Serializable]
public class HeroData
{
    public int id;
    public HeroClass heroClass;
    public string name;
    public int level;
    public int exp;
    public int level_Weapon;
    public int level_Armor;
    public int[] level_Skills;
    public bool isDead;
    
}
[Serializable]
public class HeroDatas
{
    public List<HeroData> heroDataList;
}

