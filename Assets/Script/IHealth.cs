using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealth 
{
    int Hp { get; set; }
    int Attack { get; set; }

    void TakeDamage(int damage);


}
