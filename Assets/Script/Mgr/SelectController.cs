using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SelectController : Controller
{
    LayerMask monsterOrGround;
    SelectModel selectModel;
    SelectView selectView;
    public SelectController(SelectModel model,SelectView view)
    {
        selectModel = model;
        selectView = view;
    }
    override protected void Awake()
    {
        base.Awake();
        monsterOrGround = LayerMask.GetMask("Monster") | LayerMask.GetMask("Ground");
    }
    private void OnEnable()
    {
        inputActions.Command.Select.performed += OnSelect;
        inputActions.Command.ChangeHero.performed += OnChangeHero;
    }

    private void OnChangeHero(InputAction.CallbackContext obj)
    {
        Keyboard kboard = Keyboard.current;
        if (kboard.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl k in kboard.allKeys)
            {
                if (k.wasPressedThisFrame)
                {
                    int num = (int)k.keyCode - 41;
                    if (num < DataMgr.Instance.FightingHeroDataList.Count)
                    {
                        selectModel.SelectHero(num);
                    }
                }
            }
        }
    }

    private void OnSelect(InputAction.CallbackContext obj)
    {
        if (Utils.IsClickOnUI())//UI 클릭 하면 OnSelect를 스킵
            return;
        
    }
}
