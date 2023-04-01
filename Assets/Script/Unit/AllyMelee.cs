using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyMelee : MovableUnit,ISelect
{
    public void Select()
    {
        throw new System.NotImplementedException();
    }
    public void ChargeToEnemy()
    {
        int multipleNum = 2;
        for (int i = 0; i < 10; i++)
        {
            if (IsEnemyInSearchRange(unitStat.searchRange * multipleNum))
            {
                ChangeState(UnitState.Chase);
                isProvoked = true;
                break;
            }
            multipleNum *= multipleNum;
        }
    }
    public override void Attack()
    {
        if (attackTargetTr != null)
        {
            IHealth Enemy_IHealth = attackTargetTr.GetComponent<IHealth>();
            Enemy_IHealth.TakeDamage(AttackPoint);
        }
    }
}
