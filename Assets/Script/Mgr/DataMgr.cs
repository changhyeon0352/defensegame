using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Data { }
public enum DataType {HeroDatas,upgrade,inventory}
public class Upgrade:Data
{
    int melee_armor;
    int melee_damage;
    int range_damage;
    int unitPoint;
    int hero_equipment;
    int hero_skill;
}
public class Inventory:Data
{
    
}
public class Property:Data
{
    public int money;
}
public class DefenseData : Data
{
    public int townHp;
    public int townHpMax;
    public int allyDieNum;
}
public class DataMgr : Singleton<DataMgr>
{
    [SerializeField] private Image townHpImage;
    private List<HeroData> fightingHeroDataList = new();
    private HeroDatas myHeroDatas;
    private Upgrade myUpgrade;
    private Inventory myInventory;
    private DefenseData defenseData = new();
    private Property myProperty;
    public Property MyProperty { get { return myProperty; } }
    string heroDatafileName = "heroDataList.json";
    string inventoryfileName = "inventory.json";
    //인벤토리
    private void Start()
    {
        LoadDataFromJson(DataType.HeroDatas);
        InitDefenseData();
    }

    public List<HeroData> FightingHeroDataList { get { return fightingHeroDataList; } }

    public void InitDefenseData()
    {
        defenseData.townHpMax = 100;
        defenseData.townHp = 100;
        defenseData.allyDieNum = 0;
    }
    public void DieAlly(Unit unit)
    {
        defenseData.allyDieNum++;
        if(unit.CompareTag("Hero"))
        {
            Hero hero =unit.GetComponent<Hero>();
            hero.Data.isDead = true;
        }
    }
    public void MonsterEnterTown(Unit unit)
    {
        defenseData.townHp -= unit.unitData.Atk/2;
        if(defenseData.townHp<=0)
        {
            //게임 엔드
        }
        townHpImage.fillAmount = (float)defenseData.townHp / (float)defenseData.townHpMax;
    }
    void SaveDataToJson(DataType dataType)
    {
        Data data;
        string fileName;
        SwitchData(dataType, out data, out fileName);
        string jsonData = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.dataPath, fileName);
    }
    void LoadDataFromJson(DataType dataType)
    {
        Data data;
        string fileName;
        SwitchData(dataType, out data, out fileName);
        string path = Path.Combine(Application.dataPath, fileName);
        string jsonData = File.ReadAllText(path);
        if(data is HeroDatas)
        {
            data = JsonUtility.FromJson<HeroDatas>(jsonData);
        }
        else if(data is Upgrade) { data = JsonUtility.FromJson<Upgrade>(jsonData); }
    }
    private void SwitchData(DataType dataType, out Data data, out string fileName)
    {
        data = new Data();
        fileName = "";
        switch (dataType)
        {
            case DataType.HeroDatas:
                data = (myHeroDatas!=null)?myHeroDatas:new HeroDatas();
                fileName = heroDatafileName;
                break;
            case DataType.upgrade:
                data = (myUpgrade != null) ? myUpgrade : new Upgrade();
                fileName = inventoryfileName;
                break;
        }
    }

    

    public bool AddFightingHeroData(HeroData data)
    {
        if (fightingHeroDataList.Contains(data))
        {
            fightingHeroDataList.Remove(data);
            return false;
        }
        if (fightingHeroDataList.Count < 4)
        {
            fightingHeroDataList.Add(data);
        }
        return true;
    }//HeroSlot에서 여기로 더해줌
}
