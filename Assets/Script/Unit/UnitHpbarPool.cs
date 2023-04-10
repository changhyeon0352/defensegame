using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHpbarPool : ObjectPooling
{
    private void Awake()
    {
        poolSize = 200;
    }
    protected override void InitializeObj(GameObject obj)
    {
        obj.GetComponent<UnitHpBar>().UpdateHpBar(1, 1);
    }
}
