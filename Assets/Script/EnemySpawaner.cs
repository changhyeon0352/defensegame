using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawaner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public UnitData[] monDatas;
    float timeCount=1f;
    float coolTime = 1f;

    // Update is called once per frame
    void Update()
    {
        if(GameMgr.Instance.Phase==Phase.defense)
            timeCount-=Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.C))
        {
            SpawnMonster(0);
        }
        if(timeCount<0)
        {
            timeCount=coolTime;
            SpawnMonster(0);
        }
    }

    private void SpawnMonster(int index)
    {
        Unit monster = Instantiate(EnemyPrefab, transform).GetComponent<Unit>();
        monster.unitData = monDatas[index];
        monster.InitializeUnitStat();
    }
}
