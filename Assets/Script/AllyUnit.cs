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
    protected override void Update()
    {
        //if(isSelectedUnit)
        //{
        //    if (Input.GetKey(KeyCode.X))
        //    {
        //        ChargeToEnemy();
        //    }
        //    if(Input.GetKey(KeyCode.Z))
        //    {
        //        ChangeState(unitState.Move);
        //    }
        //}
        
        base.Update();
        
    }
    public void ChargeToEnemy()
    {
        int multipleNum = 2;
        for(int i=0; i<10; i++)
        {
            if (SearchAndChase(searchRange*multipleNum))
            {
                ChangeState(unitState.Chase);
                break;
            }
            multipleNum *= multipleNum;

        }
    }
    protected override void Die()
    {
        IsSelectedUnit = false;
        transform.parent.GetComponent<UnitGroup>().RemoveUnitFromList(this);
        transform.parent = null;
        base.Die();
    }
}
