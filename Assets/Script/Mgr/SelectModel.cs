using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectModel : MonoBehaviour
{
    private List<UnitGroup> groups = new List<UnitGroup>();
    private List<HeroUnit> heroes = new List<HeroUnit>();
   

    public void SelectHero(int n)
    {
        HeroUnit hero=heroes[n];    
        if(!hero.IsDead)
        {
            //ClearSelectedGroups(); 
            //SelectedHero = hero;
            UIMgr.Instance.skillbarUI.ChangeHeroSkill(hero);
            
        }
    }
}
