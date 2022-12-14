using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage1_Rockwall : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.NonTarget;
        data.coolTime = 1f;
        data.range = 8f;
        data.nonTargetRange = 5f;
        data.damage = 5;
        data.duration = 12f;
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
