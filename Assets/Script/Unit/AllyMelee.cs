using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyMelee : MovableUnit
{
    public void ChargeToEnemy()
    {
        int multipleNum = 2;
        for (int i = 0; i < 10; i++)
        {
            if (IsEnemyInSearchRange(currentStat.searchRange * multipleNum))
            {
                ChangeState(UnitState.Chase);
                isProvoked = true;
                break;
            }
            multipleNum *= multipleNum;
        }
    }
    public void ReturnToLine()
    {
        ChangeState(UnitState.Move);
        isAttackMove=false;
    }
    public override void Attack()
    {
        if (targetCol.enabled)
        {
            IHealth Enemy_IHealth = targetCol.transform.GetComponent<IHealth>();
            Enemy_IHealth.TakeDamage(AttackPoint);
        }
    }
}
