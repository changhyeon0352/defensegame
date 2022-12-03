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
    public TextMeshProUGUI hpTmp;
    public TextMeshProUGUI mpTmp;

    private HeroData heroData;

    public void ShowHpMp(Hero hero)
    {
        hpImage.fillAmount =(float)hero.Hp/(float)hero.HpMax;
        mpImage.fillAmount = (float)hero.Mp / (float)hero.MpMax;
        hpTmp.text = $"{hero.Hp} / {hero.HpMax}";
        mpTmp.text = $"{hero.Mp} / {hero.MpMax}";
    }
    public void ShowHpMp(HeroData herodata)
    {
        ShowHpMp(Hero.FindHero(herodata));
    }
    

    public void InitializeHeroState(HeroData heroData)
    {
        nameTmp.text=heroData.name;
        levelTmp.text = heroData.level.ToString();
        heroImage.sprite = UIMgr.Instance.GetHeroSprite(heroData);
        this.heroData = heroData;
        Hero.FindHero(heroData).heroState = this;
        ShowHpMp(heroData);
    }
    public static void SelectedHeroUI()
    {
        HeroState[] heroStates = FindObjectsOfType<HeroState>();
        foreach(HeroState heroState in heroStates)
        {
            heroState.selectedImage.enabled = heroState.heroData == GameMgr.Instance.commandMgr.SelectedHero.Data;
        }
    }

}
