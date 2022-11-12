using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : Singleton<UIMgr>
{
    [SerializeField] Texture2D cursorDefault;
    [SerializeField] Texture2D cursorSword;
    [SerializeField] Texture2D cursorTargeting;
    Transform commandUiTr =null;
    Transform spawnUiTr=null;
    Transform defenseStart = null;
    List<Button> commandButtons=new();
    List<Button> spawnButtons=new();
    List<Image> commandImages = new();
    List<Image> spawnImages = new();


    override protected void Awake()
    {
        base.Awake();
        commandUiTr = transform.Find("CommandUI");
        spawnUiTr = transform.Find("SpawnUI");
        defenseStart = transform.Find("defenseStartButton");
        InitializeUI();
    }
    void InitializeUI()
    {
        for (int i = 0; i < spawnUiTr.childCount; i++)
        {
            spawnImages.Add(spawnUiTr.GetChild(i).GetComponent<Image>());
            spawnButtons.Add(spawnUiTr.GetChild(i).GetComponent<Button>());
            int index = i;
            spawnButtons[i].onClick.AddListener(() => SettingMgr.Instance.SelectSpawnUnitType(index));
        }
        for (int i = 0; i < commandUiTr.childCount; i++)
        {
            commandImages.Add(commandUiTr.GetChild(i).GetComponent<Image>());
            commandButtons.Add(commandUiTr.GetChild(i).GetComponent<Button>());
            int index =(int) Mathf.Pow(2, i);
            commandButtons[i].onClick.AddListener(() => CommandMgr.Instance.UnitCommand(index));
        }
        defenseStart.GetComponent<Button>().onClick.AddListener(DefenseStart);
        ClearSkillButton();
    }
    public void SetButtonAvailable(Skills groupsSkills)
    {
        ClearSkillButton();
        int temp = (int)groupsSkills;
        for (int i = 0; i < commandButtons.Count; i++)     //선택된건 쓸 수 있게
        {
            if ((temp & 1) == 1)
            {
                commandButtons[i].enabled = true;
                commandImages[i].color = Color.white;
            }
            temp >>= 1;
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
    public void DefenseStart()
    {
        GameMgr.Instance.inputActions.Setting.Disable();
        GameMgr.Instance.inputActions.Command.Enable();
        for(int i=0; i<spawnButtons.Count; i++)
        {
            spawnButtons[i].gameObject.SetActive(false);
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
        else
        {
            Cursor.SetCursor(cursorTargeting, new Vector2(cursorTargeting.width * 0.5f, cursorTargeting.height * 0.5f), CursorMode.ForceSoftware);
        }
    }
}
