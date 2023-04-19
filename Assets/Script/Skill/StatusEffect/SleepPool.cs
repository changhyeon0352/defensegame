using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepPool : ObjectPooling
{
    private void Awake()
    {
        poolSize = 30;
    }
    protected override void InitializeObj(GameObject obj)
    {
    }
}
