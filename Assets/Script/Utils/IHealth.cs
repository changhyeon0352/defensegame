using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
    int Hp {  get; }
    int AttackPoint { get; }
    int Armor { get; }

    void TakeDamage(int damage);
}
