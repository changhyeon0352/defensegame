using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knight1_sheild : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.ArmorPlus = data.damage;
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 20f;
        data.range = 5f;
        data.damage = 10;
        data.duration = 10f;
        data.order = 0;
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        yield return null;
    }

    public override void UsingSkill( Transform skillTr,Hero hero)
    {
        Collider[] cols = Physics.OverlapSphere(skillTr.position, data.range+2, targetLayer);
        foreach (Collider col in cols)
        {
            Unit unit = col.gameObject.GetComponent<Unit>();
            unit.ArmorPlus = 0;
        }
        hero.ArmorPlus = data.damage*2;
        AoeSkill(skillTr, hero);

    }

}
