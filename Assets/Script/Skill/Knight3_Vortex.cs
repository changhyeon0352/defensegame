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

    public override void EffectOnUnit(Unit unit, HeroUnit hero)
    {
        unit.TakeDamage(data.damage + hero.AttackPoint);
    }

    
    public override IEnumerator SkillCor(Transform skillTr, HeroUnit hero)
    {
        StartCoroutine(PlaySkillOnTr(skillTr));//스킬 이펙트 오브젝트 생성후 삭제
        for (int i=0;i<data.duration;i++)
        {
            AoeSkill(skillTr, hero);//범위 내의 타겟에게  EffextOnUnit()실행
            yield return new WaitForSeconds(1);
        }
        
    }
}
