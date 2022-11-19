using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    
    public PlayerInput inputActions;
    private Phase phase;
    public Phase Phase { get => phase; }
    public void ChangePhase(Phase _phase)
    {
        phase = _phase;
        switch (_phase)
        {
            case Phase.town:
                break;
            case Phase.selectHero:
                break;
            case Phase.setting:
                inputActions.Setting.Enable();
                SettingMgr.Instance.SpawnHeros();
                break;
            case Phase.defense:
                inputActions.Setting.Disable();
                inputActions.Command.Enable();
                break;
        }

    }

    override protected void Awake()
    {
        base.Awake();
        inputActions =new PlayerInput();
        
    }
    
   
}
