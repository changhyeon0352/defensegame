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

    public void UpdateHp(int hp,int hpMax)
    {
        hpImage.fillAmount =(float)hp/(float)hpMax;
        hpTmp.text = $"{hp} / {hpMax}";
    }
    public void UpdateMp(int hp, int hpMax)
    {
        mpImage.fillAmount = (float)hp / (float)hpMax;
        mpTmp.text = $"{hp} / {hpMax}";
    }

    public void InitializeHeroState(HeroData heroData)
    {
        nameTmp.text=heroData.name;
        levelTmp.text = heroData.level.ToString();
        heroImage.sprite = UIMgr.Instance.GetHeroSprite(heroData);
        this.heroData = heroData;
        
    }
    public void SelectedHeroUI()
    {
        HeroState[] states = FindObjectsOfType<HeroState>();
        foreach(HeroState state in states)
        {
            state.selectedImage.enabled = false;
        }
        selectedImage.enabled = true;
    }

}
