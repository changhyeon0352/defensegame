using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnergyBolt : MonoBehaviour
{
    public GameObject explosionPrefab;
    private Transform target;
    [SerializeField] float speed=10f;
    private int damage;

    private void Update()
    {
        if (target == null)
            Destroy(this.gameObject);
        transform.position += (target.position - transform.position).normalized * speed * Time.deltaTime;
        //transform.LookAt(target);
        //transform.Translate(transform.forward * speed * Time.deltaTime);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform==target)
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
        this.target = target;
        this.damage = damage;
    }
}
