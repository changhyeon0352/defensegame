using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MovableUnit
{
    public override void Attack()
    {
        if (attackTargetTr != null)
        {
            IHealth Enemy_IHealth = attackTargetTr.GetComponent<IHealth>();
            Enemy_IHealth.TakeDamage(AttackPoint);
        }
    }

    override protected void Awake()
    {
        base.Awake();
        goalTr = FindObjectOfType<Goal>().transform;
        UIMgr.Instance.hpbar.AddHpBar(this);
       
    }
     protected void Start()
    {
        ChangeState(UnitState.Move);
    }
    
}
