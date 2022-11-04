using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitGroup:MonoBehaviour
{
    List<AllyUnit> unitList;
    private UnitType unitType = UnitType.none;
    public Transform units;
    public Transform spots;
    public Transform AllyGroups;
    private Vector2Int rowColumn=Vector2Int.zero;

    SkillAvailable groupSkill =SkillAvailable.None;

    public SkillAvailable GroupSkill { get => groupSkill; }
    public UnitType UnitType { get => unitType;}

    private void Awake()
    {
        unitList = new List<AllyUnit>();
        AllyGroups = FindObjectOfType<AllyUnitGroups>().transform;
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
        
        for (int i = 0; i < units.childCount; i++)
        {
            AllyUnit allyUnit = transform.GetChild(0).GetChild(i).GetComponent<AllyUnit>();
            unitList.Add(allyUnit);
        }
        tag = transform.GetChild(0).GetChild(0).tag;
        if (CompareTag("Melee"))
        {
            groupSkill |= SkillAvailable.MoveToSpot;
            groupSkill |= SkillAvailable.Charge;
            unitType = UnitType.meleeSoldier;
        }
        else if (CompareTag("Range"))
        {
            groupSkill |= SkillAvailable.Shoot;
            unitType = UnitType.rangeSoldier;
        }
    }
    public void RemoveUnitFromList(AllyUnit unit)
    {
        unitList.Remove(unit);
    }
    
}
