using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitCommandUI : MonoBehaviour
{
    private Button[] commandButtons;
    private Image[] commandImages;
    private void Awake()
    {
        commandButtons= GetComponentsInChildren<Button>();
        commandImages=new Image[commandButtons.Length];
        UnitController unitController = FindObjectOfType<UnitController>();
        for(int i=0;i<commandButtons.Length;i++)
        {
            commandImages[i]= commandButtons[i].image;
            SoldierSkill soldierSkill = (SoldierSkill)(1 << i);
            commandButtons[i].onClick.AddListener(() =>unitController.CommandToUnitGroup(soldierSkill));
        }

    }
    public void UpdateUnitCommandButton(SoldierSkill groupsSkills)
    {
        int temp = (int)groupsSkills;
        for (int i = 0; i < commandButtons.Length; i++)     //선택된건 쓸 수 있게
        {
            if ((temp & 1) == 1)//끝자리가 1인지 체크함 이진수 11101 이런거 체크
            {
                commandButtons[i].enabled = true;
                commandImages[i].color = Color.white;
            }
            else
            {
                commandButtons[i].enabled = false;
                commandImages[i].color = Color.gray;
            }
            temp >>= 1; // 1101 => 110/1 이렇게 민다
        }
    }
}
