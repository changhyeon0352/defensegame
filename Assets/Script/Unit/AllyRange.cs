using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AllyRange : FixedUnit,ISelect
{
    BowLoadShot bowLoadShot = null;
    public float shotRange = 10f;
    protected override void Awake()
    {
        base.Awake();
        bowLoadShot = GetComponent<BowLoadShot>();
    }



    public void CheckTargetAlive()
    {
        if(target==null)
            return;
        Collider col = target.GetComponent<Collider>();
        if ((col == null || !col.enabled) && !isShotSpot)
        {
            ChangeState(UnitState.Idle);
        }
    }
    public void ShotEnemyMode()
    {
        ChangeState(UnitState.Idle);
        bowLoadShot.ShotAngle = 5;
        isShotSpot = false;
        SetAttackSpeed( unitData.AttackSpeed);
    }
    public void ShotSpotMode(Transform targetTr)
    {
        bowLoadShot.ShotAngle = 40;
        bowLoadShot.SetBowTarget(targetTr);
        ChangeState(UnitState.Attack);
        isShotSpot = true;
        SetAttackSpeed(unitData.AttackSpeed*1.3f);
    }

    public void Select()
    {
    }

    public override void Attack()
    {
        
    }

    public override void SetTarget(Transform target)
    {
        bowLoadShot.SetBowTarget(target);
    }
}
