using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroState : MonoBehaviour
{
    public TextMeshProUGUI nameTmp;
    public TextMeshProUGUI levelTmp;
    public Image hpImage;
    public Image mpImage;
    public Image heroImage;
    public Image selectedImage;
    private HeroData heroData;
    public void Hp(int hp,int hpMax)
    {
        hpImage.fillAmount =(float)hp/(float)hpMax;
    }
    public void Mp(int mp, int mpMax)
    {
        hpImage.fillAmount = (float)mp / (float)mpMax;
    }
    public void InitializeHeroState(HeroData heroData)
    {
        nameTmp.text=heroData.name;
        levelTmp.text = heroData.level.ToString();
        heroImage.sprite = UIMgr.Instance.GetHeroSprite(heroData);
        this.heroData = heroData;
        Hero.FindHero(heroData).heroState = this;
    }
    public static void SelectedHeroUI()
    {
        HeroState[] heroStates = FindObjectsOfType<HeroState>();
        foreach(HeroState heroState in heroStates)
        {
            heroState.selectedImage.enabled = heroState.heroData == GameMgr.Instance.commandMgr.SelectedHero.data;
        }
    }

}
