using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEditor;

public class Unit : MonoBehaviour,IHealth
{
    protected NavMeshAgent agent;
    protected Animator anim;
    protected Transform chaseTargetTr;
    protected Transform attackTargetTr;
    protected UnitState state = UnitState.Idle;
    public LayerMask enemyLayer;

    protected float timeCount;
    protected float attackSpeed = 2.0f;
    protected int hp = 100;
    protected int attack = 20;
    protected const float stopRange = 0.1f;
    protected const float searchRange = 4f;
    const float attackRange = 2f;

    public Transform goalTr;
    public int Hp 
    { 
        get => hp;
        set
        {
            hp = value; 
            if (hp <= 0)
            {
                hp = 0;
                //죽음
                if(state !=UnitState.Dead)
                {
                    Die();
                }
                
            }
            Debug.Log($"{transform.name}의 hp: {hp}");
            
        }
    }

    virtual protected void Die()
    {
        GetComponent<Collider>().enabled = false;
        agent.enabled = false;
        ChangeState(UnitState.Dead);
        
        //Destroy(gameObject);
    }

    public int Attack { get => attack; set => attack=value; }
    public void TakeDamage(int damage)
    {
       Hp-=damage;
    }

    virtual protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim= GetComponent<Animator>();
    }
    virtual protected void Update()
    {
        switch (state)
        {
            case UnitState.Idle:
                IdleUpdate();
                break;
            case UnitState.Move:
                MoveUpdate();
                break;
            case UnitState.Chase:
                ChaseUpdate();
                break;
            case UnitState.Attack:
                AttackUpdate();
                break;
            case UnitState.Dead:
            default:
                break;
        }

    }
    //==============================================================================
    virtual protected void IdleUpdate()
    {
        // 아군유닛 대기상태
        // 아군유닛 이동으로 전환
        //궁수는  Attack 전환
        //전사는  Chase 전환
        SearchAndChase(searchRange);
        if(goalTr!=null)
            transform.LookAt(goalTr.forward+transform.position);
    }

    

    virtual protected void MoveUpdate()
    {
        agent.SetDestination(goalTr.position);          //목표로 가기
        SearchAndChase(searchRange);
        if (agent.remainingDistance < stopRange && !agent.pathPending)                  //목표에 다가가면 Idle로 변경
        {
            ChangeState(UnitState.Idle);
        }
    }
    virtual protected void ChaseUpdate()
    {
        if (chaseTargetTr != null)
        {
            agent.SetDestination(chaseTargetTr.position);
        }
        else
        {
            ChangeState(UnitState.Move);
        }
        if (agent.remainingDistance<attackRange && !agent.pathPending)
        {
            ChangeState(UnitState.Attack);
        }
    }
    public void MeleeAttack()
    {
        if(attackTargetTr!=null)
        {
            IHealth Enemy_IHealth = attackTargetTr.GetComponent<IHealth>();
            Enemy_IHealth.TakeDamage(Attack);
        }
        
    }
    virtual protected void AttackUpdate()
    {
        timeCount -= Time.deltaTime;
        if(attackTargetTr != null)
        {
            transform.LookAt(attackTargetTr);
            if (timeCount < 0)
            {
                float distance=Vector3.SqrMagnitude(attackTargetTr.position-transform.position);
                if (distance<attackRange*attackRange)
                {
                    
                    anim.SetTrigger("Attack");
                    timeCount = attackSpeed;
                    attackTargetTr = null;
                }
                else
                {
                    ChangeState(UnitState.Move);
                }

            }
        }
        else
        {
            Transform enemyTr = SearchEnemy(attackRange);
            if(enemyTr!=null)
            {
                attackTargetTr = enemyTr;
            }
            else
            {
                ChangeState(UnitState.Move);
            }
        }
        
        
        //시간재고 공격
        //적은 범위내 상대가 없으면 무브
        //아군 범위내 상대가 없으면 진형으로 복귀

    }

   

    //=================================================================================
    public bool SearchAndChase(float radius)
    {
        bool result = false;
        Transform enemyTr = SearchEnemy(radius);   //적 검색
        if (enemyTr != null)
        {
            chaseTargetTr = enemyTr;                    //적을 표적으로
            ChangeState(UnitState.Chase);               //표적 추적으로 변경
            result = true;
        }
        return result;
    }
    public Transform SearchEnemy(float radius)
    {
        Transform enemyTr = null;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        float minimunDistance = float.MaxValue;
        foreach (Collider col in cols)
        {
            float distance = Vector3.SqrMagnitude(transform.position - col.transform.position);

            minimunDistance = MathF.Min(distance, minimunDistance);
            if (minimunDistance == distance)
            {
                enemyTr = col.transform;
            }
        }
        return enemyTr;
    }
    

    public void ChangeState(UnitState newState)
    {
        switch (state)
        {
            case UnitState.Idle:
                break;
            case UnitState.Move:
                //agent.ResetPath();
                break;
            case UnitState.Chase:
                break;
            case UnitState.Attack:
                attackTargetTr = null;

                break;
        }
        switch (newState)
        {
            case UnitState.Idle:
                anim.SetInteger("iState", 0);
                break;
            case UnitState.Move:
                chaseTargetTr = null;
                anim.SetInteger("iState", 1);
                agent.stoppingDistance = 0;
                break;
            case UnitState.Chase:
                anim.SetInteger("iState", 1);
                agent.stoppingDistance = attackRange;
                break;
            case UnitState.Attack:
                anim.SetInteger("iState", 2);
                timeCount = attackSpeed/3;
                attackTargetTr = chaseTargetTr;
                chaseTargetTr = null;
                break;
            case UnitState.Dead:
                anim.SetTrigger("isDead");
                break;
            default:
                break;
        }
        state = newState;

    }
    
    
    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, transform.up, searchRange);
    }

    
}
