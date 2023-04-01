using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class GameMgr : Singleton<GameMgr>
{
    public DeployModel Deployer;
    public DefenseCommand defenseMgr;
    public SkillController skillController;
    public PlayerInput inputActions;
    public CameraMove cameraMove;
    public GameObject cameraMain;
    public GameObject townCamera;
    float defenseTime = 240;
    float timeElapsed = 0;
    private Phase phase;
    public Action<Phase> actionChangePhase;
    public Phase Phase { get => phase; }
    private void ChangePhase(Phase _phase)
    {
        switch (phase)
        {
            case Phase.town:
                townCamera.SetActive(false);
                cameraMain.SetActive(true);
                break;
            case Phase.selectHero:
                break;
            case Phase.setting:
                Deployer.enabled = false;
                cameraMove.enabled = false;
                break;
            case Phase.defense:
                cameraMove.enabled = false;
                defenseMgr.enabled = false;
                skillController.enabled = false;
                inputActions.Command.Enable();
                break;
            case Phase.result:
                DataMgr.Instance.GenerateNewGuildList();
                break;
                
        }
        switch (_phase)
        {
            case Phase.town:
                townCamera.SetActive(true);
                cameraMain.SetActive(false);
                break;
            case Phase.selectHero:
                break;
            case Phase.setting:
                cameraMove.enabled = true;
                Deployer.enabled = true;
                Deployer.SpawnHeros();
                Deployer.InitSpawnPoint();
                break;
            case Phase.defense:
                cameraMove.enabled = true;
                defenseMgr.enabled = true;
                skillController.enabled = true;
                inputActions.Command.Enable();
                break;
            case Phase.result:
                break;
        }

        phase = _phase;
    }

    override protected void Awake()
    {
        base.Awake();
        inputActions =new PlayerInput();
        inputActions.Game.Enable();
        inputActions.Game.GameQuit.performed += OnEscape;
    }

    private void OnEscape(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(skillController==null||!skillController.IsUsingSkill)
        {
            UIMgr.Instance.ToggleGameQuitWindow();
        }
    }
    public void GameQuit()
    {
        Application.Quit();
    }
    private void Start()
    {
        actionChangePhase += ChangePhase;
        ChangePhaseAction(0);

    }
    private void Update()
    {
        if (Phase == Phase.defense)
        {
            timeElapsed += Time.deltaTime;
            UIMgr.Instance.ShowTimer((defenseTime - timeElapsed) / defenseTime);
            if (timeElapsed > defenseTime)
            {
                ChangePhaseAction(4);
            }
        }
    }
    public void ChangePhaseAction(int iPhase)
    {
        actionChangePhase.Invoke((Phase)iPhase);
    }
    
   
}
