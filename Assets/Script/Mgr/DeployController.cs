using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class DeployController : Controller
{
    DeployModel model;
    DeployView view;
    public Action<int,int> OnSpawnPointChanged;
    override protected void Awake()
    {
        base.Awake();
        model = GetComponent<DeployModel>();
        view = GetComponent<DeployView>();
        OnSpawnPointChanged += view.SetPoint;
    }
    private void OnEnable()
    {
        inputActions.Deploy.Enable();
        inputActions.Deploy.NewUnitGroup.performed += OnStartSetting;
        inputActions.Deploy.scrollUpDown.performed += OnAddorRemoveUnitColumn;
        inputActions.Deploy.RotateUnitGroup.performed += OnRotateUnitGroup;
        inputActions.Deploy.RotateUnitGroup.canceled += OnRotateCancel;
        inputActions.Deploy.Click.performed += OnCompleteSetting;
        inputActions.Deploy.SwitchRow.performed += OnChangeRow;
        inputActions.Deploy.ReSetting.performed += OnResetting;
        inputActions.Deploy.Cancel.performed += OnCancel;
        inputActions.Deploy.Click.Disable();
        inputActions.Deploy.scrollUpDown.Disable();
        inputActions.Deploy.Click.Disable();
        inputActions.Deploy.scrollUpDown.Disable();
        inputActions.Deploy.Cancel.Disable();
    }
    private void OnStartSetting(InputAction.CallbackContext obj)
    {
        if (model.UnitGroup == null && model.SpawnUnitData != null)
        {
            model.StartSetting();
            view.ShaderChange(UnitShader.transparentShader, model.UnitGroup.UnitsTr.GetComponentsInChildren<SkinnedMeshRenderer>());
            inputActions.Deploy.Click.Enable();
            inputActions.Command.Select.Disable();
            inputActions.Deploy.scrollUpDown.Enable();
            inputActions.Camera.CameraZoom.Disable();
            inputActions.Deploy.SwitchRow.Enable();
            inputActions.Game.GameQuit.Disable();
            inputActions.Deploy.Cancel.Enable();
        }
    }
    private void OnRotateCancel(InputAction.CallbackContext obj)
    {
        model.SetDir(0);
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        model.RemoveEveryUnit();
        OnCompleteSetting(obj);
    }
    private void OnResetting(InputAction.CallbackContext _)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
        {
            model.Ressetting(hit);
            view.ShaderChange(UnitShader.transparentShader, model.UnitGroup.UnitsTr.GetComponentsInChildren<SkinnedMeshRenderer>());
            inputActions.Deploy.Click.Enable();
            inputActions.Deploy.ReSetting.Disable();
            inputActions.Command.Select.Disable();
            inputActions.Deploy.scrollUpDown.Enable();
            inputActions.Camera.CameraZoom.Disable();
            inputActions.Deploy.SwitchRow.Enable();
            inputActions.Game.GameQuit.Disable();
            inputActions.Deploy.Cancel.Enable();
        }
    }

    

    private void OnChangeRow(InputAction.CallbackContext obj)
    {
        Keyboard kboard = Keyboard.current;
        if (kboard.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl k in kboard.allKeys)
            {
                int row = (int)k.keyCode - 40; //1번키의 keycode=41
                if (k.wasPressedThisFrame)
                {
                    if (model.UnitGroup != null)
                    {
                        model.ChangeRow(row); 
                    }
                    break;
                }
            }
        }
    }

    public void OnCompleteSetting(InputAction.CallbackContext obj)
    {
        view.ShaderChange(UnitShader.normalShader, model.UnitGroup.UnitsTr.GetComponentsInChildren<SkinnedMeshRenderer>());
        model.CompleteUnitSetting(Camera.main.ScreenPointToRay(Input.mousePosition));
        model.ChangeRow(1);
        inputActions.Deploy.Click.Disable();
        inputActions.Deploy.scrollUpDown.Disable();
        inputActions.Camera.CameraZoom.Enable();
        inputActions.Deploy.ReSetting.Enable();
        inputActions.Command.Select.Enable();
        inputActions.Deploy.SwitchRow.Disable();
        inputActions.Deploy.Cancel.Disable();
        inputActions.Game.GameQuit.Enable();
    }

    private void OnRotateUnitGroup(InputAction.CallbackContext obj)
    {
        model.SetDir(obj.ReadValue<float>());
    }

    private void OnAddorRemoveUnitColumn(InputAction.CallbackContext obj)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            float scroll = obj.ReadValue<float>();
            if (scroll < 0)
            {
                model.RemoveLastColumn();
            }
            else
            {
                model.AddUnitColumn();
                view.ShaderChange(UnitShader.transparentShader, model.UnitGroup.UnitsTr.GetComponentsInChildren<SkinnedMeshRenderer>());
            }
        }
    }

    
    public void ClickUnitButton(int n)
    {
        model.SelectSpawnData(n);
        view.SelectSpawnUnitUI(model.SpawnUnitData.Cost,n);
    }
}
