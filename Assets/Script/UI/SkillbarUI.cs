using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillbarUI : MonoBehaviour
{
    public Sprite[] knightSkillSprites;
    public Sprite[] mageSkillSprites;
    public Image[] skillImages;
    public Image[] skillCoolImages;
    HeroUnit selectedHero;


    public void ChangeHeroSkill(HeroUnit hero)
    {
        selectedHero=hero;
        switch(hero.HeroData.heroClass)
        {
            case HeroClass.Knight:
                for(int i=0;i<4;i++)
                {
                    skillImages[i].sprite=knightSkillSprites[i];
                }
                break;
            case HeroClass.Mage:
                for (int i = 0; i < 4; i++)
                {
                    skillImages[i].sprite = mageSkillSprites[i];
                }
                break;
        }
    }
    
    private void Update()
    {
        if (selectedHero == null)
            return;
        for(int i=0;i<4; i++)
        {
            if (!selectedHero.SkillCanUse[i])
            {
                skillCoolImages[i].fillAmount = selectedHero.SkillCoolLeft[i] / selectedHero.SkillCools[i];
            }
            else
            {
                skillCoolImages[i].fillAmount = 0;
            }
        }
        
    }
}
