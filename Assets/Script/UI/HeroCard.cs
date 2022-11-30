using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroCard : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    private HeroData heroData;
    public Image heroImage;
    public Sprite[] heroSprites;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI weaponTMP;
    public TextMeshProUGUI armorTMP;
    public TextMeshProUGUI levelTMP;
    public Image levelgage;
    private RectTransform rect;
    bool isSelected = false;

    public void InitializeCard(HeroData heroData)
    {
        this.heroData = heroData;
        heroImage.sprite = heroSprites[(int)heroData.heroClass];
        nameTMP.text = heroData.name;
        weaponTMP.text = heroData.level_Weapon.ToString();
        armorTMP.text = heroData.level_Armor.ToString();
        levelTMP.text = heroData.level.ToString();
        levelgage.fillAmount = 0.5f;
        rect = transform.GetChild(0).GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(DataMgr.Instance.FightingHeroDataList.Count<4&&!isSelected)
        {
            AddHeroToHeroSolts(heroData);
            AddorRemoveFromList(true);
        }
    }
    public void AddHeroToHeroSolts(HeroData heroData)
    {
        HeroSlot[] heroSlots=UIMgr.Instance.HeroSlots;
        for (int i = 0; i < heroSlots.Length; i++)
        {
            HeroData data1 = heroSlots[i].Data;
            if (data1 == null)
            {
                heroSlots[i].Data = heroData;
                heroSlots[i].heroDataCard = this;
                break;
            }

        }
    }
    public void AddorRemoveFromList(bool isFight)
    {
        heroImage.color = isFight ? Color.gray : Color.white;
        heroImage.transform.GetChild(0).GetComponent<Image>().enabled = isFight;
        isSelected = isFight;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(MoveHeroCard(rect, Vector2.left));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(MoveHeroCard(rect, Vector2.right));
    }
    IEnumerator MoveHeroCard(RectTransform rect,Vector2 dir)
    {
        int repeat = 0;
        while(repeat<25)
        {
            rect.Translate(dir*2);
            yield return null;
            repeat++;
        }
    }





    //TextMeshProUGUI nameTMP = cardTr.Find("ImageName").GetComponentInChildren<TextMeshProUGUI>();
    //TextMeshProUGUI weaponTMP = cardTr.Find("equipmentLevel").Find("LevelWeapon").GetComponent<TextMeshProUGUI>();
    //TextMeshProUGUI armorTMP = cardTr.Find("equipmentLevel").Find("LevelArmor").GetComponent<TextMeshProUGUI>();


}
