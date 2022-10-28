using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    meleeSoldier=0,
    rangeSoldier,
    warrior,
    mage,

}
public class UnitGroup:MonoBehaviour
{
    List<AllyUnit> unitList;
    UnitType unitType;

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
    }
    public void RemoveUnitFromList(AllyUnit unit)
    {
        unitList.Remove(unit);
    }
}
