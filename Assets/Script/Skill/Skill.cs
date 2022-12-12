using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering;

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
    public LayerMask TargetLayer 
    { get 
        {
            if (data.skillType == SkillType.NonTarget)
                return LayerMask.GetMask("Ground");
                return targetLayer; 
        } 
    }    
    public GameObject SkillPrefab { get { return skillPrefab; } }
    //private YieldInstruction yield;

    public abstract void InitSetting();
    public abstract void EffectOnUnit(Unit unit,Hero hero);
    public abstract void UseSkill(Transform skillTr,Hero hero);
    public abstract IEnumerator SkillCor(Transform skillTr, Hero hero);


    
    public virtual void AoeSkill(Transform skillTr,Hero hero)
    {
        Collider[] cols = Physics.OverlapSphere(skillTr.position, data.nonTargetRange, targetLayer);
        foreach (Collider col in cols)
        {
            Unit unit = col.gameObject.GetComponent<Unit>();
            EffectOnUnit(unit,hero);
        }
    }
    public virtual IEnumerator PlaySkillOnTr(Transform skillTr)
    {
        skillObj = Instantiate(SkillPrefab, skillTr);
        if (data.skillType == SkillType.NonTarget)
        {
            skillObj.transform.parent = null;
        }
        yield return new WaitForSeconds(data.duration);
        Destroy(skillObj);
    }

}
