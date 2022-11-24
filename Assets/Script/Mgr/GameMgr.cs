using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    public SettingMgr settingMgr;
    public CommandMgr commandMgr;
    public PlayerInput inputActions;
    public CameraMove cameraMove;
    private Phase phase;
    public Action<Phase> actionChangePhase;
    public Phase Phase { get => phase; }
    private void ChangePhase(Phase _phase)
    {
        switch (phase)
        {
            case Phase.town:
                break;
            case Phase.selectHero:
                break;
            case Phase.setting:
                settingMgr.enabled = false;
                cameraMove.enabled = false;
                break;
            case Phase.defense:
                commandMgr.enabled = false;
                cameraMove.enabled = false;
                break;
        }
        switch (_phase)
        {
            case Phase.town:
                break;
            case Phase.selectHero:
                break;
            case Phase.setting:
                cameraMove.enabled = true;
                settingMgr.enabled = true;
                GameMgr.Instance.settingMgr.SpawnHeros();
                break;
            case Phase.defense:
                cameraMove.enabled = true;
                commandMgr.enabled = true;
                inputActions.Command.Enable();
                break;
        }

        phase = _phase;
    }

    override protected void Awake()
    {
        base.Awake();
        inputActions =new PlayerInput();
    }
    private void Start()
    {
        actionChangePhase += ChangePhase;
    }
    public void ChangePhaseAction(int iPhase)
    {
        actionChangePhase.Invoke((Phase)iPhase);
    }
    
   
}
