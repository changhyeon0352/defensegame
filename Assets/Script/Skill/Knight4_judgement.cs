using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight4_judgement : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.TakeDamage(data.damage + hero.Attack);
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.UnitTarget;
        data.coolTime = 50f;
        data.range = 5f;
        data.damage = 10;
        data.duration = 5f;
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        StartCoroutine(PlaySkillOnTr(skillTr));
        WaitForSeconds sec = new WaitForSeconds(0.1f);
        for (int i = 0; i < 16; i++)
        {
            yield return sec;
            Unit unit =skillTr.GetComponent<Unit>();
            EffectOnUnit(unit, hero);
        }
    }

   
}
