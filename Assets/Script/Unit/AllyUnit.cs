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
            //base.MoveUpdate();
        }
        else
        {
            navMesh.SetDestination(goalTr.position);          //목표로 가기
            if (navMesh.remainingDistance < unitStat.attackRange && !navMesh.pathPending)                  //목표에 다가가면 Idle로 변경
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
            //if (SearchAndChase(unitStat.searchRange*multipleNum))
            //{
            //    ChangeState(UnitState.Chase);
            //    isProvoked = true;
            //    break;
            //}
            multipleNum *= multipleNum;

        }
    }
    protected override void Die()
    {
        DataMgr.Instance.DieAlly(this);
        IsSelectedUnit = false;
        UnitGroup unitGroup=transform.parent.parent.GetComponent<UnitGroup>();

        transform.parent = null;
        unitGroup.RemoveUnitFromList(this);
        
        base.Die();
    }

    protected override void IdleUpdate()
    {
        throw new System.NotImplementedException();
    }

    protected override void ChaseUpdate()
    {
        throw new System.NotImplementedException();
    }

    protected override void AttackUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
