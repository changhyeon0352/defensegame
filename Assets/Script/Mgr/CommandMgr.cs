
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.UI.CanvasScaler;

public static class InputSystemUtils
{
    public static bool IsClickOnUI()
    {
        var pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        var raycastResultsList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);

        return raycastResultsList.Any(result => result.gameObject is GameObject);
    }
}


public class CommandMgr : MonoBehaviour
{
    public GameObject[] skillIndicatorPrefabs;
    List<UnitGroup> seletedGroupList = null;
    PlayerInput inputActions;
    public GameObject[] skillPrefabs;
    private List<Transform> skillIndicatorTr = new List<Transform>();
    List<Transform> skillTargets = new List<Transform>();
    SkillAvailable usingSkill = SkillAvailable.None;
    private bool pressControl = false;
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

    

    private void OnDisable()
    {
        inputActions.Command.Disable();
    }
    
    private void Update()
    {
        
        //if(Mouse)
        //q,e로 회전가능 휠로 크기조절 가능 궁수숫자만큼 target생성
        if (skillIndicatorTr.Count>0)
        {
            
                
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
            {
                for(int i=0; i<skillIndicatorTr.Count; i++)
                {
                    skillIndicatorTr[i].position = hit.point;
                    skillIndicatorTr[i].forward = hit.point - seletedGroupList[i].spots.position;
                }
            }
        }
     }
    
   

    private void OnSelect(InputAction.CallbackContext obj)
    {
        if (InputSystemUtils.IsClickOnUI())
        {
            Debug.Log("yes");
            return;
        }
        else
        {
            Debug.Log("no");
        }
            Debug.Log("셀렉트");
        pressControl = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if(hit.transform.GetComponent<AllyUnit>()!=null)
            {
                UnitGroup unitGroup = hit.transform.parent.parent.GetComponent<UnitGroup>();
                if (unitGroup != null)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (seletedGroupList.Contains(unitGroup))
                        {
                            seletedGroupList.Remove(unitGroup);
                        }
                        else
                        {
                            seletedGroupList.Add(unitGroup);
                        }
                    }
                    else
                    {
                        ClearSelect();
                        seletedGroupList.Add(unitGroup);
                    }
                }
            }
            else
            {
                Debug.Log(hit.transform.name);
                if (!Input.GetKey(KeyCode.LeftShift))
                    ClearSelect();
            }
        }
        AllCheckSelected();
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

    public void ClearSelect()
    {
        seletedGroupList.Clear();
        GameMgr.Instance.uiMgr.ClearSkillButton();
    }
    
    public void AllCheckSelected()
    {
        UnitGroup[] unitGroups = FindObjectsOfType<UnitGroup>(); 
        foreach(UnitGroup unitGroup in unitGroups)
        {
            unitGroup.CheckSelected();
        }
    }
    private void OnSkillClick(InputAction.CallbackContext obj)
    {


        UnitCommand((usingSkill));

        foreach(var indicator in skillIndicatorTr)
        {
            indicator.GetChild(1).parent = null;
            Destroy(indicator.gameObject);
        }
        
        inputActions.Command.Select.Enable();
        inputActions.Command.skillClick.Disable();
        usingSkill = SkillAvailable.None;
        skillIndicatorTr.Clear();
        skillTargets.Clear();
    }

    //스킬=================================================
    public void SelectShotSpot()
    {
        usingSkill = SkillAvailable.Shoot;
        inputActions.Command.Select.Disable();
        inputActions.Command.skillClick.Enable();
        List<Transform> spots = new();
        for(int i = 0; i < seletedGroupList.Count; i++)
        {
            skillIndicatorTr.Add(Instantiate(skillIndicatorPrefabs[0]).transform);
            skillIndicatorTr[i].localScale = new Vector3(seletedGroupList[i].rowColumn.y, 1, seletedGroupList[i].rowColumn.x)*GameMgr.Instance.settingMgr.UnitOffset;
            spots.Add(Instantiate(seletedGroupList[i].spots));
            
            spots[i].parent = skillIndicatorTr[i];
            spots[i].position = Vector3.zero;
            spots[i].forward = skillIndicatorTr[i].forward;
        }
        //스팟즈를 스킬타켓즈에 순서대로 넣기

        for(int i=0;i<spots.Count;i++)
        {
            for(int a = 0; a < spots[i].childCount;a++)
            {
                skillTargets.Add(spots[i].GetChild(a));
            }
        }
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
