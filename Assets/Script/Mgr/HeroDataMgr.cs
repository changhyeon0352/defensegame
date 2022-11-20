using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDataMgr : Singleton<HeroDataMgr>
{
    private List<HeroData> fightingHeroDataList = new();
    public List<HeroData> FightingHeroDataList { get { return fightingHeroDataList; } }

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
