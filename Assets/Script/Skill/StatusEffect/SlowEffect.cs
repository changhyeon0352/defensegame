using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEffect : StatusEffect
{
    public SlowEffect(Unit unit, float duration) : base(unit, duration)
    {

    }
    public override void EndEffect()
    {
        unit.MultiplyMoveSpeed(1);
    }

    public override void StartEffect()
    {
        unit.MultiplyMoveSpeed(0.25f);
    }

    public override void UpdateEffect()
    {
        if (unit.State==UnitState.Dead)
        {
            isEnd = true;
        }
    }
}
