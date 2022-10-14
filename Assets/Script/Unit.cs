using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Unit : MonoBehaviour,IHealth
{
    protected NavMeshAgent agent;
    protected Animator anim;
    protected Transform chaseTargetTr;
    protected Transform attackTargetTr;
    protected unitState state = unitState.Idle;
    public LayerMask enemyLayer;

    protected float timeCount;
    protected float attackSpeed = 2.0f;
    protected int hp = 100;
    protected int attack = 20;
    float searchRange = 3f;
    float attackRange = 1.5f;

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
                ChangeState(unitState.Dead);
            }
            Debug.Log($"{transform.name}의 hp: {hp}");
        }
    }
    public int Attack { get => attack; set => attack=value; }
    public void TakeDamage(int damage)
    {
       Hp-=damage;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim= GetComponent<Animator>();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            ChangeState(unitState.Move);
        }
        
            

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
    //==============================================================================
    virtual protected void IdleUpdate()
    {
        // 아군유닛 대기상태
        // 아군유닛 이동으로 전환
        //궁수는  Attack 전환
        //전사는  Chase 전환
        SearchAndChase(searchRange);
    }

    

    virtual protected void MoveUpdate()
    {
        agent.SetDestination(goalTr.position);          //목표로 가기
        SearchAndChase(searchRange);
        if(agent.remainingDistance<1f&&!agent.pathPending)                  //목표에 다가가면 Idel로 변경
        {                                       
            ChangeState(unitState.Idle);                        
        }

    }
    virtual protected void ChaseUpdate()
    {
        if (chaseTargetTr != null)
        {
            agent.SetDestination(chaseTargetTr.position);
        }
        if (agent.remainingDistance<1f && !agent.pathPending)
        {
            ChangeState(unitState.Attack);
        }
    }
    public void MeleeAttack()
    {
        IHealth Enemy_IHealth = attackTargetTr.GetComponent<IHealth>();
        Enemy_IHealth.TakeDamage(Attack);
    }
    virtual protected void AttackUpdate()
    {
        timeCount -= Time.deltaTime;
        if(attackTargetTr != null)
        {
            transform.LookAt(attackTargetTr);
        }
        else
        {
            ChangeState(unitState.Move);
        }
        if (timeCount < 0)
        {
            Transform enemyTr = SearchEnemy(attackRange);
            if (enemyTr != null)
            {
                attackTargetTr = enemyTr;
                anim.SetTrigger("Attack");
                timeCount = attackSpeed;
            }
            else
            {
                ChangeState(unitState.Move);
            }
            
        }
        
        //시간재고 공격
        //적은 범위내 상대가 없으면 무브
        //아군 범위내 상대가 없으면 진형으로 복귀

    }

   

    //=================================================================================
    private void SearchAndChase(float radius)
    {
        Transform enemyTr = SearchEnemy(radius);   //적 검색
        if (enemyTr != null)
        {
            chaseTargetTr = enemyTr;                    //적을 표적으로
            ChangeState(unitState.Chase);               //표적 추적으로 변경
        }
    }
    public Transform SearchEnemy(float radius)
    {
        Transform enemyTr = null;
        Collider[] cols = Physics.OverlapSphere(transform.position, 2f, enemyLayer);
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
    

    public void ChangeState(unitState newState)
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
                anim.SetInteger("iState", 0);
                break;
            case unitState.Move:
                anim.SetInteger("iState", 1);
                break;
            case unitState.Chase:
                anim.SetInteger("iState", 1);
                break;
            case unitState.Attack:
                anim.SetInteger("iState", 0);
                timeCount = 0;
                break;
            case unitState.Dead:
                anim.SetInteger("iState", 3);
                break;
            default:
                break;
        }
        state = newState;

    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, searchRange);
    }

    
}
