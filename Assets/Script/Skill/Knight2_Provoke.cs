using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight2_Provoke : Skill
{
    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 20f;
        data.range = 5f;
        data.damage = 0;
        data.duration = 10f;
        data.order = 0;
    }

    public override void EffectOnUnit(Unit unit,Hero hero)
    {
        StartCoroutine(unit.ProvokedBy(GameMgr.Instance.commandMgr.SelectedHero.transform, data.duration));
    }
    public override void AoeSkill(Transform skillTr, Hero hero)
    {
        base.AoeSkill(skillTr, hero);
    }
    public override void UsingSkill(Transform skillTr, Hero hero)
    {

    }
    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        yield return null;
    }
}
