using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawaner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public UnitData[] monDatas; 

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Unit monster =Instantiate(EnemyPrefab,transform).GetComponent<Unit>();
            monster.unitData = monDatas[0];
            monster.InitializeUnitStat();
        }
    }
}
