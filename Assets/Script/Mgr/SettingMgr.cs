using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


    

public class SettingMgr : MonoBehaviour
{
    public GameObject[] unitPrefabs;
    private GameObject spawnUnitPrefab;
    public GameObject unitGroupPrefab;
    PlayerInput inputActions;

    List<GameObject> unitSetList;
    private float   unitOffset  = 1.5f;
    public  float   scrollSpeed = 1f;
    private int     num_row;
    float dir = 0;
    private Transform unitGroupTr;


    private void Awake()
    {
        inputActions= GameMgr.Instance.inputActions;

        unitSetList = new List<GameObject>();
    }
    private void OnEnable()
    {
        inputActions.Setting.Enable();
        inputActions.Setting.NewUnitGroup.performed     += OnStartSetting;
        inputActions.Setting.scrollUpDown.performed     += OnscrollUpDown;
        inputActions.Setting.RotateUnitGroup.performed  += OnRotateUnitGroup;
        inputActions.Setting.RotateUnitGroup.canceled   += (_) => dir = 0;
        inputActions.Setting.Click.performed            += OnCompleteSetting;
        inputActions.Setting.SwitchRow.performed        += OnSwitchRow;
        inputActions.Setting.ReSetting.performed        += OnResetting;
    }

   

    private void OnDisable()
    {
        inputActions.Setting.ReSetting.performed        -= OnResetting;
        inputActions.Setting.Click.performed            -= OnCompleteSetting;
        inputActions.Setting.RotateUnitGroup.canceled   -= (_) => dir = 0;
        inputActions.Setting.scrollUpDown.performed     -= OnscrollUpDown;
        inputActions.Setting.RotateUnitGroup.performed  -= OnRotateUnitGroup;
        inputActions.Setting.NewUnitGroup.performed     -= OnStartSetting;
        inputActions.Setting.Disable();
    }

    private void Start()
    {
        inputActions.Setting.Click.Disable();
        inputActions.Setting.scrollUpDown.Disable();
    }

