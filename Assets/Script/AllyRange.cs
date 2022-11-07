using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyRange : AllyUnit
{
    BowLoadShot bowLoadShot = null;
    public float shotRange = 10f;
    private bool isShotSpot=false;
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
        timeCount -= Time.deltaTime;
        if (bowLoadShot.target != null)
        {
            transform.LookAt(bowLoadShot.target);
            if (timeCount < 0)
            {
                anim.SetTrigger("Attack");
                timeCount = attackSpeed;
            }
        }
        else
        {
            ChangeState(UnitState.Idle);
        }

    }
    public void CheckTargetAlive()
    {
        Collider col = bowLoadShot.target.GetComponent<Collider>();
        if((col==null||!col.enabled)&&!isShotSpot)
        {
            ChangeState(UnitState.Idle);
        }
    }

    public void SetNewTarget(Transform targetTr)
    {
        bowLoadShot.target = targetTr;
        ChangeState(UnitState.Attack);
        isShotSpot = true;
    }
    
}
