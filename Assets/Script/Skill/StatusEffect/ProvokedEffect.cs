using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProvokedEffect : StatusEffect
{
    private Transform target;

    public ProvokedEffect(Unit unit, float duration, Transform target) : base(unit, duration)
    {
        this.target = target;
        psPool = GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Provoke);
    }
    public override void EndEffect()
    {
        unit.isProvoked = false;
        if (unit.State != UnitState.Dead)
        {
            unit.ChangeState(UnitState.Move);
        }
    }

    public override void StartEffect()
    {
        MovableUnit movableUnit=unit as MovableUnit;
        movableUnit.ChaseTarget(target);
        //이펙트 시작
    }

    public override void UpdateEffect()
    {
        if(target==null|| unit.State == UnitState.Dead)
        {
            isEnd=true;
        }
    }
}
