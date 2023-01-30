using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guild : MonoBehaviour
{
    [SerializeField]
    private Transform heroListTr;
    public Transform HeroListTr { get { return heroListTr; } }
    private List<HeroData> heroDataList=new();
    private void Start()
    {
        UIMgr.Instance.InitializeHeroCardList(heroListTr, DataMgr.Instance.GuildHeroDatas);
    }
    public void AddHeroData(HeroData data)
    {
        heroDataList.Add(data);
    }
    public void RemoveHeroData(HeroData data)
    {
        heroDataList.Remove(data);
    }
    public void CompleteAddHeroData()
    {
        DataMgr.Instance.AddHeroDatasToMyHeroList(heroDataList);
    }
    public bool IsContainData(HeroData data)
    {
        return heroDataList.Contains(data); 
    }
}
