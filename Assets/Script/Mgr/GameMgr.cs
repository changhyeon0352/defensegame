using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    
    public PlayerInput inputActions;

    override protected void Awake()
    {
        base.Awake();
        inputActions =new PlayerInput();
        
    }
    
   
}
