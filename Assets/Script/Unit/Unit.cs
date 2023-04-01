/*유닛의 기본적인 움직임을 제어하는 클래스 (유한상태머신)
 Ally, Monster 등의 유닛관련 클래스의 최상위 부모 클래스*/
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using static UnityEngine.Rendering.DebugUI;


public struct UnitStat
{
    public int hp;
    public int hpMax;
    public int mp;
    public int mpMax;
    public int attackPoint;
    public int armor;
    public float searchRange;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
}


public abstract class Unit : MonoBehaviour, IHealth
{
    protected UnitController controller;
    protected UnitData unitData;
    public UnitData UnitData { get { return unitData; } }
    protected UnitStat unitStat;
    public UnitStat UnitStat { get { return unitStat; } }
    protected NavMeshAgent navMesh;
    protected Transform chaseTargetTr;
    protected Transform attackTargetTr;
    protected UnitState state = UnitState.Idle;
    public UnitState State { get { return state; } }
    public LayerMask enemyLayer;
    public bool isProvoked = false;
    public bool isSleep = false;
    protected float lastAttackTime=0;
    protected Animator anim;
    protected Rigidbody rb;
    protected Collider col;
    [SerializeField]
    protected Transform goalTr;
    protected float attackCooldown;
    protected StatusEffect statusEffect;

    public float AttackSpeed { get { return unitStat.attackSpeed; }}
    public bool IsDead { get => state == UnitState.Dead; }
    public float MoveSpeed
    {
        get { return unitStat.moveSpeed; }
        private set
        {
            unitStat.moveSpeed = value;
            navMesh.speed = value;
        }
    }
    public virtual int Hp
    {
        get => unitStat.hp;
        private set{ unitStat.hp = value;}
    }
    public int AttackPoint { get => unitStat.attackPoint; }
    public int Armor { get => unitStat.armor; set => unitStat.armor = value; }
    public int HpMax { get => unitStat.hpMax; }
    public int Mp { get => unitStat.mp; }
    public int MpMax { get => unitStat.mpMax; }




    //=====================================================================================================================
    virtual protected void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        controller = GetComponent<UnitController>();
        anim = GetComponent<Animator>();
    }
    virtual protected void Update()
    {
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
    virtual public void InitializeUnitStat()
    {
        unitStat.hpMax = unitData.HP;
        unitStat.hp = unitStat.hpMax;
        unitStat.mpMax = unitData.MP;
        unitStat.mp = unitStat.mpMax;
        unitStat.attackPoint = unitData.Atk;
        unitStat.armor = unitData.Armor;
        MoveSpeed = unitData.MoveSpeed;
        unitStat.attackRange = unitData.AttackRange;
        SetAttackSpeed(unitData.AttackSpeed);
        unitStat.searchRange = unitData.SearchRange;
        ChangeState(UnitState.Move);
        navMesh.enabled = true;
    }


    public IEnumerator AddAromor(int armorPlus, float sec)
    {
        yield return new WaitForSeconds(sec);//임시
        //if (unitData.Armor + armorPlus > unitStat.armor)
        //{
        //    unitStat.armor = unitData.Armor + armorPlus;
        //    if (!shieldbuff.isPlaying)
        //        shieldbuff.Play();
        //    //UIMgr.Instance.unitStatUI.isRefresh = true;
        //    yield return new WaitForSeconds(sec);
        //    unitStat.armor = unitData.Armor;
        //    shieldbuff.Stop();
        //    //UIMgr.Instance.unitStatUI.isRefresh = true;
        //}
    }
    public IEnumerator Provoked(Transform tr, float sec)
    {
        chaseTargetTr = tr;
        ChangeState(UnitState.Chase);
        isProvoked = true;
        GameObject obj = Instantiate(GameMgr.Instance.skillController.Buffs[(int)Buff.provoked], transform);
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
        //anim.SetTrigger("Sleep");
        navMesh.enabled = false;
        GameObject obj = Instantiate(GameMgr.Instance.skillController.Buffs[(int)Buff.sleep], transform);
        while (count > 0)
        {
            count -= Time.deltaTime;
            yield return null;
            if (!isSleep)
                break;
        }
        if (state != UnitState.Dead)
        {
            navMesh.enabled = true;
            isSleep = false;
            //anim.SetTrigger("WakeUp");
            ChangeState(UnitState.Move);
        }
        Destroy(obj);
    }
    public IEnumerator Slow(float sec)
    {
        MoveSpeed = unitData.MoveSpeed / 4;
        UIMgr.Instance.unitStatUI.isRefresh = true;
        yield return new WaitForSeconds(sec);
        if (state != UnitState.Dead)
            MoveSpeed = unitData.MoveSpeed;
        UIMgr.Instance.unitStatUI.isRefresh = true;
    }

    public void SetAttackSpeed(float speed)
    {
        unitStat.attackSpeed = speed;
        attackCooldown = speed == 0 ? float.MaxValue : 1 / speed;
        anim.SetFloat("attackSpeed", UnitData.AttackAniLength[0] * UnitStat.attackSpeed * 1.5f);
    }
    public void SetUnitData(UnitData unitData )
    {
        this.unitData = unitData;
    }

    virtual protected void Die()
    {
        col.enabled = false;
        navMesh.enabled = false;
        ChangeState(UnitState.Dead);
        ParticleSystem[] psArray = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in psArray)
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
        if (state == UnitState.Dead)
            return;
        double decreaseRate = 1 - Math.Atan((double)(unitStat.armor) / 50) / (Math.PI / 2);//아머가 50쯤 되면 50%
        int netDamage = (int)(damage * decreaseRate);
        if (netDamage > Hp)
        {
            Hp = 0;
            Die();
        }
        else
            Hp -= ((netDamage == 0)? 1 : netDamage);
        
        isSleep = false;
        controller.OnHPChanged?.Invoke();
    }



    //==============================================================================
   
    abstract protected void IdleUpdate();
    abstract protected void MoveUpdate();
    abstract protected void ChaseUpdate();
    abstract protected void AttackUpdate();
    abstract public void Attack();
    
    //=================================================================================
    //public bool SearchAndChase(float radius)
    //{
    //    bool result = false;
    //    Transform enemyTr = SearchEnemy(radius);   //적 검색
    //    if (enemyTr != null)
    //    {
    //        chaseTargetTr = enemyTr;                    //적을 표적으로
    //        result = true;
    //    }
    //    return result;
    //}
    public Transform SearchEnemyInRange(float radius)
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
                break;
            case UnitState.Move:
                isProvoked = false;
                chaseTargetTr = null;
                navMesh.stoppingDistance = 0;
                break;
            case UnitState.Chase:
                navMesh.stoppingDistance = unitStat.attackRange;
                break;
            case UnitState.Attack:
                attackTargetTr = chaseTargetTr;
                chaseTargetTr = null;
                break;
            case UnitState.Dead:
                break;
            default:
                break;
        }
        state = newState;
        anim.SetInteger("iState", (int)state);
    }
    private IEnumerator EffectCoroutine()
    {
        statusEffect.StartEffect();

        float elapsed = 0f;
        while (elapsed < statusEffect.Duration)
        {
            statusEffect.UpdateEffect();
            elapsed += Time.deltaTime;
            yield return null;
        }

        statusEffect.EndEffect();
        statusEffect = null;
    }
    public void SetGoalSpot(Transform tr)
    {
        goalTr = tr;
    }
}
