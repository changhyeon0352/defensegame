using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MovableUnit
{
    public override void Attack()
    {
        if (targetCol != null&&targetCol.enabled)
        {
            IHealth Enemy_IHealth = targetCol.GetComponent<IHealth>();
            Enemy_IHealth.TakeDamage(AttackPoint);
        }
    }

    override protected void Awake()
    {
        base.Awake();
        goalTr = FindObjectOfType<Goal>().transform;
        
       
    }
    protected void Start()
    {
        ChangeState(UnitState.Move);
        
    }
    protected override IEnumerator DieFallCor()
    {
        if(state == UnitState.Dead)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.drag = 20;
            yield return new WaitForSeconds(1);
            FindObjectOfType<EnemySpawaner>().ReturnObjectToPool(this.gameObject);
        }
    }
    public void ReSetUnitAfterDie()
    {
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.drag = 0;
        col.enabled = true;
        col.isTrigger = false;
        navMesh.enabled = true;
        InitUnitHpbar();
        ChangeState(UnitState.Move);
    }
}
