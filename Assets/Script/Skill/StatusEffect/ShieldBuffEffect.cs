using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBuffEffect : StatusEffect
{
    int shiledPoint;
    public ShieldBuffEffect(Unit unit, float duration,int shiledPoint) : base(unit, duration)
    {
        this.shiledPoint = shiledPoint;
        psPool = GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Shield);
    }
    public override void EndEffect()
    {
        unit.Armor=unit.PermanentStat.armor;
    }

    public override void StartEffect()
    {
        unit.Armor= unit.Armor+ shiledPoint;

    }

    public override void UpdateEffect()
    {
        if (unit.State==UnitState.Dead)
        {
            isEnd = true;
        }
    }
}
