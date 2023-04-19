using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawaner : MonoBehaviour
{
    [SerializeField]
    float coolTime;
    [SerializeField]
    ObjectPooling objectPooling;
    
    float lastTime = 0;
    private void Update()
    {
        if(Time.time-lastTime>coolTime)
        {
            lastTime = Time.time;
            GameObject obj = objectPooling.GetObjectFromPool();
            obj.transform.position= transform.position;
            obj.SetActive(true);
        }
    }
}
