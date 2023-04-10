using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;


public class UnitGroup:MonoBehaviour
{
    [SerializeField]List<Unit> unitList;
    private UnitType unitType = UnitType.none;
    [SerializeField]
    private Transform unitsTr;
    public Transform UnitsTr { get { return unitsTr; } }
    [SerializeField]
    private Transform spotsTr;
    public Transform SpotsTr { get { return spotsTr; } }
    //유닛그룹의 부모가 될 오브젝트
    public Transform AllyGroups;
    public Vector2Int rowColumn=Vector2Int.zero;
    BasicSkills groupSkill =BasicSkills.None;
    private bool isSelected=false;

    public BasicSkills GroupSkill { get => groupSkill; }
    public int NumUnitList { get => unitList.Count; }
    public List<Unit> UnitList { get => unitList; }

    private void Awake()
    {
        unitList = new List<Unit>();
        //AllyGroups = FindObjectOfType<AllyUnitGroups>().transform;
    }
    public void CheckSelected()
    {
        

    }
    public void SetSpots(Vector3 position)
    {
        spotsTr.position = position;
    }
    public void AddUnitList(Unit unit)
    {
        unitList.Add(unit);
    }
    public void SetUnitGroupSkill(UnitData data)
    {
        unitType = data.Type;
        if (unitList.Count == 0)
            Destroy(this.gameObject);
        
        if (unitType == UnitType.soldier_Melee)
        {
            groupSkill |= BasicSkills.MoveToSpot;
            groupSkill |= BasicSkills.Charge;
        }
        else if (unitType == UnitType.soldier_Range)
        {
            groupSkill |= BasicSkills.ShootSpot;
            groupSkill |= BasicSkills.ShootEnemy;
        }
        
    }
    public void RemoveUnitFromList(Unit unit)
    {
        unitList.Remove(unit);
        Destroy(unit.gameObject);
        if(unitList.Count== 0)
        {
            //GameMgr.Instance.defenseMgr.SelectedGroupList.Remove(this);
            Destroy(gameObject);
        }
    }
    public void SelectThisGroup()
    {
        isSelected = true;
        foreach(Unit unit in unitList)
        {
            unit.ShowSelectEffect();
        }
    }
    public void CancelSelect()
    {
        isSelected = false;
        foreach (Unit unit in unitList)
        {
            unit.HideSelectEffect();
        }
    }
}
