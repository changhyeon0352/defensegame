using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{ 
    Transform commandUiTr =null;
    List<Button> commandButtons;
    private void Awake()
    {
        commandUiTr = transform.Find("CommandUI");
        commandButtons = new List<Button>();
        InitializeUI();
    }
    void InitializeUI()
    {
        
        for (int i = 0; i < commandUiTr.childCount; i++)
        {
            commandButtons.Add(commandUiTr.GetChild(i).GetComponent<Button>());
            int index = i;
            commandButtons[i].onClick.AddListener(() => GameMgr.Instance.CommandMgr.UnitCommand(index));
        }

        //commandButtons[0].onClick.AddListener(()=>Test0(1));
        //commandButtons[1].onClick.AddListener(()=> GameMgr.Instance.CommandMgr.UnitCommand(1));
    }
    void Test0(int NumCommand)
    {
        GameMgr.Instance.CommandMgr.UnitCommand(NumCommand);
    }
    void Test1()
    {
        GameMgr.Instance.CommandMgr.UnitCommand(1);
    }
}
