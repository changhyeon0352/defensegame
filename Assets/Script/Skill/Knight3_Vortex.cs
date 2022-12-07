using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight3_Vortex : Skill
{
    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 12f;
        data.range = 3f;
        data.damage = 10;
        data.duration = 5f;
        data.order = 2;
    }

    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.TakeDamage(data.damage + hero.Attack);
    }
    public override void AoeSkill(Transform skillTr, Hero hero)
    {
        base.AoeSkill(skillTr, hero);
    }

    public override void UsingSkill(Transform skillTr,Hero hero)
    {
        
    }
    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        yield return null;
    }
}
