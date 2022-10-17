using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyUnit : Unit
{
    //ㄴㅁㅇ
    protected override void Update()
    {
        if(Input.GetKey(KeyCode.X))
        {
            ChargeToEnemy();
        }
        base.Update();
        
    }
    void ChargeToEnemy()
    {
        int multipleNum = 2;
        for(int i=0; i<10; i++)
        {
            if (SearchAndChase(attackRange*multipleNum))
            {
                break;
            }
            multipleNum *= multipleNum;

        }
    }
}
