using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Knight : HeroUnit
{
    public override void Attack()
    {
        if (targetCol.transform != null&& targetCol.enabled)
        {
            IHealth Enemy_IHealth = targetCol.transform.GetComponent<IHealth>();
            Enemy_IHealth.TakeDamage(AttackPoint);
        }
    }
}
