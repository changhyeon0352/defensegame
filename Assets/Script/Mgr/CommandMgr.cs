
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class CommandMgr : MonoBehaviour
{
    public GameObject[] skillIndicatorPrefabs;
    List<UnitGroup> seletedGroupList = null;
    PlayerInput inputActions;
    public List<UnitGroup> SelectedGroupList
    {
        get => seletedGroupList;
    }
    private void Awake()
    {
        inputActions = GameMgr.Instance.inputActions;
        seletedGroupList = new List<UnitGroup>();
    }
    private void OnEnable()
    {
        inputActions.Command.Enable();
        inputActions.Command.Click.performed += OnClick;
        
    }
    private void OnDisable()
    {
        inputActions.Command.Click.performed -= OnClick;
        inputActions.Command.Disable();
    }
    private void OnClick(InputAction.CallbackContext obj)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ShiftClickGroup();
        }
        else
        {
            ClickGroup();
        }
        //유닛 그룹에 스킬 교집합 체크 스킬넘버배열을 넘겨주자
        //스킬 사용가능여부 비트연산자 이용할것
        SkillAvailable groupsSkills = ~SkillAvailable.None;
        foreach(UnitGroup selectGroup in seletedGroupList)
        {
            groupsSkills &= selectGroup.GroupSkill;
        }
        if(seletedGroupList.Count>0)
        {
            GameMgr.Instance.uiMgr.SetButtonAvailable(groupsSkills);
        }
        
    }

    public void ClearSelect()
    {
        seletedGroupList.Clear();
        GameMgr.Instance.uiMgr.ClearSkillButton();
    }
    private void ClickGroup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
        {
            ClearSelect();
            UnitGroup unitGroup = hit.transform.parent.GetComponent<UnitGroup>();
            if (unitGroup != null)
            {
                seletedGroupList.Add(unitGroup);
            }
        }
        else
        {
            ClearSelect();
        }
        
        AllCheckSelected();
    }
    private void ShiftClickGroup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
        {
            UnitGroup unitGroup = hit.transform.parent.GetComponent<UnitGroup>();
            if (unitGroup != null)
            {
                if (seletedGroupList.Contains(unitGroup))
                {
                    seletedGroupList.Remove(unitGroup);
                }
                else
                {

                    seletedGroupList.Add(unitGroup);
                }
                AllCheckSelected();
            }
        }
    }
    public void AllCheckSelected()
    {
        UnitGroup[] unitGroups = FindObjectsOfType<UnitGroup>(); 
        foreach(UnitGroup unitGroup in unitGroups)
        {
            unitGroup.CheckSelected();
        }
    }
    //커맨드 함수==============================

    public void UnitCommand(int command)
    {
        UnitCommand((SkillAvailable)command);
    }
    public void UnitCommand(SkillAvailable command )
    {
        foreach (UnitGroup unitGroup in seletedGroupList)
        {
            AllyUnit[] units = unitGroup.GetComponentsInChildren<AllyUnit>();
            foreach (AllyUnit unit in units)
            {
                switch (command)
                {
                    case(SkillAvailable.MoveToSpot):
                        unit.ChangeState(UnitState.Move);
                        break;
                    case (SkillAvailable.Charge):
                        unit.ChargeToEnemy();
                        break;
                }
            }
        }
    }
}
