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
        // �Ʊ����� ������
        // �Ʊ����� �̵����� ��ȯ
        //�ü���  Attack ��ȯ
        //�����  Chase ��ȯ

    }
    private void MoveUpdate()
    {
        //�� chase��ȯ
        //
    }
    private void ChaseUpdate()
    {
        //���� ������ ��밡 ������ ����
        //�Ʊ� ������ ��밡 ������ �������� ����
    }
    private void AttackUpdate()
    {
        //�ð���� ����
        //���� ������ ��밡 ������ ����
        //�Ʊ� ������ ��밡 ������ �������� ����
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
