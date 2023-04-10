using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawaner : ObjectPooling
{
    [SerializeField]
    UnitData unitData;
    float coolTime = 1f;
    float lastTime = 0;
    
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
    private void Update()
    {
        if(Time.time-lastTime>coolTime)
        {
            lastTime = Time.time;
            GetObjectFromPool();
        }
    }
}
