using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    private static GameMgr instance = null;
    public SettingMgr   settingMgr;
    public CommandMgr   CommandMgr;
    public UIMgr        uiMgr;
    public PlayerInput inputActions;
    
    private void Awake()
    {
        inputActions=new PlayerInput();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
    public static GameMgr Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
    }
    //게임시작, 일시정지, 끝, 재시작 등등// 세팅중, 액션맵 키고 끄기
    
    private void OnEnable()
    {
        
    }

   
}
