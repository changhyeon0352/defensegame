using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IHealth
{
    int hpMax=100;
    int hp;
    private void Awake()
    {
        hp = hpMax;
    }
    public int Hp { get => hp;
        set
        {
            hp = value;
            //UIMgr.Instance.hpbar.ChangeHPbarGate((float)hp / (float)hpMax);
            if (hp < 0) 
            { 
                Destroy(this.gameObject); 
            } 
        } 
    }
    int attack = 0;
    public int AttackPoint { get => attack; set { } }
    int armor=0;
    public int Armor { get =>armor; set { }  }

    public void TakeDamage(int damage)
    {
        Hp-=damage;
        Debug.Log(hp);
    }
}
