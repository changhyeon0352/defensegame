using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage4_dragonBreath : Skill
{
    public override void EffectOnUnit(Unit unit, HeroUnit hero)
    {
        unit.TakeDamage(data.damage + unit.UnitData.MagicPower / 2);
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.directional;
        data.coolTime = 40f;
        data.range = 8f;
        data.nonTargetRange = 1f;
        data.damage = 10;
        data.duration = 7f;
        data.Name = "용의 숨결";
    }

    public override IEnumerator SkillCor(Transform skillTr, HeroUnit hero)
    {
        Hero_Mage hero_Mage = (Hero_Mage)hero;
        UIMgr.Instance.ShowDuration(data.Name, data.duration);
        hero_Mage.MoveSpots(hero.transform.position);
        hero_Mage.IsStopSkill = true;
        StartCoroutine(PlaySkillOnTr(skillTr));
        yield return new WaitForSeconds(data.duration);
        hero_Mage.IsStopSkill = false;
    }

    
}