    private void Update()
    {
        //세팅중
        if (unitSetList.Count>0)
        {
            //유닛 회전
            unitGroupTr.rotation *= Quaternion.Euler(0f, dir * 360f * Time.deltaTime, 0f);
            //유닛 정렬해서 위치시키기
            SortingUnit();

            
        }
        //else // 유닛 세팅 고치기
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
                
        //    }
        //}
    }
    //inputActions 연결함수=======================================================================
    private void OnSwitchRow(InputAction.CallbackContext obj)
    {
        Keyboard kboard = Keyboard.current;
        if (kboard.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl k in kboard.allKeys)
            {
                if (k.wasPressedThisFrame)
                {
                    ChangeGroupRow((int)k.keyCode - 40);
                    break;
                }

            }
        }
    }
    private void OnCompleteSetting(InputAction.CallbackContext obj)
    {
        CompleteUnitSetting();
        inputActions.Setting.Click.Disable();
        inputActions.Setting.scrollUpDown.Disable();
        inputActions.Setting.ReSetting.Enable();
        inputActions.Command.Click.Enable();
    }
    private void OnRotateUnitGroup(InputAction.CallbackContext obj)
    {
        dir = obj.ReadValue<float>();
    }
    private void OnStartSetting(InputAction.CallbackContext _)
    {
        
        if (unitSetList.Count == 0&&spawnUnitPrefab!=null)
        {
            GameObject obj = Instantiate(unitGroupPrefab, Vector3.zero, Quaternion.identity);
            unitGroupTr = obj.transform;
            unitGroupTr.parent = transform;
            num_row = 1;
            AddUnitRow();
            inputActions.Setting.scrollUpDown.Enable();
            inputActions.Setting.Click.Enable();
            inputActions.Command.Click.Disable();
        }
        
    }
    private void OnscrollUpDown(InputAction.CallbackContext obj)
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            float scroll = obj.ReadValue<float>();
            if (scroll < 0)
            {
                RemoveLastRow();
            }
            else
            {
                AddUnitRow();
            }
        }
    }
    private void OnResetting(InputAction.CallbackContext _)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
        {
            unitGroupTr = hit.transform.parent;
            //unitspot제거
            GameObject objDestroy = unitGroupTr.GetChild(unitGroupTr.childCount - 1).gameObject;
            objDestroy.transform.parent = null;
            Destroy(objDestroy);

            unitGroupTr.parent = transform;
            for (int i = 0; i < unitGroupTr.childCount; i++)
            {
                unitSetList.Add(unitGroupTr.GetChild(i).gameObject);
            }
            if (unitGroupTr != null && unitGroupTr.childCount == unitSetList.Count)
            {
                ShaderChange(UnitShader.transparentShader);
            }
            inputActions.Setting.Click.Enable();
            inputActions.Setting.ReSetting.Disable();
            inputActions.Command.Click.Disable();
        }
        
    }
    public void SelectSpawnUnitType(int i)
    {
        spawnUnitPrefab = unitPrefabs[i];
    }
    // 로컬 함수====================================================
    

    
    private void SortingUnit()
    {
        SortingUnit(Mouse.current.position.ReadValue());
    }
    private void SortingUnit(Vector3 mousePosition)
    {
        //유닛그룹 정렬시키기
        //짝수면 n/2-0.5  홀수면 (n-1)/2 
        if(unitSetList.Count > 0)
        {
            float centerDiff = 0;
            if ((unitSetList.Count / num_row) % 2 == 0)
            {
                centerDiff = (unitSetList.Count / num_row) / 2 - 0.5f;
            }
            else
            {
                centerDiff = ((unitSetList.Count / num_row) - 1) / 2;
            }
            for (int i = 0; i < unitSetList.Count / num_row; i++)
            {
                for (int j = 0; j < num_row; j++)
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                    ray.origin += unitOffset * ((i - centerDiff) * unitGroupTr.right - unitGroupTr.forward * j);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                    {
                        unitSetList[i * num_row + j].transform.position = hit.point;
                    }
                }
            }
        }
        
    }

    private void CompleteUnitSetting()
    {
        GameObject obj = new GameObject();
        obj.name = "unitSpots";

        for (int i = 0; i < unitGroupTr.childCount; i++)
        {
            GameObject spot = new GameObject();
            spot.name = $"spot{i}";
            spot.transform.position = unitGroupTr.GetChild(i).position;
            spot.transform.rotation = unitGroupTr.rotation;
            spot.transform.parent = obj.transform;
            Unit unit = unitGroupTr.GetChild(i).GetComponent<Unit>();
            unit.goalTr = spot.transform;
            unit.ChangeState(UnitState.Move);
        }
        obj.transform.parent = unitGroupTr;

        ShaderChange(UnitShader.normalShader);

        unitGroupTr.parent = transform.parent;//원래 이거자식이 unitGroupTr인데 형제로 격상
        unitSetList.Clear();
        UnitGroup unitgroup=unitGroupTr.GetComponent<UnitGroup>();
        unitgroup.InitializeUnitList();
        unitGroupTr = null;

        
    }
    
    

    void ChangeGroupRow(int iRow)
    {
        if(unitSetList.Count>0)
        {
            num_row = iRow;
            int a = unitSetList.Count % iRow;
            for(int i=0; i<a; i++)
            {
                RemoveLastToList();
            }
        }
    }
    void UnitListClear()
    {
        for(int i=0;i<unitSetList.Count;i++)
        {
            RemoveLastToList();
        }
    }
    void AddUnitRow()
    {
        for(int i=0;i<num_row;i++)
        {
            GameObject obj = Instantiate(spawnUnitPrefab, unitGroupTr);
            ShaderChange(UnitShader.transparentShader);
            unitSetList.Add(obj);
        }
    }
    void RemoveLastRow()
    { 
        if (unitSetList.Count > num_row)
        { 
            for(int i=0; i<num_row;i++)
            {
                RemoveLastToList();
            }
        }
    }
    void RemoveLastToList()
    {
        if(unitSetList.Count>1)
        {
            GameObject obj = unitSetList[unitSetList.Count - 1];
            unitSetList.Remove(obj);
            Destroy(obj);
        }
    }
    void ShaderChange(UnitShader _type)
    {
        SkinnedMeshRenderer[] skinRen = unitGroupTr.GetComponentsInChildren<SkinnedMeshRenderer>();


        for (int i = 0; i < skinRen.Length; i++)
        {
            skinRen[i].material.SetInt("_IsSpawning", (int)_type);
            if(_type==UnitShader.transparentShader)
            {
                skinRen[i].material.SetFloat("_Alpha",0.2f);
            }
            else
            {
                skinRen[i].material.SetFloat("_Alpha", 1f);

            }
        }
    }

}
