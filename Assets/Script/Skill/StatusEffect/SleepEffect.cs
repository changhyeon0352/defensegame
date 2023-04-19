using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepEffect : StatusEffect
{
    public SleepEffect(Unit unit, float duration) : base(unit, duration)
    {
        psPool = GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Sleep);
    }
    public override void EndEffect()
    {
        unit.Sleep(false);
        unit.ChangeState(UnitState.Move);
    }
    public override void StartEffect()
    {
        unit.Sleep(true);
    }
    public override void UpdateEffect()
    {
        if(!unit.isSleep)
        {
            isEnd = true;
        }
    }
}
