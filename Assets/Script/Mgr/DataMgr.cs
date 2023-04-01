using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Data { }
public enum BarracksUpgradeType { melee_armor = 0, range_damage, unitPoint }
public enum UpgradeInfoType { cost=0,value,name,upgladeNum}
public enum DataType {HeroDatas,inventory,BarracksData,Property,BlacksmithData,GuildData}
[Serializable]
public class BarracksData:Data
{
    //강화비용
    //강화수치
    //타입별 스트링
    public int[] costs;
    public int[] values;
    //public int[] costs;
    //public int[] increaseValues;
    public string[] typeStr = new string[3];
    public int[] upgladeNums;
}
[Serializable]
public class BlacksmithData : Data
{
    public int[] costs;
    public int[] values;
    public string[] typeStr = new string[3];
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
[Serializable]
public class IntArr
{
    public int[] intArr;
    public IntArr(int[] _intArr)
    {
        intArr = _intArr;
    }
}

public class DataMgr : Singleton<DataMgr>
{
    [SerializeField] private Image townHpImage;
    private List<HeroData> fightingHeroDataList = new();
    private HeroDatas myHeroDatas;
    private Inventory myInventory;
    private DefenseData defenseData = new();
    private Property propertyData;
    public BarracksData barracksData;
    public BlacksmithData blacksmithData;
    private HeroDatas guildHeroDatas;
    string dataPath;

    public HeroDatas MyHeroDatas { get { return myHeroDatas; } }
    public List<HeroData> FightingHeroDataList { get { return fightingHeroDataList; } }
    public HeroDatas GuildHeroDatas { get { return guildHeroDatas; } }
    public DefenseData DefenseData { get { return defenseData; } }
    public int Money { get => propertyData.money;
        set {
            propertyData.money = value;
            SaveDataToJson(DataType.Property);
            UIMgr.Instance.ChangeMoneyText(Money);
        } 
    }
    
