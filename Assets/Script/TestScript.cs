using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        float a= Mathf.InverseLerp(1, 4, 3);
        Debug.Log(a);
        float b = Mathf.InverseLerp(1, 4, 2);
        Debug.Log(b);
        Debug.Log(Mathf.InverseLerp(2, 4, 2));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
