using System.Collections;
using UnityEditor;
using UnityEngine;

public abstract class HeroUnit : MovableUnit
{
    
    private HeroData heroData;
    public HeroData HeroData { get => heroData;}

    [SerializeField]
    GameObject heroStatePrefab;
    float[] skillCoolsLeft = { -1, -1, -1, -1 };
    public float[] SkillCoolLeft { get => skillCoolsLeft; }
    float[] skillCools = new float[4];
    public float[] SkillCools { get => skillCools; }
    bool[] skillCanUse = { true, true, true, true };
    private HeroState heroState;
    private SkillController skillController;
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
    }
    public void SetSkillController(SkillController skillController)
    {
        this.skillController = skillController;
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
    
    

    protected override void ChaseUpdate()
    {
        if (skillController.IsChasingForSkill)
        {
            navMesh.SetDestination(targetCol.transform.position);
            if(navMesh.remainingDistance < skillController.SkillRange && !navMesh.pathPending)
            {
                skillController.UseSkillToTarget(targetCol.transform);
                MoveSpot(transform.position,false);
            }
        }
        else
            base.ChaseUpdate();
    }
    public void MoveSpot(Vector3 position,bool isAttackMove)
    {
        UnitGroup unitgroup =transform.parent.parent.GetComponent<UnitGroup>();
        unitgroup.SetSpots(position);
        ChangeState(UnitState.Move);
        this.isAttackMove = isAttackMove;
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
        FindObjectOfType<HeroUnitController>().ChangeHeroAfterDie(this);
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
