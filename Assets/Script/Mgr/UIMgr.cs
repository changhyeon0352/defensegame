using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{ 
    Transform commandUiTr =null;
    Transform spawnUiTr=null;
    List<Button> commandButtons;
    List<Image> commandImages;
    private void Awake()
    {
        commandUiTr = transform.Find("CommandUI");
        spawnUiTr = transform.Find("SpawnUI");
        commandButtons = new List<Button>();
        commandImages = new();
        InitializeUI();
    }
    void InitializeUI()
    {
        for (int i = 0; i < spawnUiTr.childCount; i++)
        {
            int index = i;
            spawnUiTr.GetChild(i).GetComponent<Button>().onClick.AddListener(() => GameMgr.Instance.settingMgr.SelectSpawnUnitType(index));
        }
        for (int i = 0; i < commandUiTr.childCount; i++)
        {
            commandImages.Add(commandUiTr.GetChild(i).GetComponent<Image>());
            commandButtons.Add(commandUiTr.GetChild(i).GetComponent<Button>());
            int index = i;
            commandButtons[i].onClick.AddListener(() => GameMgr.Instance.CommandMgr.UnitCommand(index));
        }
        ClearSkillButton();
    }
    public void SetButtonAvailable(SkillAvailable groupsSkills)
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
}
