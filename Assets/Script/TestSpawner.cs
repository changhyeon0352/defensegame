using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestSpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Instantiate(EnemyPrefab,transform);
        }
    }
}
