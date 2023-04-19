using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight2_Provoke : Skill
{
    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 20f;
        data.nonTargetRange = 5f;
        data.damage = 0;
        data.duration = 10f;
    }

    public override void EffectOnUnit(Unit unit,HeroUnit hero)
    {
        ProvokedEffect provokedEffect =new ProvokedEffect(unit, data.duration, hero.transform);
        StartCoroutine(unit.StatusEffectCoroutine(provokedEffect));
    }
    public override IEnumerator SkillCor(Transform skillTr, HeroUnit hero)
    {
        yield return null;
        StartCoroutine(PlaySkillOnTr(skillTr));
        AoeSkill(skillTr, hero);
    }
}
