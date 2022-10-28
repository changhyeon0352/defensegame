
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum CommandType
{
    moveToGoal=0,
    Charge
}

public class CommandMgr : MonoBehaviour
{
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
    }

    public void ClearSelect()
    {
        seletedGroupList.Clear();
    }
    private void ClickGroup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ClearSelect();
                UnitGroup unitGroup = hit.transform.parent.GetComponent<UnitGroup>();
                if (unitGroup != null)
                {
                    seletedGroupList.Add(unitGroup);
                }
            }
                    
        }
        else if (Physics.Raycast(ray, out RaycastHit hit1, 1000.0f, LayerMask.GetMask("Ground")))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ClearSelect();
            }
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
        UnitCommand((CommandType)command);
    }
    public void UnitCommand(CommandType command )
    {
        foreach (UnitGroup unitGroup in seletedGroupList)
        {
            AllyUnit[] units = unitGroup.GetComponentsInChildren<AllyUnit>();
            foreach (AllyUnit unit in units)
            {
                switch (command)
                {
                    case(CommandType.moveToGoal):
                        unit.ChangeState(unitState.Move);
                        break;
                    case (CommandType.Charge):
                        unit.ChargeToEnemy();
                        break;
                }
            }
        }
    }
}
