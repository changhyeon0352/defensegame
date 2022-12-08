using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage2_Sleep : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        StartCoroutine(unit.Sleep(data.duration));
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.NonTarget;
        data.coolTime = 1f;
        data.range = 8f;
        data.damage = 30;
        data.duration = 10f;
        data.nonTargetRange = 5f;
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        yield return null;
        UseSkill(skillTr, hero);
    }

    public override void UseSkill(Transform skillTr, Hero hero)
    {
        AoeSkill(skillTr, hero);
    }
}
