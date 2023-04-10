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
    public int hpMax;
    public int mpMax;
    public int attackPoint;
    public int armor;
    public float searchRange;
    public float attackRange;
    public float attackSpeed;
    public float moveSpeed;
}


public abstract class Unit : MonoBehaviour, IHealth, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    protected UnitData unitData;
    public UnitData UnitData { get { return unitData; } }
    protected UnitStat currentStat;
    protected UnitStat permanentStat;
    public UnitStat CurrentStat { get { return currentStat; } }
    public UnitStat PermanentStat { get { return permanentStat; } }
    protected NavMeshAgent navMesh;
    protected Collider targetCol;
    protected UnitState state = UnitState.Idle;
    public UnitState State { get { return state; } }
    public LayerMask enemyLayer;
    protected float lastAttackTime=0;
    protected Animator anim;
    protected Rigidbody rb;
    protected Collider col;
    [SerializeField]
    protected Transform goalTr;
    protected float attackCooldown;
    protected StatusEffect statusEffect;
    protected int hp;
    protected int mp;
    public bool isProvoked = false;
    public bool isSleep = false;
    private UnitHpBar unitHpBar;
    [SerializeField]
    protected ParticleSystem selectEffect;

    public float AttackSpeed { get { return currentStat.attackSpeed; }}
    public bool IsDead { get => state == UnitState.Dead; }
    public float MoveSpeed
    {
        get { return currentStat.moveSpeed; }
        private set
        {
            currentStat.moveSpeed = value;
            navMesh.speed = value;
        }
    }
    public virtual int Hp
    {
        get => hp;
        protected set{ hp = value;
            UpdateHPbar(hp, currentStat.hpMax);
        }
    }
    public int AttackPoint { get => currentStat.attackPoint; }
    public int Armor { get => currentStat.armor; set => currentStat.armor = value; }
    public int HpMax { get => currentStat.hpMax; }
    public int Mp { get => mp; }
    public int MpMax { get => currentStat.mpMax; }




    //=====================================================================================================================
    virtual protected void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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
        permanentStat.hpMax = unitData.HP;
        permanentStat.mpMax = unitData.MP;
        permanentStat.attackPoint = unitData.Atk;
        permanentStat.armor = unitData.Armor;
        permanentStat.moveSpeed = unitData.MoveSpeed;
        permanentStat.attackRange = unitData.AttackRange;
        permanentStat.searchRange = unitData.SearchRange;
        permanentStat.attackSpeed=unitData.AttackSpeed;
        if (unitData.Type == UnitType.soldier_Range)
        {
            permanentStat.attackPoint += DataMgr.Instance.GetBarracksUpgradeValue(BarracksUpgradeType.range_damage);
        }
        else if (unitData.Type == UnitType.hero)
        { 
            HeroUnit hero = GetComponent<HeroUnit>();
            permanentStat.attackPoint += DataMgr.Instance.blacksmithData.values[hero.HeroData.level_Weapon];
            permanentStat.armor += DataMgr.Instance.blacksmithData.values[5 + hero.HeroData.level_Armor];
        }
        else if (unitData.Type == UnitType.soldier_Melee)
        {
            permanentStat.armor += DataMgr.Instance.GetBarracksUpgradeValue(BarracksUpgradeType.melee_armor);
        }
        navMesh.enabled = true;
        currentStat.hpMax = permanentStat.hpMax;
        currentStat.mpMax = permanentStat.mpMax;
        currentStat.attackPoint = permanentStat.attackPoint;
        currentStat.armor = permanentStat.armor;
        currentStat.attackRange = permanentStat.attackRange;
        currentStat.searchRange = permanentStat.searchRange;
        MoveSpeed = permanentStat.moveSpeed;
        SetAttackSpeed(permanentStat.attackSpeed);
        ChangeState(UnitState.Move);
        hp = currentStat.hpMax;
        mp = currentStat.mpMax;
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
        targetCol = tr.GetComponent<Collider>();
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
        yield return new WaitForSeconds(sec);
        if (state != UnitState.Dead)
            MoveSpeed = unitData.MoveSpeed;
    }

    public void SetAttackSpeed(float speed)
    {
        currentStat.attackSpeed = speed;
        attackCooldown = speed == 0 ? float.MaxValue : 1 / speed;
        anim.SetFloat("attackSpeed", UnitData.AttackAniLength[0] * CurrentStat.attackSpeed * 1.5f);
    }
    public void SetUnitData(UnitData unitData )
    {
        this.unitData = unitData;
    }

    virtual protected void Die()
    {
        FindObjectOfType<UnitHpbarPool>().ReturnObjectToPool(unitHpBar.gameObject);
        anim.SetTrigger("Dead");
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
    protected virtual IEnumerator DieFallCor()
    {
        if (state == UnitState.Dead)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.drag = 20;
            yield return new WaitForSeconds(1);
            Destroy(this);
        }
    }
    

    public void TakeDamage(int damage)
    {
        if (state == UnitState.Dead)
            return;
        double decreaseRate = 1 - Math.Atan((double)(currentStat.armor) / 50) / (Math.PI / 2);//아머가 50쯤 되면 50%
        int netDamage = (int)(damage * decreaseRate);
        if (netDamage >= Hp)
        {
            Hp = 0;
            Die();
        }
        else
            Hp -= ((netDamage == 0)? 1 : netDamage);
        
        isSleep = false;
    }



    //==============================================================================
   
    abstract protected void IdleUpdate();
    abstract protected void MoveUpdate();
    abstract protected void ChaseUpdate();
    abstract protected void AttackUpdate();
    abstract public void Attack();
    
    //=================================================================================
    
    public Collider SearchEnemyInRange(float radius)
    {
        Collider enemyCol = null;
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        float minimunDistance = float.MaxValue;
        foreach (Collider col in cols)
        {
            float distance = Vector3.SqrMagnitude(transform.position - col.transform.position);
            minimunDistance = Mathf.Min(distance, minimunDistance);
            if (minimunDistance == distance)
            {
                enemyCol = col;
            }
        }
        return enemyCol;
    }
    public void ChaseTarget(Transform tr)
    {
        targetCol=tr.GetComponent<Collider>();
        isProvoked = true;
        ChangeState(UnitState.Chase);
    }
    public void ChaseTarget()
    {
        isProvoked = true;
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

                break;
        }
        switch (newState)
        {
            case UnitState.Idle:
                break;
            case UnitState.Move:
                isProvoked = false;
                navMesh.stoppingDistance = 0;
                break;
            case UnitState.Chase:
                navMesh.stoppingDistance = currentStat.attackRange;
                break;
            case UnitState.Attack:
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

    public void InitUnitHpbar()
    {
        Transform unitHpbarParent = FindObjectOfType<HPbars>().transform;
        unitHpBar = FindObjectOfType<UnitHpbarPool>().GetObjectFromPool().GetComponent<UnitHpBar>();
        unitHpBar.transform.SetParent(unitHpbarParent);
        //unitHpBar = Instantiate(unitHpbarPrefab, unitHpbarParent).GetComponent<UnitHpBar>();
        unitHpBar.SetUnitInfo(this);
    }

    public void UpdateHPbar(int hp, int hpMax)
    {
        unitHpBar.UpdateHpBar(hp, hpMax);
    }

    public void ShowSelectEffect()
    {
        selectEffect.Play();
    }
    public void HideSelectEffect()
    {
        selectEffect.Stop();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetComponent<Outline>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetComponent<Outline>().enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIMgr.Instance.unitStatUI.gameObject.SetActive(true);
        UIMgr.Instance.unitStatUI.RefreshUnitStatWindow(this);
    }
    
}
