using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knight1_sheild : Skill
{
    public override void EffectOnUnit(Unit unit, HeroUnit hero)
    {
        ShieldBuffEffect shieldBuff = new ShieldBuffEffect(unit, 0.5f, data.damage);

        StartCoroutine(unit.StatusEffectCoroutine(shieldBuff));
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 20f;
        data.nonTargetRange = 5f;
        data.damage = 10;
        data.duration = 10f;
    }

    public override IEnumerator SkillCor(Transform skillTr, HeroUnit hero)
    {
        StartCoroutine(PlaySkillOnTr(skillTr));
        StartCoroutine(hero.AddAromor(data.damage * 2, data.duration));
        for (int i = 0; i < data.duration*2; i++)
        {
            AoeSkill(skillTr, hero);
            yield return new WaitForSeconds(0.5f);
        }
    }

}
