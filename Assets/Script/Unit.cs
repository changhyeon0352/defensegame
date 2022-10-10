using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    NavMeshAgent agent;
    Transform targetTr;
    Animator anim;
    unitState state = unitState.Idle;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim= GetComponent<Animator>();
    }
    private void Update()
    {
        if (targetTr != null)
            agent.SetDestination(targetTr.position);

        switch (state)
        {
            case unitState.Idle:
                IdleUpdate();
                break;
            case unitState.Move:
                MoveUpdate();
                break;
            case unitState.Chase:
                ChaseUpdate();
                break;
            case unitState.Attack:
                AttackUpdate();
                break;
            case unitState.Dead:
            default:
                break;
        }

    }

    private void IdleUpdate()
    {
        // 아군유닛 대기상태
        // 아군유닛 이동으로 전환
        //궁수는  Attack 전환
        //전사는  Chase 전환

    }
    private void MoveUpdate()
    {
        //적 chase전환
        //
    }
    private void ChaseUpdate()
    {
        //적은 범위내 상대가 없으면 무브
        //아군 범위내 상대가 없으면 진형으로 복귀
    }
    private void AttackUpdate()
    {
        //시간재고 공격
        //적은 범위내 상대가 없으면 무브
        //아군 범위내 상대가 없으면 진형으로 복귀
    }
    void ChangeState(unitState newState)
    {
        switch (state)
        {
            case unitState.Idle:
                break;
            case unitState.Move:
                break;
            case unitState.Chase:
                break;
            case unitState.Attack:
                break;
        }
        switch (newState)
        {
            case unitState.Idle:
                break;
            case unitState.Move:
                break;
            case unitState.Chase:
                break;
            case unitState.Attack:
                break;
            case unitState.Dead:
                break;
            default:
                break;
        }
        state = newState;

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            targetTr=other.transform;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}
