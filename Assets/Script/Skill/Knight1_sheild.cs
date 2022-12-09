using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knight1_sheild : Skill
{
    public override void EffectOnUnit(Unit unit, Hero hero)
    {
        unit.ArmorPlus = data.damage;
    }

    public override void InitSetting()
    {
        data.skillType = SkillType.OnHero;
        data.coolTime = 20f;
        data.nonTargetRange = 5f;
        data.damage = 10;
        data.duration = 10f;
    }

    public override IEnumerator SkillCor(Transform skillTr, Hero hero)
    {
        double timer = data.duration;
        while(true)
        {
            timer -= Time.deltaTime;
            UseSkill(skillTr, hero);
            yield return null;
            if (timer < 0)
            {
                Collider[] cols = Physics.OverlapSphere(skillTr.position, data.nonTargetRange + 2, targetLayer);
                foreach (Collider col in cols)
                {
                    Unit unit = col.gameObject.GetComponent<Unit>();
                    unit.ArmorPlus = 0;
                }
                break;
            }
        }
    }

    public override void UseSkill( Transform skillTr,Hero hero)
    {
        Collider[] cols = Physics.OverlapSphere(skillTr.position, data.nonTargetRange+2, targetLayer);
        foreach (Collider col in cols)
        {
            Unit unit = col.gameObject.GetComponent<Unit>();
            unit.ArmorPlus = 0;
        }
        AoeSkill(skillTr, hero);
        hero.ArmorPlus = data.damage * 2;
    }

}
