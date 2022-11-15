using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyUnit : Unit
{
    [SerializeField] GameObject selectedMark;
    private bool isSelectedUnit=false;
    public bool IsSelectedUnit
    {
        get => isSelectedUnit;
        set
        {
            isSelectedUnit = value;
            selectedMark.SetActive(isSelectedUnit);
        }
    }
    
    public void ChargeToEnemy()
    {
        int multipleNum = 2;
        for(int i=0; i<10; i++)
        {
            if (SearchAndChase(searchRange*multipleNum))
            {
                ChangeState(UnitState.Chase);
                break;
            }
            multipleNum *= multipleNum;

        }
    }
    protected override void Die()
    {
        IsSelectedUnit = false;
        UnitGroup unitGroup=transform.parent.parent.GetComponent<UnitGroup>();
        transform.parent = null;
        unitGroup.RemoveUnitFromList(this);
        
        base.Die();
    }
    
}
