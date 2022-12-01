using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIMgr : Singleton<UIMgr>
{

    [SerializeField] Texture2D cursorDefault;
    [SerializeField] Texture2D cursorSword;
    [SerializeField] Texture2D cursorTargeting;
    [SerializeField] Texture2D cursorFindTarget;

    public SkillbarUI skillbarUI;
    public HPbar hpbar;
    public Sprite[] heroSprites;
    public GameObject heroCardPrefab;
    Transform commandUiTr =null;
    Transform spawnUiTr=null;
    Transform defenseStart = null;
    Transform settingTr=null;
    Transform commandTr = null;
    Transform HeroSettingTr=null;
    List<Button> commandButtons=new();
    List<Button> spawnButtons=new();
    List<Image> commandImages = new();
    List<Image> spawnImages = new();
    HeroSlot[] heroSlots;
    [SerializeField]
    TextMeshProUGUI maxSpawnPoint;
    [SerializeField]
    TextMeshProUGUI nowSpawnPoint;
    public GameObject heroStatePrefab;


    public HeroSlot[] HeroSlots { get => heroSlots; }



    override protected void Awake()
    {
        base.Awake();
        commandTr = transform.Find("Command");
        commandUiTr = commandTr.Find("CommandUI");
        settingTr = transform.Find("Setting");
        spawnUiTr = settingTr.Find("SpawnUnitButton");
        defenseStart = settingTr.Find("defenseStartButton");
        HeroSettingTr = transform.Find("HeroSetting");
        GameMgr.Instance.actionChangePhase += ChangePhase;
        
    }
    private void Start()
    {
    }
    void InitializeUI(Phase phase)
    {
        switch(phase)
        {
            case Phase.town:

                break;
            case Phase.selectHero:
                InitializeHeroCardList();
                break;
            case Phase.setting:
                for (int i = 0; i < spawnUiTr.childCount; i++)
                {
                    spawnImages.Add(spawnUiTr.GetChild(i).GetComponent<Image>());
                    spawnButtons.Add(spawnUiTr.GetChild(i).GetComponent<Button>());
                    int index = i;
                    spawnButtons[i].onClick.AddListener(() => GameMgr.Instance.settingMgr.SelectSpawnUnitType(index));
                    spawnButtons[i].onClick.AddListener(() => SelectedSpawnButton(index));
                }
                break;
            case Phase.defense:
                for (int i = 0; i < commandUiTr.childCount; i++)
                {
                    
                    commandImages.Add(commandUiTr.GetChild(i).GetComponent<Image>());
                    commandButtons.Add(commandUiTr.GetChild(i).GetComponent<Button>());
                    int index = (int)Mathf.Pow(2, i);
                    commandButtons[i].onClick.AddListener(() => GameMgr.Instance.commandMgr.UnitCommand(index));
                }
                MakeHeroStates();
                ClearSkillButton();
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
        if(cursor==CursorType.Sword)
        {
            Cursor.SetCursor(cursorSword,new Vector2(cursorSword.width*0.25f,0), CursorMode.ForceSoftware);
        }
        else if(cursor == CursorType.Default)
        {
            Cursor.SetCursor(cursorDefault, new Vector2(cursorDefault.width*0.3f,cursorDefault.height*0.1f), CursorMode.ForceSoftware);
        }
        else if(cursor == CursorType.targeting)
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
        Transform heroCardListTr = HeroSettingTr.Find("HeroList");
        List<HeroData> HeroDataList = HeroDataController.Instance.heroDatas.heroDataList;
        foreach(HeroData heroData in HeroDataList)
        {
            Transform cardTr=Instantiate(heroCardPrefab, heroCardListTr).transform;
            cardTr.GetComponent<HeroCard>().InitializeCard(heroData);
        }
    }
    public Sprite GetHeroSprite(HeroData data)
    {
        return heroSprites[(int)data.heroClass];
    }
    public void MakeHeroStates()
    {
        Transform heroListTr =commandTr.Find("HeroList");
        foreach(HeroData heroData in DataMgr.Instance.FightingHeroDataList)
        {
            HeroState heroState = Instantiate(heroStatePrefab, heroListTr).GetComponent<HeroState>();
            heroState.InitializeHeroState(heroData);
        }
    }
    public void SelectedSpawnButton(int index)
    {
        for(int i= 0; i < spawnImages.Count; i++)
        {
            if(i==index)
            {
                spawnImages[i].color = Color.yellow;
            }
            else
            {
                spawnImages[i].color = Color.white;
            }
        }
    }
    private void ChangePhase(Phase _phase)
    {
        switch (GameMgr.Instance.Phase)
        {
            case Phase.town:
                break;
            case Phase.selectHero:
                HeroSettingTr.gameObject.SetActive(false);
                heroSlots = null;
                break;
            case Phase.setting:
                settingTr.gameObject.SetActive(false);
                break;
            case Phase.defense:
                commandTr.gameObject.SetActive(false);
                break;
        }
        switch (_phase)
        {
            case Phase.town:
                break;
            case Phase.selectHero:
                HeroSettingTr.gameObject.SetActive(true);
                heroSlots = GetComponentsInChildren<HeroSlot>();
                InitializeUI(_phase);
                break;
            case Phase.setting:
                settingTr.gameObject.SetActive(true);
                InitializeUI(_phase);
                break;
            case Phase.defense:
                commandTr.gameObject.SetActive(true);
                InitializeUI(_phase);
                break;
        }
    }
    public void UpdateSpawnPoint(int nowPoint,int maxPoint)
    {
        nowSpawnPoint.text = nowPoint.ToString();
        maxSpawnPoint.text = maxPoint.ToString();
    }
}
