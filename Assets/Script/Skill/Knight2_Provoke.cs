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

    public override void EffectOnUnit(Unit unit,Hero hero)
    {
        StartCoroutine(unit.Provoked(hero.transform, data.duration));
    }
    public override void UseSkill(Transform skillTr, Hero hero)
    {
        AoeSkill(skillTr, hero);
    }
    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        yield return null;
        StartCoroutine(PlaySkillOnTr(skillTr));
        UseSkill(skillTr, hero);
    }
}
