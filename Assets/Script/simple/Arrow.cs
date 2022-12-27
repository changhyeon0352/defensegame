using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    private float shootingVelocity;
    [SerializeField] private int arrowDamage = 10;
    [SerializeField] private float gravityForce = 10f;
    bool isFlying = true;
    Collider col;
    LayerMask monsterOrGround;
    public float GravityForce { get=>gravityForce;}
    public float ShootVel 
    {
        get=>shootingVelocity; 
        set { shootingVelocity = value; } 
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        monsterOrGround = LayerMask.GetMask("Monster") | LayerMask.GetMask("Ground");
        GetComponent<ConstantForce>().force = new Vector3(0, -(GravityForce - 9.8f), 0);
    }
    void Start()
    {
        //rb.AddForce(transform.forward * shootingVelocity, ForceMode.Impulse);
        rb.velocity = transform.forward * shootingVelocity;

    }

    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
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
        yield return new WaitForSeconds(5f);
        Destroy(this.gameObject);
    }
}
