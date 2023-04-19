using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : Controller
{
    List<UnitGroup> selectedGroupList = new List<UnitGroup>();
    UnitCommandUI unitCommandUI;
    SkillController skillController;
    private HeroUnitController heroController;
    private void OnEnable()
    {
        inputActions.Command.Select.performed += OnSelect;
        heroController=FindObjectOfType<HeroUnitController>();
        skillController=FindObjectOfType<SkillController>();
        skillController.cancelAction += OnCancelSkillClick;
        
    }

    private void OnCancelSkillClick()
    {
        inputActions.Command.skillClick.performed -= ShootToSpot;
    }

    private void ShootToSpot(InputAction.CallbackContext _)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        GameObject obj = new GameObject();
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            obj.transform.position = hit.point;
        }
        for(int i = 0; i < selectedGroupList.Count; i++)
        {
            for (int j = 0; j < selectedGroupList[i].UnitList.Count; j++)
            {
                AllyRange allyRange = selectedGroupList[i].UnitList[j] as AllyRange;
                allyRange.ShotSpotMode(obj.transform);
            }
        }
        inputActions.Command.Select.Enable();
        inputActions.Command.skillClick.Disable();
        skillController.SkillEnd();
    }

    private void Start()
    {
        unitCommandUI = FindObjectOfType<UnitCommandUI>();
    }
    private void OnSelect(InputAction.CallbackContext obj)
    {
        if (Utils.IsClickOnUI())//UI 클릭 하면 OnSelect를 스킵
        {
            return;
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit != null)
            {
                UIMgr.Instance.unitStatUI.gameObject.SetActive(true);
                UIMgr.Instance.unitStatUI.RefreshUnitStatWindow(unit);
            }
            if ((int)MathF.Pow(2, hit.transform.gameObject.layer) == LayerMask.GetMask("Ally")) //hit한게 ally layer라면
            {
                //영웅이 클릭됐는지
                HeroUnit hero = hit.transform.GetComponent<HeroUnit>();
                if (hero != null)
                {
                    heroController.SelectHero(hero);
                }
                else //병사들이 클릭 됐으면
                {
                    UnitGroup unitGroup = hit.transform.parent.parent.GetComponent<UnitGroup>();
                    if (unitGroup != null)
                    {
                        if (Input.GetKey(KeyCode.LeftShift) && selectedGroupList.Count > 0)
                        {
                            if (selectedGroupList.Contains(unitGroup))
                            {
                                RemoveUnitGroup(unitGroup);
                            }
                            else
                            {
                                AddUnitGroup(unitGroup);
                            }
                        }
                        else
                        {
                            ClearUnitGroupList();
                            AddUnitGroup(unitGroup);
                        }
                    }
                }
            }
            else //아군이 아니면 ex)땅,적
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                    ClearUnitGroupList();
            }
        }
        //유닛 그룹에 스킬 교집합 체크 스킬넘버배열을 넘겨주자
        //스킬 사용가능여부 비트연산자 이용할것
        CheckIntersectionUnitCommand();

    }
    private void ClearUnitGroupList()
    {
        for (int i = selectedGroupList.Count - 1; i >= 0; i--)
        {
            UnitGroup group = selectedGroupList[i];
            group.CancelSelect();
            selectedGroupList.RemoveAt(i);
        }
    }

    
    private void AddUnitGroup(UnitGroup unitGroup)
    {
        selectedGroupList.Add(unitGroup);
        unitGroup.SelectThisGroup();
    }
    private void RemoveUnitGroup(UnitGroup unitGroup)
    {
        selectedGroupList.Remove(unitGroup);
        unitGroup.CancelSelect();
    }
    private void CheckIntersectionUnitCommand()
    {
        SoldierSkill groupsSkills = ~SoldierSkill.None; //1111 1111 로 초기화
        if(selectedGroupList.Count > 0)
        {
            foreach (UnitGroup selectGroup in selectedGroupList)
            {
                groupsSkills &= selectGroup.GroupSkill; //유닛그룹들의 사용가능 스킬 교집합만 남긴다.
            }
        }
        else
            groupsSkills=SoldierSkill.None;
        unitCommandUI.UpdateUnitCommandButton(groupsSkills); //버튼에 사용사능 스킬만 활성화             
    }
    public void CommandToUnitGroup(SoldierSkill command)
    {
        switch (command)
        {
            case(SoldierSkill.Charge):
                for(int i=0; i<selectedGroupList.Count;i++)
                {
                    for(int j=0; j < selectedGroupList[i].UnitList.Count;j++)
                    {
                        AllyMelee allyMelee = selectedGroupList[i].UnitList[j] as AllyMelee;
                        allyMelee.ChargeToEnemy();
                    }
                }
                break;
            case (SoldierSkill.ReturnToLine):
                for (int i = 0; i < selectedGroupList.Count; i++)
                {
                    for (int j = 0; j < selectedGroupList[i].UnitList.Count; j++)
                    {
                        AllyMelee allyMelee = selectedGroupList[i].UnitList[j] as AllyMelee;
                        allyMelee.ReturnToLine();
                    }
                }
                break;
            case (SoldierSkill.ShootSpot):
                ChooseSpotToShoot();
                
                break;
            case (SoldierSkill.ShootEnemy):
                for (int i = 0; i < selectedGroupList.Count; i++)
                {
                    for (int j = 0; j < selectedGroupList[i].UnitList.Count; j++)
                    {
                        AllyRange allyRange = selectedGroupList[i].UnitList[j] as AllyRange;
                        allyRange.ShotEnemyMode();

                    }
                }
                break;

        }

    }

    private void ChooseSpotToShoot()
    {
        inputActions.Command.skillClick.Enable();
        inputActions.Command.skillClick.performed += ShootToSpot;
        inputActions.Command.Select.Disable();
        skillController.ShowIndicator(AllyRange.SpotRadius);
    }
}
