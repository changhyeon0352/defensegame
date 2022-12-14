using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AllyUnit : Unit
{
    
    [SerializeField] GameObject selectedMark;
    private bool isSelectedUnit=false;

    public bool isattackMove = true;
    public bool IsSelectedUnit
    {
        get => isSelectedUnit;
        set
        {
            isSelectedUnit = value;
            selectedMark.SetActive(isSelectedUnit);
        }
    }
    protected override void MoveUpdate()
    {
        if (isattackMove)
        {
            base.MoveUpdate();
        }
        else
        {
            agent.SetDestination(goalTr.position);          //목표로 가기
            if (agent.remainingDistance < stopRange && !agent.pathPending)                  //목표에 다가가면 Idle로 변경
            {
                ChangeState(UnitState.Idle);
                isattackMove = true;
            }
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
                isProvoked = true;
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
