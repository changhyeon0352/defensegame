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
}
public abstract class Skill : MonoBehaviour
{
    public SkillData data;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected GameObject skillPrefab;
    [SerializeField] protected GameObject indicator;
    public GameObject Indicator { get { return indicator; } }
    public GameObject SkillPrefab { get { return skillPrefab; } }
    //private YieldInstruction yield;

    public abstract void InitSetting();
    public abstract void EffectOnUnit(Unit unit,Hero hero);
    public abstract void UseSkill(Transform skillTr,Hero hero);
    public abstract IEnumerator SkillCor(Transform skillTr, Hero hero);


    
    public virtual void AoeSkill(Transform skillTr,Hero hero)
    {
        Collider[] cols = Physics.OverlapSphere(skillTr.position, data.range, targetLayer);
        foreach (Collider col in cols)
        {
            Unit unit = col.gameObject.GetComponent<Unit>();
            EffectOnUnit(unit,hero);
        }
    }
    

}
