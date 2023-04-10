using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FixedUnit : Unit
{
    protected bool isShotSpot = false;
    protected Transform target=null;

    abstract public void SetTarget(Transform target);
    protected override void IdleUpdate()
    {
        Collider enemyTr = SearchEnemyInRange(currentStat.searchRange);
        if (enemyTr != null)
        {
            target = enemyTr.transform;
            SetTarget(target);
            ChangeState(UnitState.Attack);
        }
    }
    protected override void AttackUpdate()
    {
        if (target != null)
        {
            transform.LookAt(target);
            if (Time.time-lastAttackTime>=attackCooldown)
            {
                float distance = Vector3.Distance(transform.position,target.position);
                if (distance < currentStat.searchRange + 0.6f || isShotSpot)
                {
                    anim.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                }
                else
                    ChangeState(UnitState.Idle);
            }
        }
        else
        {
            ChangeState(UnitState.Idle);
        }
    }
    protected override void ChaseUpdate()
    {
        ChangeState(UnitState.Idle);
    }

    

    protected override void MoveUpdate()
    {
        ChangeState(UnitState.Idle);
    }
}
