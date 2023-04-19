using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage2_Sleep : Skill
{
    public override void EffectOnUnit(Unit unit, HeroUnit hero)
    {
        SleepEffect sleepEffect = new SleepEffect(unit, data.duration);
        StartCoroutine(unit.StatusEffectCoroutine(sleepEffect));
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.AreaTarget;
        data.coolTime = 20f;
        data.range = 8f;
        data.damage = 30;
        data.duration = 10f;
        data.nonTargetRange = 5f;
    }

    public override IEnumerator SkillCor(Transform skillTr, HeroUnit hero)
    {
        StartCoroutine(PlaySkillOnTr(skillTr));
        yield return null;
        AoeSkill(skillTr, hero);
    }
}
