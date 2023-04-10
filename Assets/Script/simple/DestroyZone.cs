using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            FindObjectOfType<EnemySpawaner>().ReturnObjectToPool(other.gameObject);
        }
        else
            Destroy(other.gameObject);
    }
}
