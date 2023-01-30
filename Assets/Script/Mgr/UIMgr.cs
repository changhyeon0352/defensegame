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
    public UnitStat unitStatUI;
    public SkillbarUI skillbarUI;
    public HPbar hpbar;
    public Sprite[] heroSprites;
    public GameObject heroCardPrefab;
    Transform UnitCommandTr = null;
    Transform spawnUiTr = null;
    Transform defenseStart = null;
    Transform DeploymentTr = null;
    Transform defenseTr = null;
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
        GameMgr.Instance.actionChangePhase += ChangePhase;
        

    }
    private void Update()
    {
        
        
    }

    void InitializeUI(Phase phase)
    {
        switch (phase)
        {
            case Phase.town:

                break;
            case Phase.selectHero:
                
                break;
            case Phase.setting:
                
                break;
            case Phase.defense:
                
                break;
        }
    }
    public void SetButtonAvailable(BasicSkills groupsSkills)
    {
        ClearSkillButton();
        int temp = (int)groupsSkills;
        for (int i = 0; i < commandButtons.Count; i++)     //선택된건 쓸 수 있게
        {
            if ((temp & 1) == 1)//끝자리가 1인지 체크함 이진수 11101 이런거 체크
            {
                commandButtons[i].enabled = true;
                commandImages[i].color = Color.white;
            }
            temp >>= 1; // 1101 => 110/1 이렇게 민다
        }
    }

    public void ClearSkillButton()
    {
        for (int i = 0; i < commandButtons.Count; i++) //모두 못쓰게
        {
            commandButtons[i].enabled = false;
            commandImages[i].color = Color.gray;
        }
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
        Transform heroStateListTr = defenseTr.Find("HeroStateList");
        foreach (HeroData heroData in DataMgr.Instance.FightingHeroDataList)
        {
            HeroState heroState = Instantiate(heroStatePrefab, heroStateListTr).GetComponent<HeroState>();
            heroState.InitializeHeroState(heroData);
        }
    }
    public void SelectedSpawnButton(int index)
    {
        int unitCost = GameMgr.Instance.settingMgr.SelectSpawnUnitType(index);
        for (int i = 0; i < spawnImages.Count; i++)
        {
            if (i == index)
            {
                spawnImages[i].transform.localScale = Vector3.one * 1.1f;
                spawnImages[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = unitCost.ToString();
            }
            else
            {
                spawnImages[i].transform.localScale = Vector3.one * 1f;
                spawnImages[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
    private void ChangePhase(Phase _phase)
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
            case Phase.setting:
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
            case Phase.setting:
                DeploymentTr.gameObject.SetActive(true);
                screenMover.SetActive(true);
                for (int i = 0; i < spawnUiTr.childCount; i++)
                {
                    spawnImages.Add(spawnUiTr.GetChild(i).GetComponent<Image>());
                    spawnButtons.Add(spawnUiTr.GetChild(i).GetComponent<Button>());
                    int index = i;
                    spawnButtons[i].onClick.AddListener(() => SelectedSpawnButton(index));
                }
                break;
            case Phase.defense:
                defenseTr.gameObject.SetActive(true);
                for (int i = 0; i < UnitCommandTr.childCount; i++)
                {

                    commandImages.Add(UnitCommandTr.GetChild(i).GetChild(0).GetComponent<Image>());
                    commandButtons.Add(UnitCommandTr.GetChild(i).GetComponent<Button>());
                    int index = (int)Mathf.Pow(2, i);
                    commandButtons[i].onClick.AddListener(() => GameMgr.Instance.defenseMgr.UnitCommand(index));
                }
                MakeHeroStates();
                ClearSkillButton();
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
        gameQuitWindow.SetActive(!gameQuitWindow.active);

    }
    public void ChangeBuildingNameColor(BuildingType buildingType,Color color)
    {
        TextMeshProUGUI buildingName = buildingNamesTr.GetChild((int)buildingType).GetComponent<TextMeshProUGUI>();
        buildingName.color=color;

    }
}
