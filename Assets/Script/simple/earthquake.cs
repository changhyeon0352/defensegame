using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthquake : MonoBehaviour
{
    float sec = 1f;
    float count=1f;
    ParticleSystem ps;
    private void Awake()
    {
        ps=GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        count-=Time.deltaTime;
        if(count<0)
        {
            count = sec;
            ps.Stop();
            ps.Play();
        }
    }


}
