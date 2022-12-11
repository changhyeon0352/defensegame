using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragonBreath : MonoBehaviour
{
    int damage;
    private void Awake()
    {
        //damage = GameMgr.Instance.skillMgr.UsingSKill.data.damage;
        damage = 10;
    }
    private void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Monster"))
        {
            other.GetComponent<Unit>().TakeDamage(damage);
        }
    }
}
