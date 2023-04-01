using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvokedEffect : StatusEffect
{
    private Transform target;

    public ProvokedEffect(Unit unit, float duration, Transform target) : base(unit, duration)
    {
        this.target = target;
    }
    public override void EndEffect()
    {
        if (unit.State != UnitState.Dead)
        {
            unit.ChangeState(UnitState.Move);
        }
    }

    public override void StartEffect()
    {
        //unit.ChaseTargetTr = target;
        unit.ChangeState(UnitState.Chase);
    }

    public override void UpdateEffect()
    {
        //if (!unit.IsProvoked || unit.ChaseTargetTr == null)
        //{
        //    // 도발 상태가 해제됐을 때, 상태이상 효과를 종료합니다.
        //    unit.EndStatusEffect();
        //}
    }
}
