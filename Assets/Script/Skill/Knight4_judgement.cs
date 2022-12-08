using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight4_judgement : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        throw new System.NotImplementedException();
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.Targrt;
        data.coolTime = 50f;
        data.range = 5f;
        data.damage = 10;
        data.duration = 5f;
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        WaitForSeconds sec = new WaitForSeconds(0.1f);
        for (int i = 0; i < 16; i++)
        {
            yield return sec;
            UseSkill(skillTr, hero);
        }
    }

    public override void UseSkill(Transform skillTr, Hero hero)
    {
        skillTr.GetComponent<Unit>().TakeDamage(data.damage + hero.Attack);
    }
}