    //인벤토리
    override protected void Awake()
    {
        dataPath = Path.Combine(Application.streamingAssetsPath, "Data");
        LoadDataFromJson(DataType.HeroDatas);
        LoadDataFromJson(DataType.Property);
        LoadDataFromJson(DataType.BarracksData);
        LoadDataFromJson(DataType.BlacksmithData);
        InitDefenseData();
        LoadDataFromJson(DataType.GuildData);
        GenerateNewGuildList();
        //blacksmithData = new();
        //blacksmithData.values = new int[8] { 10, 20, 30, 40, 20, 35, 50, 70 };
        //blacksmithData.costs = new int[8] { 1000, 1500, 2000, 2500, 1000, 1500, 2000, 2500 };
        //blacksmithData.typeStr = new string[2] { "영웅 공격력", "영웅 방어력" };
        //SaveDataToJson(DataType.BlacksmithData);
    }
    private void Start()
    {
        UIMgr.Instance.ChangeMoneyText(Money);
    }
    
    
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
            hero.HeroData.isDead = true;
        }
    }
    //몬스터가 타운에 들어가면 타운 피 깍음
    public void MonsterEnterTown(Unit unit)
    {
        defenseData.townHp -= unit.UnitData.Atk/2;
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
        string path = Path.Combine(dataPath, fileName);
        File.WriteAllText(path, jsonData);
    }
    void LoadDataFromJson(DataType dataType)
    {
        Data data;
        string fileName;
        SwitchData(dataType, out data, out fileName);
        string path = Path.Combine(dataPath, fileName);
        string jsonData = File.ReadAllText(path);
        
            


        if (data is HeroDatas) { 
            if(dataType==DataType.GuildData)
                guildHeroDatas= JsonUtility.FromJson<HeroDatas>(jsonData);
            else
                myHeroDatas = JsonUtility.FromJson<HeroDatas>(jsonData); 
        }
        else if (data is BarracksData) { barracksData = JsonUtility.FromJson<BarracksData>(jsonData); }
        else if (data is Property) { propertyData = JsonUtility.FromJson<Property>(jsonData); }
        else if (data is BlacksmithData) { blacksmithData = JsonUtility.FromJson<BlacksmithData>(jsonData); }
        

    }
    //데이터를 특정 데이터 타입으로 바꿔줌
    private void SwitchData(DataType dataType, out Data data, out string fileName)
    {
        data = new Data();
        fileName = "";
        switch (dataType)
        {
            case DataType.HeroDatas:
                data = (myHeroDatas!=null)?myHeroDatas:new HeroDatas();
                fileName = "heroDataList.json";
                break;
            case DataType.GuildData:
                data = (guildHeroDatas != null) ? guildHeroDatas : new HeroDatas();
                fileName = "GuildData.json";
                break;
            case DataType.BarracksData:
                data= (barracksData != null) ? barracksData : new BarracksData();
                fileName = "BarracksData.json";
                break;
            case DataType.Property:
                data = (propertyData != null) ? propertyData : new Property();
                fileName = "PropertyData.json";
                break;
            case DataType.BlacksmithData:
                data = (blacksmithData != null) ? blacksmithData : new BlacksmithData();
                fileName = "BlacksmithData.json";
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
    public void UpgradeBarracks(int typeNum,int cost)
    {
        Money -= cost;
        barracksData.upgladeNums[typeNum]++;
        SaveDataToJson(DataType.BarracksData);
    }
    public void UpgradeBlacksmith(HeroData heroData,int typeNum, int cost)
    {
        Money -= cost;
        if(typeNum==0)
        {
            heroData.level_Weapon++;
        }
        else
        {
            heroData.level_Armor++;
        }
        SaveDataToJson(DataType.HeroDatas);
    }
    public void HeroDie(Hero hero)
    {
        myHeroDatas.heroDataList[hero.HeroData.id - 1].isDead = true;
        SaveDataToJson(DataType.HeroDatas);
    }
    public int GetBarracksUpgradeValue(BarracksUpgradeType upgladeType)
    {
        return barracksData.values[(int)upgladeType * 5 + barracksData.upgladeNums[(int)upgladeType]];
    }
    public string GetBarrackInfo(UpgradeInfoType infoType, BarracksUpgradeType upgladeType, int step)
    {
        string result = "";
        switch (infoType)
        {
            case (UpgradeInfoType.cost):
                result = barracksData.costs[(int)upgladeType * 4 + step].ToString();
                break;
            case (UpgradeInfoType.value):
                result = barracksData.values[(int)upgladeType * 5 + step+1].ToString();
                break;
            case (UpgradeInfoType.name):
                result = barracksData.typeStr[(int)upgladeType];
                break;
        }
        return result;
    }
    public int GetSpawnPoint()
    {
        return barracksData.values[(int)BarracksUpgradeType.unitPoint*5+barracksData.upgladeNums[2]];
    }
    public string GetBlacksmithInfo(UpgradeInfoType infoType,int iType, int step)
    {
        string result = "";
        switch (infoType)
        {
            case (UpgradeInfoType.cost):
                result = blacksmithData.costs[(int)iType * 4 + step].ToString();
                break;
            case (UpgradeInfoType.value):
                result = blacksmithData.values[(int)iType * 5 + step+1].ToString();
                break;
            case (UpgradeInfoType.name):
                result = blacksmithData.typeStr[(int)iType];
                break;
        }
        return result;
    }
    public HeroData GetRandomHeroData()
    {
        HeroData data= new HeroData();
        data.isDead = false;
        data.exp = 0;
        int rand=UnityEngine.Random.Range(0, 100);
        if (rand < 10)
            data.level = 2;
        else if (rand < 40)
            data.level = 1;
        else
            data.level = 0;
        int classCount=Enum.GetValues(typeof(HeroClass)).Length;
        int classNum= UnityEngine.Random.Range(0, classCount);
        data.heroClass = (HeroClass)classNum;
        data.level_Armor = data.level;
        data.level_Weapon = data.level;
        data.id = 0;
        data.level_Skills = new int[4];
        for(int i=0;i<data.level_Skills.Length; i++)
        {
            data.level_Skills[i] = data.level;
        }
        string[] names=new string[] {"레이널드", "디스마스","줄리아","바리스탄","리놈","리네시","호세","휴","오스먼드","베르트람","베릴","시보스" };
        data.name = names[UnityEngine.Random.Range(0, names.Length)];
        return data;
    }
    public void GenerateNewGuildList()
    {
        guildHeroDatas = new HeroDatas();
        guildHeroDatas.heroDataList = new List<HeroData>();
        for(int i=0; i<4; i++)
        {
            guildHeroDatas.heroDataList.Add(GetRandomHeroData());
        }
        SaveDataToJson(DataType.GuildData);
    }
    public void AddHeroDatasToMyHeroList(List<HeroData> dataList)
    {
        int id = myHeroDatas.heroDataList.Last().id+1;
        foreach(var data in dataList)
        {
            guildHeroDatas.heroDataList.Remove(data);
            data.id = id;
            myHeroDatas.heroDataList.Add(data);
            
            id++;
        }
        SaveDataToJson(DataType.GuildData);
        SaveDataToJson(DataType.HeroDatas);
        UIMgr.Instance.InitializeHeroCardList();
    }
}
