using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage4_dragonBreath : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.TakeDamage(data.damage + unit.unitData.MagicPower / 2);
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.directional;
        data.coolTime = 1f;
        data.range = 8f;
        data.nonTargetRange = 1f;
        data.damage = 10;
        data.duration = 10f;// playskillontr을 반복하기 위해서
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        StartCoroutine(PlaySkillOnTr(skillTr));
        yield return null;
    }

    public override void UseSkill(Transform skillTr, Hero hero)
    {
        
    }
}
