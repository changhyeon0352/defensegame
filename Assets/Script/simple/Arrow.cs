using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private int arrowDamage = 10;
    [SerializeField] private float gravityForce = 10f;
    bool isFlying = true;
    Collider col;
    LayerMask monsterOrGround;
    public float GravityForce { get=>gravityForce;}    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        monsterOrGround = LayerMask.GetMask("Monster") | LayerMask.GetMask("Ground");
        GetComponent<ConstantForce>().force = new Vector3(0, -(GravityForce - 9.8f), 0);
    }
    public void SetSpeed(float speed)
    {
        rb.velocity = transform.forward * speed;
    }
    public void ResetArrow()
    {
        isFlying = true;
        rb.isKinematic =false;
        rb.useGravity = true;
        col.enabled = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==6|| other.gameObject.layer == 7)
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit != null)
            {
                unit.TakeDamage(arrowDamage);
            }


            transform.parent = other.transform.Find("Root") ?? other.transform.Find("Bip001") ?? other.transform;
            isFlying = false;
            rb.isKinematic = true;
            rb.useGravity = false;
            col.enabled = false;
            StartCoroutine(DestroyTimer());
        }
    }
    private void Update()
    {
        if(isFlying)
        {
            transform.LookAt(transform.position + rb.velocity);
            
        }
           // 
    }
    public void MakeNoise(Vector3 noiseVector)
    {
        
        rb.AddForce(noiseVector, ForceMode.Impulse);
    }
    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(3f);
        GameMgr.Instance.ObjectPools.GetObjectPool(ObjectPoolType.Arrow).ReturnObjectToPool(this.gameObject);
    }
}
