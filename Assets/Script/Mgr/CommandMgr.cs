
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



public class CommandMgr : MonoBehaviour
{
    public GameObject[] skillIndicatorPrefabs;
    List<UnitGroup> seletedGroupList = null;
    PlayerInput inputActions;
    public GameObject[] skillPrefabs;
    Transform skillIndicatorTr;
    Transform[] skillTargets;
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
        inputActions.Command.Select.performed += OnSelect;
        inputActions.Command.skillClick.performed += OnSkillClick;
        
        inputActions.Command.skillClick.Disable();
    }

    private void OnSkillClick(InputAction.CallbackContext obj)
    {
        int selectedUnitNum = 0;


        UnitCommand(SkillAvailable.Shoot);
        Destroy(skillIndicatorTr.gameObject);
        inputActions.Command.Select.Enable();
        inputActions.Command.skillClick.Disable();
    }

    private void OnDisable()
    {
        inputActions.Command.skillClick.performed -= OnSkillClick;
        inputActions.Command.Select.performed -= OnSelect;
        inputActions.Command.Disable();

    }
    private void Update()
    {
        //q,e로 회전가능 휠로 크기조절 가능 궁수숫자만큼 target생성
        if(skillIndicatorTr!=null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
            {
                skillIndicatorTr.position=hit.point;
            }
        }
    }
    private void OnSelect(InputAction.CallbackContext obj)
    {
        if(!EventSystem.current.IsPointerOverGameObject())
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
            foreach (UnitGroup selectGroup in seletedGroupList)
            {
                groupsSkills &= selectGroup.GroupSkill;
            }
            if (seletedGroupList.Count > 0)
            {
                GameMgr.Instance.uiMgr.SetButtonAvailable(groupsSkills);
            }
        }
    }

    public void ClearSelect()
    {
        seletedGroupList.Clear();
        GameMgr.Instance.uiMgr.ClearSkillButton();
    }
    private void ClickGroup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f,LayerMask.GetMask("Ally")))
        {
            ClearSelect();
            UnitGroup unitGroup = hit.transform.parent.parent.GetComponent<UnitGroup>();
            if (unitGroup != null)
            {
                seletedGroupList.Add(unitGroup);
            }        }
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
            UnitGroup unitGroup = hit.transform.parent.parent.GetComponent<UnitGroup>();
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

    //스킬=================================================
    public void SelectShotSpot()
    {
        inputActions.Command.Select.Disable();
        inputActions.Command.skillClick.Enable();
        foreach(var group in seletedGroupList)
        {
            Instantiate(group.spots);
        }
        Instantiate(skillIndicatorPrefabs[0]);
        
    }
    
    //커맨드 함수==============================

    public void UnitCommand(int command)
    {
        UnitCommand((SkillAvailable)command);
    }
    public void UnitCommand(SkillAvailable command )
    {
        Debug.Log("스킬사용");
        foreach (UnitGroup unitGroup in seletedGroupList)
        {
            AllyUnit[] units;
            units = unitGroup.GetComponentsInChildren<AllyUnit>();
            
            //foreach (var unit in units)
            for(int i = 0; i < units.Length; i++)
            {
                switch (command)
                {
                    case (SkillAvailable.MoveToSpot):
                        units[i].ChangeState(UnitState.Move);
                        break;
                    case (SkillAvailable.Charge):
                        units[i].ChargeToEnemy();
                        break;
                    case (SkillAvailable.Shoot):
                        if(!inputActions.Command.skillClick.enabled)
                        {
                            SelectShotSpot();
                            return;
                        }
                        else
                        {
                            units[i].GetComponent<AllyRange>().SetNewTarget(skillTargets[i]);
                        }
                        break;
                }
            }
        }
    }
}
