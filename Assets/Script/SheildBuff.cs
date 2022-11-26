using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheildBuff : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        int a = LayerMask.GetMask("Ally");
        Debug.Log(a);
        if (other.gameObject.layer == LayerMask.GetMask("Ally"))
        {
            AllyUnit ally = other.GetComponent<AllyUnit>();
            ally.armorPlus = 10;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.GetMask("Ally"))
        {
            AllyUnit ally = other.GetComponent<AllyUnit>();
            ally.armorPlus = 0;
        }
    }
}
