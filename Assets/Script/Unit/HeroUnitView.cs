using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroUnitView : MonoBehaviour
{
    SkillbarUI skillbarUI;
    public void MakeHeroStateUI(HeroData heroData)
    {
        Transform heroStateParent = UIMgr.Instance.DefenseUITr.Find("HeroStateList");
    }
    public void ChageSkillbar(HeroUnit hero)
    {
        if(skillbarUI==null)
            skillbarUI = FindObjectOfType<SkillbarUI>();
        skillbarUI.ChangeHeroSkill(hero);
    }
}
