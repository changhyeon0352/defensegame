using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class Unit : MonoBehaviour,IHealth,IPointerEnterHandler,IPointerExitHandler
{
    public UnitData unitData;
    protected NavMeshAgent agent;
    protected Animator anim;
    protected Transform chaseTargetTr;
    protected Transform attackTargetTr;
    protected UnitState state = UnitState.Idle;
    public LayerMask enemyLayer;
    public bool isProvoked = false;
    public bool isSleep = false;
    protected float timeCount;
    //protected float attackSpeed = 2.0f;
    protected int hp;
    protected int hpMax = 100;
    protected int mp;
    protected int mpMax = 100;
    protected int attack = 20;
    protected float moveSpeed;
    private Rigidbody rb;
    private Collider col;
    private int attackCombo = 0;
    private float attackSpeed;
    public float AttackSpeed { get { return attackSpeed; } 
        set { 
            attackSpeed = value; 
            anim.SetFloat("attackSpeed", unitData.AttackAniLength[0] * attackSpeed * 1.5f);
            anim.SetFloat("attackSpeed2", unitData.AttackAniLength[1] * attackSpeed * 1.5f);
        } 
    }
    public bool IsDead { get=>state==UnitState.Dead;}
    public float MoveSpeed 
    { 
        get { return moveSpeed; } 
        set 
        { 
            moveSpeed = value; 
            agent.speed = value; 
        } 
    }
    [SerializeField]protected int armor = 0;
    protected int armorPlus = 0;
    public int ArmorPlus { get { return armorPlus; }
        set
        {
            armorPlus = value;
            if (armorPlus > 0)
            {
                if(!shieldbuff.isPlaying)
                    shieldbuff.Play();
            }
            else
            {
                if (shieldbuff.isPlaying)
                    shieldbuff.Stop();
            }
        }
    } 
    protected const float stopRange = 0.1f;
    protected const float searchRange = 4f;
    protected float attackRange = 2f;
    public ParticleSystem shieldbuff;

    public Transform goalTr;

    //=====================================================================================================================
    virtual public void InitializeUnitStat()
    {
        hpMax = unitData.HP;
        hp = hpMax;
        mpMax = unitData.MP;
        mp = mpMax;
        attack = unitData.Atk;
        armor = unitData.Armor;
        MoveSpeed = unitData.MoveSpeed;
        attackRange = unitData.AttackRange;
        AttackSpeed= unitData.AttackSpeed;
    }
    public IEnumerator Provoked(Transform tr,float sec)
    {
            chaseTargetTr = tr;
            ChangeState(UnitState.Chase);
            isProvoked = true;
            GameObject obj = Instantiate(GameMgr.Instance.skillMgr.Buffs[(int)Buff.provoked], transform);
            yield return new WaitForSeconds(sec);
            if (state != UnitState.Dead)
            {
                ChangeState(UnitState.Move);
                isProvoked = false;
            }
            Destroy(obj);
    }
    public IEnumerator Sleep(float sec)
    {
            float count = sec;
            isSleep = true;
            anim.SetTrigger("Sleep");
            agent.enabled = false;
            GameObject obj = Instantiate(GameMgr.Instance.skillMgr.Buffs[(int)Buff.sleep], transform);
            while (count > 0)
            {
                count -= Time.deltaTime;
                yield return null;
                if (!isSleep)
                    break;
            }
            if (state != UnitState.Dead)
            {
                agent.enabled = true;
                isSleep = false;
                anim.SetTrigger("WakeUp");
                ChangeState(UnitState.Move);
            }
            Destroy(obj);
    }
    public IEnumerator Slow(float sec)
    {
        MoveSpeed = unitData.MoveSpeed /4;
        yield return new WaitForSeconds(sec);
        if(state != UnitState.Dead)
            MoveSpeed = unitData.MoveSpeed;
    }
    
    public virtual int Hp 
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
                    DataMgr.Instance.DieAlly(this);
                }
            }
            UIMgr.Instance.hpbar.ChangeHPbar(this, (float)hp / (float)hpMax);
            Debug.Log($"{transform.name}의 hp: {hp}");
        }
    }
    public int Attack { get => attack; set => attack = value; }
    public int Armor { get => armor; set => armor = value; }
    public int HpMax { get => hpMax; }
    public  int Mp { get => mp; }
    public int MpMax { get => mpMax; }

    virtual protected void Die()
    {
        col.enabled = false;
        agent.enabled = false;
        ChangeState(UnitState.Dead);
        ParticleSystem[] psArray =GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem ps in psArray)
        {
            ps.Stop();
        }
    }
    public void DieFall()
    {
        StartCoroutine(DieFallCor());
    }
    IEnumerator DieFallCor()
    {
        if (state == UnitState.Dead)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.drag = 20;
            yield return new WaitForSeconds(1);
            Destroy(this.gameObject);
        }
        
    }
   

    public void TakeDamage(int damage)
    {
        double decreaseRate= 1-Math.Atan((double)(armor+armorPlus)/50)/(Math.PI/2);//아머가 50쯤 되면 50%
        int netDamage = (int)(damage * decreaseRate);
        Hp -= netDamage == 0 ? 1 : netDamage;
        isSleep = false;
    }
    

    virtual protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim= GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }
    virtual protected void Update()
    {
        timeCount -= Time.deltaTime;
        if (isSleep)
            return;
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
        if(SearchAndChase(searchRange))
            ChangeState(UnitState.Chase);
        if(goalTr!=null&&unitData.unitType!=UnitType.hero)
            transform.LookAt(goalTr.forward+transform.position);
    }

    

    virtual protected void MoveUpdate()
    {
        if (!agent.enabled)
            return;

        agent.SetDestination(goalTr.position);          //목표로 가기
        if (SearchAndChase(searchRange))
            ChangeState(UnitState.Chase);
        if (agent.remainingDistance < stopRange && !agent.pathPending)                  //목표에 다가가면 Idle로 변경
        {
            ChangeState(UnitState.Idle);
        }
    }
    virtual protected void ChaseUpdate()
    {
        if(!agent.enabled)
            return ;
        if ((isProvoked&&chaseTargetTr!=null)||SearchAndChase(searchRange)) //도발되었고 쫒는놈이 있다면 혹은 서칭거리안에 있다면
        {
            
            agent.SetDestination(chaseTargetTr.position);
        }
        else
        {
            ChangeState(UnitState.Move);
        }
        if (agent.enabled&&agent.remainingDistance < attackRange && !agent.pathPending)
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
        if(attackTargetTr != null)
        {
            transform.LookAt(attackTargetTr);
            if (timeCount < 0)
            {
                CapsuleCollider capsuleCollider = attackTargetTr.GetComponent<CapsuleCollider>();
                if (capsuleCollider!=null&&!capsuleCollider.enabled)
                {
                    ChangeState(UnitState.Move);
                    return;
                }
                
                float distance=Vector3.SqrMagnitude(attackTargetTr.position-transform.position);
                if (distance<attackRange*attackRange) //사거리 이내면
                {
                    anim.SetInteger("AttackCombo", attackCombo);
                    
                    anim.SetTrigger("Attack");
                    //공격 애니메이션:컴뱃idle = 2:1비율로
                    
                    timeCount = 1/unitData.AttackSpeed;
                    //attackTargetTr = null;
                    attackCombo++;
                    attackCombo %= 2;
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
    public void ChaseTarget(Transform tr)
    {
        isProvoked = true;
        chaseTargetTr = tr;
        ChangeState(UnitState.Chase);
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
                isProvoked = false;
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetComponent<Outline>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetComponent<Outline>().enabled = false;
    }
}
