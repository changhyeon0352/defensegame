using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnergyBolt : MonoBehaviour
{
    public GameObject explosionPrefab;
    private Collider targetCol;
    [SerializeField] float speed=10f;
    private int damage;

    private void Update()
    {
        if (targetCol == null|| !targetCol.enabled)
            Destroy(this.gameObject);
        else
        {
            transform.LookAt(targetCol.transform);
            transform.position += (targetCol.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform== targetCol.transform)
        {
            IHealth targetHP = other.gameObject.GetComponent<IHealth>();
            if (targetHP != null)
            {
                targetHP.TakeDamage(damage);
                Instantiate(explosionPrefab,transform.position,Quaternion.identity);
                Destroy(this.gameObject);
            }
        }
    }
    public void SetTargetAndDamage(Transform target,int damage)
    {
        this.targetCol = target.GetComponent<Collider>();
        this.damage = damage;
    }
}
