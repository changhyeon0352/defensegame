using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableUnit : Unit
{
    protected override void IdleUpdate()
    {
        if (IsEnemyInSearchRange(unitStat.searchRange))
            ChangeState(UnitState.Chase);
        if (goalTr != null && unitData.unitType != UnitType.hero)
            transform.LookAt(goalTr.forward + transform.position);
    }

    protected override void MoveUpdate()
    {
        if (!navMesh.enabled)
            return;
        navMesh.SetDestination(goalTr.position);          //목표로 가기
        if (IsEnemyInSearchRange(unitStat.searchRange))
            ChangeState(UnitState.Chase);
        if (navMesh.remainingDistance < 0.1f && !navMesh.pathPending)                  //목표에 다가가면 Idle로 변경
        {
            ChangeState(UnitState.Idle);
        }
    }
    protected override void ChaseUpdate()
    {
        if (!navMesh.enabled)
            return;
        if ((isProvoked && chaseTargetTr != null) || IsEnemyInSearchRange(unitStat.searchRange)) //도발되었고 쫒는놈이 있다면 혹은 서칭거리안에 있다면
        {

            navMesh.SetDestination(chaseTargetTr.position);
            if (navMesh.enabled && navMesh.remainingDistance < unitStat.attackRange && !navMesh.pathPending)
            {
                ChangeState(UnitState.Attack);
            }
        }
        else
        {
            ChangeState(UnitState.Move);
        }
        
    }
    protected override void AttackUpdate()//사거리 체크를 시간 체크보다 먼저 해야겠다
    {
        if (attackTargetTr != null)
        {
            transform.LookAt(attackTargetTr);
            if (Time.time-lastAttackTime>=AttackSpeed)
            {
                //죽어서 col사라지는거 떄문에 적은듯? 체크 필요
                //Collider col = attackTargetTr.GetComponent<CapsuleCollider>();
                //if (col != null && !col.enabled)
                //{
                //    ChangeState(UnitState.Move);
                //    return;
                //}

                float distance = Vector3.SqrMagnitude(attackTargetTr.position - transform.position);
                if (distance < unitStat.attackRange * unitStat.attackRange) //사거리 이내면
                {
                    if (Time.time - lastAttackTime >= AttackSpeed)
                    {
                        anim.SetTrigger("Attack");
                        lastAttackTime = Time.time;
                    }
                }
                else //사거리 밖이면
                {
                    if (isProvoked)
                    {
                        ChaseTarget(attackTargetTr);
                    }
                    else
                    {
                        ChangeState(UnitState.Move);
                    }
                }
            }
        }
        else//attactTr이 없어지면
        {
            isProvoked = false;
            Transform enemyTr = SearchEnemyInRange(unitStat.attackRange);
            if (enemyTr != null)
            {
                attackTargetTr = enemyTr;
            }
            else
            {
                ChangeState(UnitState.Move);
            }
        }
    }
    
    public bool IsEnemyInSearchRange(float radius)
    {
        bool result = false;
        Transform enemyTr = SearchEnemyInRange(radius);   //적 검색
        if (enemyTr != null)
        {
            chaseTargetTr = enemyTr;                    //적을 표적으로
            result = true;
        }
        return result;
    }

}
