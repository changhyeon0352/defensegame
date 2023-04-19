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
        yield return new WaitForSeconds(1);
        Arrow[] arrows = GetComponentsInChildren<Arrow>();
        for(int i = 0; i < arrows.Length; i++)
        {
            GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Arrow).ReturnObjectToPool(arrows[i].gameObject);
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.drag = 20;
        yield return new WaitForSeconds(1);
        ObjectPooling objPool;
        if (UnitData.Type==UnitType.mon2)
        {
            objPool=GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.mon2);
        }
        else
        {
            objPool = GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.mon1);
        }
        
        objPool.ReturnObjectToPool(this.gameObject);
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
