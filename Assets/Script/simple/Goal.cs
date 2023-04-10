using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if(unit!=null)
        {
            if(other.CompareTag("Monster"))
            {
                unit.TakeDamage(10000);
                DataMgr.Instance.MonsterEnterTown(unit);
            }
        }
    }
}
