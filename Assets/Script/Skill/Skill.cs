using System.Collections;
using UnityEngine;

public struct SkillData
{
    public SkillType skillType;
    public float coolTime;
    public float range;
    public float duration;
    public int damage;
    public float nonTargetRange;
    public string Name;
}
public abstract class Skill : MonoBehaviour
{
    public SkillData data;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected GameObject skillPrefab;
    [SerializeField] protected GameObject indicator;
    protected GameObject skillObj;
    public GameObject Indicator { get { return indicator; } }
    // AreaTarget 타입은 targetlayer는 유닛이지만 스킬사용시 땅을 클릭해야해서 예외처리
    public LayerMask TargetLayer 
    { get 
        {
            if (data.skillType == SkillType.AreaTarget)
                return LayerMask.GetMask("Ground");
            return targetLayer; 
        } 
    }    
    public GameObject SkillPrefab { get { return skillPrefab; } }
    //스킬 데이터 세팅 (지속시간,범위,데미지...)
    public abstract void InitSetting();
    //타겟이 되는 유닛에 어떠한 효과를 가하는 함수
    public abstract void EffectOnUnit(Unit unit,HeroUnit hero);
    //스킬을 사용할 때 실행되는 모든 함수를 담음 SkillController에서 실행되는 함수
    public abstract IEnumerator SkillCor(Transform skillTr, HeroUnit hero);
    //원형 범위의 적 유닛에 EffectOnUnit을 실행함
    public virtual void AoeSkill(Transform skillTr,HeroUnit hero)
    {
        Collider[] cols = Physics.OverlapSphere(skillTr.position,data.nonTargetRange, targetLayer);
        foreach (Collider col in cols)
        {
            Unit unit = col.gameObject.GetComponent<Unit>();
            if(!unit.IsDead)
                EffectOnUnit(unit,hero);
        }
    }
    //스킬 프리펩을 생성함
    public virtual IEnumerator PlaySkillOnTr(Transform skillTr)
    {
        skillObj = Instantiate(SkillPrefab, skillTr);
        if (data.skillType == SkillType.AreaTarget)
        {
            skillObj.transform.parent = null;
        }
        yield return new WaitForSeconds(data.duration);
        Destroy(skillObj);
    }
}
