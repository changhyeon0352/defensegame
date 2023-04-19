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
using Unity.VisualScripting;

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
    protected int hp;
    protected int mp;
    public bool isProvoked = false;
    public bool isSleep = false;
    private UnitGroup ownUnitGoup;
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
    public void SetUnitGroup(UnitGroup group)
    {
        ownUnitGoup = group;
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
    public void Sleep(bool isSleep)
    {
        this.isSleep = isSleep;
        navMesh.enabled = !isSleep;
    }
    public void MultiplyMoveSpeed(float multiple)
    {
        MoveSpeed=permanentStat.moveSpeed*multiple;
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
        if (unitHpBar == null)
        {
            int a = 0;
        }    
        GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Hpbar).ReturnObjectToPool(unitHpBar.gameObject);
        anim.SetTrigger("Dead");
        col.enabled = false;
        navMesh.enabled = false;
        ChangeState(UnitState.Dead);
        ParticleSystem[] psArray = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in psArray)
        {
            ps.Stop();
        }
        this.transform.parent = null;
        StartCoroutine(DieFallCor());
        
    }
    protected virtual IEnumerator DieFallCor()
    {
        ownUnitGoup.RemoveUnitFromList(this);
        yield return new WaitForSeconds(1);
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.drag = 20;
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
        
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
        if(targetCol==null)
        {
            SphereCollider col = tr.AddComponent<SphereCollider>();
            col.radius = 0.5f;
            col.isTrigger = true;
            targetCol = col;
        }
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
    public IEnumerator StatusEffectCoroutine(StatusEffect statusEffect)
    {
        if (statusEffect.PsPool!=null)
        {
            GameObject psObj = statusEffect.PsPool.GetObjectFromPool();
            psObj.transform.SetParent(transform);
            psObj.transform.position=transform.position+Vector3.up;
            psObj.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            psObj.SetActive(true);
            statusEffect.StartEffect();
            float elapsed = 0f;
            while (elapsed < statusEffect.Duration&& !statusEffect.IsEnd)
            {
                statusEffect.UpdateEffect();
                elapsed += Time.deltaTime;
                yield return null;
            }
            statusEffect.PsPool.ReturnObjectToPool(psObj);
        }
        else
        {
            statusEffect.StartEffect();
            float elapsed = 0f;
            while (elapsed < statusEffect.Duration&&!statusEffect.IsEnd)
            {
                statusEffect.UpdateEffect();
                elapsed += Time.deltaTime;
                yield return null;
            }
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
        GameObject obj = GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Hpbar).GetObjectFromPool();
        obj.SetActive(true);
        unitHpBar = obj.GetComponent<UnitHpBar>();
        unitHpBar.transform.SetParent(unitHpbarParent);
        //unitHpBar = Instantiate(unitHpbarPrefab, unitHpbarParent).GetComponent<UnitHpBar>();
        unitHpBar.SetUnitInfo(this);
    }

    protected void UpdateHPbar(int hp, int hpMax)
    {
        if(unitHpBar!=null)
        {
            unitHpBar.UpdateHpBar(hp, hpMax);
        }
        
    }

    public void ShowSelectEffect()
    {
        selectEffect.gameObject.SetActive(true);
    }
    public void HideSelectEffect()
    {
        selectEffect.gameObject.SetActive(false);
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
