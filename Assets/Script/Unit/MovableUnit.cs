using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableUnit : Unit
{
    protected override void IdleUpdate()
    {
        if (IsEnemyInSearchRange(currentStat.searchRange))
            ChangeState(UnitState.Chase);
        if (goalTr != null && unitData.Type != UnitType.hero)
            transform.LookAt(goalTr.forward + transform.position);
    }

    protected override void MoveUpdate()
    {
        if (!navMesh.enabled)
            return;
        navMesh.SetDestination(goalTr.position);          //목표로 가기
        if (IsEnemyInSearchRange(currentStat.searchRange))
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
        if ((isProvoked && targetCol != null) || IsEnemyInSearchRange(currentStat.searchRange)) //도발되었고 쫒는놈이 있다면 혹은 서칭거리안에 있다면
        {

            navMesh.SetDestination(targetCol.transform.transform.position);
            if (navMesh.enabled && navMesh.remainingDistance < currentStat.attackRange && !navMesh.pathPending)
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
        if (targetCol!=null&& targetCol.enabled)
        {
            transform.LookAt(targetCol.transform.position);

            float distance = Vector3.SqrMagnitude(targetCol.transform.position - transform.position);
            if (distance < currentStat.attackRange * currentStat.attackRange) //사거리 이내면
            {
                if (Time.time - lastAttackTime >= attackCooldown)
                {
                    anim.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                }
            }
            else //사거리 밖이면
            {
                if (isProvoked)
                {
                    ChaseTarget();
                }
                else
                {
                    ChangeState(UnitState.Move);
                }
            }
            
        }
        else//attactTr이 없어지면
        {
            isProvoked = false;
            Collider enemyCol = SearchEnemyInRange(currentStat.attackRange);
            if (enemyCol != null)
            {
                targetCol = enemyCol;
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
        Collider enemyTr = SearchEnemyInRange(radius);   //적 검색
        if (enemyTr != null)
        {
            targetCol = enemyTr;                    //적을 표적으로
            result = true;
        }
        return result;
    }

}
