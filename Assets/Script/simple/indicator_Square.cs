using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class indicator_Square : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale=new Vector3(transform.localScale.x, 1, 1);
    }

}
