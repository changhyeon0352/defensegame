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
            int a=Random.Range(0, 10);
            if(monDatas.Length>1&&a<2)
                SpawnMonster(1);
            else
                SpawnMonster(0);

            timeCount =coolTime;
            
        }
    }

    private void SpawnMonster(int index)
    {
        Unit monster = Instantiate(monDatas[index].unitPrefab, transform).GetComponent<Unit>();
        monster.SetUnitData(monDatas[index]);
        monster.InitializeUnitStat();
    }
}
