using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour, IHealth
{
    int hp = 100;
    public int Hp { get => hp;set { hp = value; if (hp < 0) { Destroy(this.gameObject); } } }
    int attack = 0;
    public int Attack { get => attack; set { } }
    int armor;
    public int Armor { get =>armor; set { }  }

    public void TakeDamage(int damage)
    {
        Hp-=damage;
        Debug.Log(hp);
    }
}
