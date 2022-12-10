using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight3_Vortex : Skill
{
    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 12f;
        data.nonTargetRange = 3f;
        data.damage = 10;
        data.duration = 5f;
    }

    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.TakeDamage(data.damage + hero.Attack);
    }

    public override void UseSkill(Transform skillTr,Hero hero)
    {
        AoeSkill(skillTr, hero);
    }
    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        for(int i=0;i<data.duration;i++)
        {
            UseSkill(skillTr, hero);
            yield return new WaitForSeconds(1);
        }
        
    }
}
