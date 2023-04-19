using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AllyRange : FixedUnit
{
    BowLoadShot bowLoadShot = null;
    public float shotRange = 10f;
    static float spotRadius = 2.5f;
    public static float SpotRadius { get { return spotRadius; } }
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
        bowLoadShot.SetEnemyMode();
        isShotSpot = false;
        //SetAttackSpeed( unitData.AttackSpeed);
    }
    public void ShotSpotMode(Transform targetTr)
    {
        target = targetTr;
        bowLoadShot.SetSpotMode(targetTr);
        ChangeState(UnitState.Attack);
        isShotSpot = true;
    }
    
    //이 유닛은 따로 구현 불필요
    public override void Attack()
    {
        
    }

    public override void SetTarget(Transform target)
    {
        bowLoadShot.SetTarget(target);
    }

}
