using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : ObjectPooling
{
    private void Awake()
    {
        poolSize = 100;
    }
    protected override void InitializeObj(GameObject obj)
    {
        Arrow arrow = obj.GetComponent<Arrow>();
        obj.SetActive(true);
        arrow.ResetArrow();
    }
}
