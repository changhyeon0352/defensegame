using System.Collections;
using UnityEditor;
using UnityEngine;

public class Hero : AllyUnit
{
    private HeroData heroData;
    public HeroData HeroData { get => heroData;
        set { heroData = value;
        }
    }
    override public void InitializeUnitStat()
    {
        unitData.HeroData= heroData;
        base.InitializeUnitStat();
    }
    private bool isStopSkill=false;
    public bool IsStopSkill { get => isStopSkill; set { isStopSkill = value; agent.SetDestination(goalTr.position); } }
    public HeroState heroState;
    bool isSelected = false;
    public bool IsSelected{ get => isSelected; }
    [SerializeField] GameObject energyBoltPrefab;
    [SerializeField] Transform tipOfStaff;
    public float rangeforGizmo;
    
    float[] skillCoolsLeft = { -1, -1, -1, -1 };
    public float[] SkillCoolLeft { get => skillCoolsLeft; }
    float[] skillCools = new float[4];
    public float[] SkillCools { get => skillCools; }
    bool[] skillCanUse = { true, true, true, true };
    public bool[] SkillCanUse { get { return skillCanUse; } }
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
 
    public override int Hp
    {
        get => hp;
        set 
        { 
            hp = value;
            if (hp <= 0)
            {
                hp = 0;
                //죽음
                if (state != UnitState.Dead)
                {
                    Die();
                    DataMgr.Instance.HeroDie(this);
                    GameMgr.Instance.defenseMgr.ChangeHeroAfterDie();
                }
            }
            heroState.ShowHpMp(this);
        }
    }
    public new int Mp
    {
        get => mp;
        set
        {
            mp = value;
            heroState.ShowHpMp(this);
        }
    }

    protected override void Update()
    {
        if(isStopSkill)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Ground")))
            {
                Vector3 mouseDir=new Vector3(hit.point.x,transform.position.y,hit.point.z)-transform.position;
                float dot = Vector3.Dot(Vector3.Cross(transform.forward, mouseDir).normalized, Vector3.up);
                transform.Rotate(transform.up * dot*0.1f);
                return;
            }
        }
        base.Update();
    }

    //무브업데이트땐 적추적 ㄴㄴ
    public static Hero FindHero(HeroData herodata)
    {
        Hero result = new();
        Hero[] heros=FindObjectsOfType<Hero>();
        foreach(Hero hero in heros)
        {
            if(hero.heroData == herodata)
            {
                result = hero;
            }
        }
        return result;
    }
    
    override protected void ChaseUpdate()
    {

        if ((isProvoked && chaseTargetTr != null) || SearchAndChase(searchRange)) //도발되었고 쫒는놈이 있다면 혹은 서칭거리안에 있다면
        {
            agent.SetDestination(chaseTargetTr.position);
        }
        else
        {
            ChangeState(UnitState.Move);
        }
        //스킬 범위 밖에 스킬을 사용한 경우 스킬범위 안으로 온다면 스킬사용
        if (GameMgr.Instance.skillController.IsChasingForSkill && agent.remainingDistance < GameMgr.Instance.skillController.SkillRange && !agent.pathPending)
        {
            GameMgr.Instance.skillController.UseClinkingSkill(chaseTargetTr);
            MoveSpots(transform.position);
        }
        else if (agent.remainingDistance < attackRange && !agent.pathPending&&chaseTargetTr.CompareTag("Monster"))
        {
            ChangeState(UnitState.Attack);
        }
    }
    public void MoveSpots(Vector3 position)
    {
        UnitGroup unitgroup =transform.parent.parent.GetComponent<UnitGroup>();
        unitgroup.spotsTr.position = position;
        ChangeState(UnitState.Move);
    }
    public void SetChaseTarget(Transform targetTr)
    {
        chaseTargetTr = targetTr;
    }
    public void RangeAttack()
    {
        GameObject obj=Instantiate(energyBoltPrefab,tipOfStaff.position,Quaternion.identity);
        obj.GetComponent<EnergyBolt>().SetTargetAndDamage(attackTargetTr, attack);

    }
    public void SkillAnimation(bool isMaintain,float sec)
    {
        if (isMaintain)
            StartCoroutine(MaintainCor(sec));
        else
            anim.SetTrigger("UseSkill");
    }
    //스킬쓸때 자세유지
    IEnumerator MaintainCor(float sec)
    {
        anim.SetBool("MaintainSkill", true);
        yield return new WaitForSeconds(sec);
        anim.SetBool("MaintainSkill", false);
    }    
   
}
