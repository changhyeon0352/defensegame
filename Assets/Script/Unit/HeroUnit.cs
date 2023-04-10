using System.Collections;
using UnityEditor;
using UnityEngine;

public abstract class HeroUnit : MovableUnit
{
    bool isAttackMove=true;
    private HeroData heroData;
    protected HeroUnitController controller;
    public HeroData HeroData { get => heroData;}
    bool isSelected = false;
    public bool IsSelected{ get => isSelected; }

    [SerializeField]
    GameObject heroStatePrefab;
    float[] skillCoolsLeft = { -1, -1, -1, -1 };
    public float[] SkillCoolLeft { get => skillCoolsLeft; }
    float[] skillCools = new float[4];
    public float[] SkillCools { get => skillCools; }
    bool[] skillCanUse = { true, true, true, true };
    private HeroState heroState;
    public bool[] SkillCanUse { get { return skillCanUse; } }
    public override int Hp
    {
        get => hp;
        protected set
        {
            hp = value;
            UpdateHPbar(hp, currentStat.hpMax);
            UpdateHeroHp(Hp, currentStat.hpMax);
        }
    }


    public new int Mp
    {
        get => mp;
    }

    override protected void Awake()
    {
        base.Awake();
        controller = GetComponent<HeroUnitController>();
    }
    public void SetHeroData(HeroData data)
    {
        heroData = data;
        unitData.HeroData = heroData;
    }
    public IEnumerator SkillCoolCor(int num,float coolTime)
    {
        skillCanUse[num] = false;
        skillCools[num] = coolTime;
        skillCoolsLeft[num]=coolTime;
        for (int i=0; i<10000;i++)
        {
            skillCoolsLeft[num]-=Time.deltaTime;
            if (skillCoolsLeft[num] < 0)
            {
                skillCanUse[num]=true;
                Debug.Log($"{num + 1}번째 스킬사용가능");
                break;
            }
                
            yield return null;
        }
    }
    public bool CheckSkillCool(int skillNum)
    {
        bool result = false;
        if (skillCoolsLeft[skillNum]<0)
        {
            result = true;
        }
        return result;
    }
    
    

    //무브업데이트땐 적추적 ㄴㄴ
    public static HeroUnit FindHero(HeroData herodata)
    {
        HeroUnit[] heros=FindObjectsOfType<HeroUnit>();
        HeroUnit result = heros[0];
        foreach(HeroUnit hero in heros)
        {
            if(hero.heroData == herodata)
            {
                result = hero;
            }
        }
        return result;
    }

    protected override void MoveUpdate()
    {
        if(!isAttackMove)
        {
            navMesh.SetDestination(goalTr.position);
            if(navMesh.remainingDistance<0.1f&&!navMesh.pathPending)
            {
                isAttackMove= true;
            }
        }
        else
            base.MoveUpdate();  
    }
    protected override void ChaseUpdate()
    {
        if (GameMgr.Instance.skillController.IsChasingForSkill)
        {
            navMesh.SetDestination(targetCol.transform.position);
            if(navMesh.remainingDistance < GameMgr.Instance.skillController.SkillRange && !navMesh.pathPending)
            {
                GameMgr.Instance.skillController.UseClinkingSkill(targetCol.transform);
                MoveSpots(transform.position);
            }
        }
        else
            base.ChaseUpdate();
    }
    public void MoveSpots(Vector3 position)
    {
        UnitGroup unitgroup =transform.parent.parent.GetComponent<UnitGroup>();
        unitgroup.SetSpots(position);
        ChangeState(UnitState.Move);
        isAttackMove = false;
    }
    public void SetTarget(Transform targetTr)
    {
        targetCol = targetTr.GetComponent<Collider>();
    }
    
    public void SkillAnimation(bool isMaintain,float sec)
    {
        if (isMaintain)
            StartCoroutine(MaintainCor(sec));
        //else
            //anim.SetTrigger("UseSkill");
    }
    //스킬쓸때 자세유지
    IEnumerator MaintainCor(float sec)
    {
        //anim.SetBool("MaintainSkill", true);
        yield return new WaitForSeconds(sec);
        //anim.SetBool("MaintainSkill", false);
    }


    
    protected override void Die()
    {
        base.Die();
        DataMgr.Instance.HeroDie(this);
        GameMgr.Instance.heroController.ChangeHeroAfterDie(this);
    }
    public void MakeHeroStateUI()
    {
        Transform heroStateParent = UIMgr.Instance.DefenseUITr.Find("HeroStateList");
        heroState = Instantiate(heroStatePrefab, heroStateParent).GetComponent<HeroState>();
        heroState.InitializeHeroState(heroData);
    }
    public void UpdateHeroHp(int hp, int hpMax)
    {
        heroState.UpdateHp(hp, hpMax);
    }
    public void ShowSelectedState()
    {
        heroState.SelectedHeroUI();
    }
}
