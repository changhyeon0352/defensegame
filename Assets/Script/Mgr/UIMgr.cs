using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIMgr : Singleton<UIMgr>
{

    [SerializeField] Texture2D cursorDefault;
    [SerializeField] Texture2D cursorSword;
    [SerializeField] Texture2D cursorTargeting;
    [SerializeField] Texture2D cursorFindTarget;
    public GameObject skilldurationUI;
    public UnitStatUI unitStatUI;
    public SkillbarUI skillbarUI;
    public HPbars hpbar;
    public Sprite[] heroSprites;
    public GameObject heroCardPrefab;
    Transform UnitCommandTr = null;
    Transform spawnUiTr = null;
    Transform defenseStart = null;
    Transform DeploymentTr = null;
    Transform defenseTr = null;
    public Transform DefenseUITr { get => defenseTr; }
    Transform HeroSelectingTr = null;
    GameObject screenMover = null;
    List<Button> commandButtons = new();
    List<Button> spawnButtons = new();
    List<Image> commandImages = new();
    List<Image> spawnImages = new();
    HeroSlot[] heroSlots;
    [SerializeField]
    TextMeshProUGUI maxSpawnPoint;
    [SerializeField]
    TextMeshProUGUI nowSpawnPoint;
    public GameObject heroStatePrefab;
    [SerializeField]
    GameObject tooltipObj;
    [SerializeField]
    TextMeshProUGUI moneyText;
    [SerializeField]
    GameObject townUI;
    [SerializeField]
    Image timerImg;
    [SerializeField]
    GameObject defenseResult;
    [SerializeField]
    private GameObject[] buildings;
    [SerializeField]
    private GameObject heroList;
    [SerializeField]
    private GameObject gameQuitWindow;
    [SerializeField]
    Transform buildingNamesTr;
    CursorType nowCursorType;
    public Transform HeroListTr { get => heroList.transform; }

    public HeroSlot[] HeroSlots { get => heroSlots; }
    



    override protected void Awake()
    {
        base.Awake();
        defenseTr = transform.Find("Defense");
        UnitCommandTr = defenseTr.Find("UnitCommand");
        DeploymentTr = transform.Find("Deployment");
        spawnUiTr = DeploymentTr.Find("SpawnUnitButton");
        defenseStart = DeploymentTr.Find("defenseStartButton");
        HeroSelectingTr = transform.Find("HeroSelecting");
        screenMover = transform.Find("screenMover").gameObject;
        

    }

    
    

    public void ChangeCursor(CursorType cursor)
    {
        if (cursor == CursorType.Sword)
        {
            Cursor.SetCursor(cursorSword, new Vector2(cursorSword.width * 0.25f, 0), CursorMode.ForceSoftware);
        }
        else if (cursor == CursorType.Default)
        {
            Cursor.SetCursor(cursorDefault, new Vector2(cursorDefault.width * 0.3f, cursorDefault.height * 0.1f), CursorMode.ForceSoftware);
        }
        else if (cursor == CursorType.targeting)
        {
            Cursor.SetCursor(cursorTargeting, new Vector2(cursorTargeting.width * 0.5f, cursorTargeting.height * 0.5f), CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(cursorFindTarget, new Vector2(cursorTargeting.width * 0.5f, cursorTargeting.height * 0.5f), CursorMode.ForceSoftware);
        }
        nowCursorType = cursor;
    }
    public void CursorOnMonster(bool isEnter)
    {
        if((nowCursorType==CursorType.Default||nowCursorType==CursorType.findTarget)&&isEnter)
        {
            ChangeCursor((CursorType)((int)nowCursorType + 1));
        }
        else if((nowCursorType == CursorType.Sword || nowCursorType == CursorType.targeting)&& !isEnter)
        {
            ChangeCursor((CursorType)((int)nowCursorType + -1));
        }
    }

    public void InitializeHeroCardList()
    {
        InitializeHeroCardList(heroList.transform, DataMgr.Instance.MyHeroDatas);
        Guild guild = FindObjectOfType<Guild>();
        if( guild!= null)
        {
            InitializeHeroCardList(guild.HeroListTr,DataMgr.Instance.GuildHeroDatas);
        }
            
    }
    public void InitializeHeroCardList(Transform heroListTr,HeroDatas heroDatas)
    {
        var child = heroListTr.GetComponentsInChildren<HeroCard>();

        foreach (var iter in child)
        {
                Destroy(iter.gameObject);
        }
        foreach (HeroData heroData in heroDatas.heroDataList)
        {
            if(!heroData.isDead)
            {
                Transform cardTr = Instantiate(heroCardPrefab, heroListTr).transform;
                cardTr.GetComponent<HeroCard>().InitializeCard(heroData);
            }
        }
    }
    public Sprite GetHeroSprite(HeroData data)
    {
        return heroSprites[(int)data.heroClass];
    }
    public void MakeHeroStates()
    {
        HeroUnit[] heros=FindObjectsOfType<HeroUnit>();
        foreach(HeroUnit hero in heros)
        {
            hero.MakeHeroStateUI();
        }
    }
    public void MakeUnitsHpbar()
    {
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            unit.InitUnitHpbar();
        }
    }

    public void ChangePhase(Phase _phase)
    {
        switch (GameMgr.Instance.Phase)
        {
            case Phase.town:
                townUI.SetActive(false);
                break;
            case Phase.selectHero:
                HeroSelectingTr.gameObject.SetActive(false);
                heroSlots = null;
                heroList.SetActive(false);
                break;
            case Phase.Deployment:
                DeploymentTr.gameObject.SetActive(false);
                break;
            case Phase.defense:
                defenseTr.gameObject.SetActive(false);
                break;
            case Phase.result:
                defenseResult.SetActive(false);
                break;
        }
        switch (_phase)
        {
            case Phase.town:
                heroList.SetActive(true);
                townUI.SetActive(true);
                InitializeHeroCardList(heroList.transform,DataMgr.Instance.MyHeroDatas);
                break;
            case Phase.selectHero:
                heroList.SetActive(true);
                HeroSelectingTr.gameObject.SetActive(true);
                heroSlots = GetComponentsInChildren<HeroSlot>();
                //InitializeHeroCardList();
                break;
            case Phase.Deployment:
                DeploymentTr.gameObject.SetActive(true);
                screenMover.SetActive(true);
                for (int i = 0; i < spawnUiTr.childCount; i++)
                {
                    spawnImages.Add(spawnUiTr.GetChild(i).GetComponent<Image>());
                    spawnButtons.Add(spawnUiTr.GetChild(i).GetComponent<Button>());
                    int index = i;
                    //spawnButtons[i].onClick.AddListener(() => SelectSpawnUnitUI(index)); 미수정
                }
                break;
            case Phase.defense:
                defenseTr.gameObject.SetActive(true);
                
                MakeHeroStates();
                MakeUnitsHpbar();
                //ClearSkillButton();
                break;
            case Phase.result:
                defenseResult.SetActive(true);
                break;
        }
    }
    public void UpdateSpawnPoint(int nowPoint, int maxPoint)
    {
        nowSpawnPoint.text = nowPoint.ToString();
        maxSpawnPoint.text = maxPoint.ToString();
    }
    public void ShowDuration(string name, float sec)
    {
        Instantiate(skilldurationUI, transform).GetComponent<SkillDuration>().InitSkillDurationUI(name, sec);
    }
    
    public void OpenToolTip(Transform tr,string str)
    {
        tooltipObj.SetActive(true);
        tooltipObj.GetComponentInChildren<TextMeshProUGUI>().text = str;
        tooltipObj.transform.position = tr.position;
    }
    public void CloseToolTip()
    {
        tooltipObj.SetActive(false);
    }
    public void ChangeMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
    public void ShowTimer(float timePercent)
    {
        timerImg.fillAmount = timePercent;
    }
    public void OpenBuildingsUI(int buildingType)
    {
        buildings[buildingType].SetActive(true);
    }
    public void CloseBuildingsUI(int buildingType)
    {
        buildings[buildingType].SetActive(false);

    }
    public void ToggleGameQuitWindow()
    {
        gameQuitWindow.SetActive(!gameQuitWindow.activeSelf);

    }
    public void ChangeBuildingNameColor(BuildingType buildingType,Color color)
    {
        TextMeshProUGUI buildingName = buildingNamesTr.GetChild((int)buildingType).GetComponent<TextMeshProUGUI>();
        buildingName.color=color;

    }
}
