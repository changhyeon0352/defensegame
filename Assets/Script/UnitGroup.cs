using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitGroup:MonoBehaviour
{
    List<AllyUnit> unitList;
    UnitType unitType;
    SkillAvailable groupSkill =SkillAvailable.None;

    public SkillAvailable GroupSkill { get => groupSkill; }

    private void Awake()
    {
        unitList = new List<AllyUnit>();
    }
    public void CheckSelected()
    {
        bool isSelected = (GameMgr.Instance.CommandMgr.SelectedGroupList.Contains(this));
        
        for(int i=0;i<unitList.Count;i++)
        {
            unitList[i].IsSelectedUnit = isSelected;
        }
    }
    public void InitializeUnitList()
    {
        unitList.Clear();
        
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            AllyUnit allyUnit = transform.GetChild(i).GetComponent<AllyUnit>();
            unitList.Add(allyUnit);
        }
        tag = transform.GetChild(0).tag;
        SetAvailableSkills();
    }
    public void RemoveUnitFromList(AllyUnit unit)
    {
        unitList.Remove(unit);
    }
    private void SetAvailableSkills()
    {
        if (CompareTag("Melee"))
        {
            groupSkill |= SkillAvailable.MoveToSpot;
            groupSkill |= SkillAvailable.Charge;
        }
        else if (CompareTag("Range"))
        {
            groupSkill |= SkillAvailable.Shoot;
        }
    }
}
