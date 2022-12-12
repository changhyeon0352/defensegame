using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Unit
{

    
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
