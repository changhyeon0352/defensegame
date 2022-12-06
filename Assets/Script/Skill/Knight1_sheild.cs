using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Knight1_sheild : Skill
{
    public override void InitSetting()
    {
        data.skillMode = SkillMode.OnHero;
        data.coolTime = 20f;
        data.range = 5f;
        data.damage = 10;
        data.duration = 10f;
        data.order = 0;
        data.skillEffect = skillPrefab;
    }
    IEnumerator ShieldAuraCor()// 히어로에 방어력 적용이 안됨
    {
        for (int i = 0; i < 10000; i++)
        {
            //SetShiedPlus(data.range + 2, 0);
            //SetShiedPlus(data.range, 10);
            yield return null;
        }
    }
    public void SetShiedPlus(Transform heroTr,float range, int armorPlus)
    {
        Collider[] cols = Physics.OverlapSphere(heroTr.position, range,targetLayer );
        foreach (Collider col in cols)
        {
            AllyUnit ally = col.gameObject.GetComponent<AllyUnit>();
            ally.ArmorPlus = armorPlus;
        }
        GameMgr.Instance.commandMgr.SelectedHero.ArmorPlus = armorPlus * 2;
    }
}
