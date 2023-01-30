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
        if(GameMgr.Instance.Phase==Phase.selectHero&&DataMgr.Instance.FightingHeroDataList.Count<4&&!isSelected)
        {
            AddHeroToHeroSolts(heroData);
            AddorRemoveFromList(true);
        }
        if(GameMgr.Instance.Phase==Phase.town)
        {
            Blacksmith blacksmith = FindObjectOfType<Blacksmith>();
            Guild guild = FindObjectOfType<Guild>();
            if (blacksmith != null)
            {
                HeroSlot slot = FindObjectOfType<HeroSlot>();
                if (slot != null)
                {
                    slot.GiveheroDataToSlot(heroData);
                    slot.heroDataCard = this;
                    blacksmith.InitBlacksmithHeroInfo();
                }
            }
            else if(guild!=null)
            {
                bool isContain=guild.IsContainData(heroData);
                heroImage.color = isContain ? Color.white : Color.gray;
                heroImage.transform.GetChild(0).GetComponent<Image>().enabled = !isContain;//참전표시
                if (!isContain)
                    guild.AddHeroData(heroData);
                else
                    guild.RemoveHeroData(heroData);
            }
        }
    }
    public void AddHeroToHeroSolts(HeroData heroData)
    {
        HeroSlot[] heroSlots=UIMgr.Instance.HeroSlots;
        for (int i = 0; i < heroSlots.Length; i++)//왼쪽부터 보고 비어있는 슬롯에 넣음
        {
            HeroData data1 = heroSlots[i].Data;
            if (data1 == null)
            {
                heroSlots[i].GiveheroDataToSlot(heroData);
                heroSlots[i].heroDataCard = this;
                break;
            }

        }
    }
    public void AddorRemoveFromList(bool isFight)
    {
        heroImage.color = isFight ? Color.gray : Color.white;
        heroImage.transform.GetChild(0).GetComponent<Image>().enabled = isFight;//참전표시
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
