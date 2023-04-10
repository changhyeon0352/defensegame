using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    List<UnitGroup> selectedUnitGroups = new List<UnitGroup>();


    public void NewUnitGroup(UnitGroup unitGroup)
    {
        foreach(UnitGroup group in selectedUnitGroups)
        {
            group.CancelSelect();
        }
        selectedUnitGroups.Clear();
        AddUnitGroup(unitGroup);

    }
    public void AddUnitGroup(UnitGroup unitGroup)
    {
        selectedUnitGroups.Add(unitGroup);
        unitGroup.SelectThisGroup();
    }
}
