using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage3_earthquake : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.TakeDamage(data.damage + unit.unitData.MagicPower/2);
        StartCoroutine(unit.Slow(1f));
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.NonTarget;
        data.coolTime = 1f;
        data.range = 8f;
        data.nonTargetRange = 5f;
        data.damage = 5;
        data.duration = 10f;// playskillontr을 반복하기 위해서
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < data.duration; i++)
        {
            UseSkill(skillTr, hero);
            yield return new WaitForSeconds(1f);
        }
    }

    public override void UseSkill(Transform skillTr, Hero hero)
    {
        AoeSkill(skillTr, hero);
    }
}
