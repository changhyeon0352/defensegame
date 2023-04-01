using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDeploy : MonoBehaviour
{
    DeployModel model;
    private void Awake()
    {
        model = GetComponent<DeployModel>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            model.SelectSpwanUnitData(0);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            model.SelectSpwanUnitData(1);
        }
    }
}
