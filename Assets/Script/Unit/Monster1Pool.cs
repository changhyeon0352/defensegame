using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster1Pool : ObjectPooling
{
    [SerializeField]
    UnitData unitData;
    private void Awake()
    {
        prefab = unitData.unitPrefab;
        poolSize = 30;
    }
    protected override void InitializeObj(GameObject obj)
    {
        Monster unit = obj.GetComponent<Monster>();
        unit.ReSetUnitAfterDie();
        unit.SetUnitData(unitData);
        unit.InitializeUnitStat();
    }
}
