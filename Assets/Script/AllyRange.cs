using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyRange : AllyUnit
{
    BowLoadShot bowLoadShot = null;
    public float shotRange = 10f;

    protected override void Awake()
    {
        base.Awake();
        bowLoadShot = GetComponent<BowLoadShot>();
    }

    protected override void IdleUpdate()
    {
        //// 아군유닛 대기상태
        //// 아군유닛 이동으로 전환
        ////궁수는  Attack 전환
        ////전사는  Chase 전환
        //SearchAndChase(searchRange);
        //if (goalTr != null)
        //    transform.LookAt(goalTr.forward + transform.position);
        Transform enemyTr=SearchEnemy(shotRange);
        if(enemyTr!=null)
        {
            bowLoadShot.target = enemyTr;
            ChangeState(UnitState.Attack);
        }
    }
    
    protected override void AttackUpdate()
    {

    }
    public void CheckTargetAlive()
    {
        Collider col = bowLoadShot.target.GetComponent<Collider>();
        if(col==null||!col.enabled)
        {
            ChangeState(UnitState.Idle);
        }
    }

    public void SetNewTarget(Transform targetTr)
    {
        bowLoadShot.target = targetTr;
        ChangeState(UnitState.Attack);
    }
}
