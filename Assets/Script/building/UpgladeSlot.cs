using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgladeSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public Sprite[] upgladeCheckSprite;
    int typeNum;  
    int stepNum;
    string toolTipStr;
    int cost;
    public HeroData heroData;
    Image image;
    public int StepNum { get => stepNum; }
    public string ToolTipStr { get => toolTipStr; }
    bool isUpgradable=false;
    BuildingType buildingType;
    private void Awake()
    {
        image= GetComponent<Image>(); 
    }
    public void Initialize(int _typeNum,int _stepNum, string _toolTipStr, int spriteNum,int _cost, BuildingType buildingType,bool Upgradable=false)
    {
        typeNum = _typeNum;
        stepNum = _stepNum;
        toolTipStr = _toolTipStr;
        image.sprite = upgladeCheckSprite[spriteNum];
        cost = _cost;
        isUpgradable = Upgradable;
        this.buildingType = buildingType;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIMgr.Instance.OpenToolTip(transform,ToolTipStr);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIMgr.Instance.CloseToolTip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isUpgradable&&cost<=DataMgr.Instance.Money&&image.sprite== upgladeCheckSprite[1])
        {
            if(buildingType==BuildingType.Barracks)
            {
                DataMgr.Instance.UpgradeBarracks(typeNum, cost);
                FindObjectOfType<Barracks>().InitBarracks();
            }
            else if(buildingType==BuildingType.Blacksmith)
            {
                DataMgr.Instance.UpgradeBlacksmith(heroData,typeNum, cost);
                FindObjectOfType<Blacksmith>().InitBlacksmithHeroInfo();
                UIMgr.Instance.InitializeHeroCardList(UIMgr.Instance.HeroListTr,DataMgr.Instance.MyHeroDatas);
            }
            
        }
    }
}
