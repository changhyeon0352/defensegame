using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SettingMgr : MonoBehaviour
{
    public GameObject[] heroPrefabs;
    //public GameObject[] unitPrefabs;
    private GameObject spawnHeroPrefab;
    public GameObject unitGroupPrefab;
    PlayerInput inputActions;
    List<GameObject> unitSetList;
    private float   unitOffset  = 1.5f;
    public  float   scrollSpeed = 1f;
    private int     num_row;
    float dir = 0;
    private Transform unitGroupTr;
    private UnitGroup unitGroup;
    public Transform heroSpawnSpots;
    private int unitSpawnPoint = 4000;
    private int nowUnitSpawnPoint;
    public UnitData[] unitDatas;
    private UnitData spawnUnitData;

    public int NowUnitSpawnPoint { get { return nowUnitSpawnPoint; } 
        set { nowUnitSpawnPoint = value; UIMgr.Instance.UpdateSpawnPoint(nowUnitSpawnPoint, unitSpawnPoint); } }
    public float UnitOffset { get => unitOffset; }

    private void Awake()
    {
        inputActions = GameMgr.Instance.inputActions;
        unitSetList = new();
        
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
        NowUnitSpawnPoint = unitSpawnPoint;

    }
    private void OnDisable()
    {
        inputActions.Setting.Disable();
    }

    private void Start()
    {
        inputActions.Setting.Click.Disable();
        inputActions.Setting.scrollUpDown.Disable();
    }

    private void Update()
    {
        //?????????
        if (unitSetList.Count>0)
        {
            //?????? ??????
            unitGroupTr.rotation *= Quaternion.Euler(0f, dir * 360f * Time.deltaTime, 0f);
            //?????? ???????????? ???????????????
            SortingUnit();
        }
    }
    //inputActions ????????????=======================================================================
    private void OnSwitchRow(InputAction.CallbackContext _)
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
    private void OnCompleteSetting(InputAction.CallbackContext _)
    {
        CompleteUnitSetting(Camera.main.ScreenPointToRay(Input.mousePosition));
        inputActions.Setting.Click.Disable();
        inputActions.Setting.scrollUpDown.Disable();
        inputActions.Camera.CameraZoom.Enable();
        inputActions.Setting.ReSetting.Enable();
        inputActions.Command.Select.Enable();
        inputActions.Setting.SwitchRow.Disable();
    }
    private void OnRotateUnitGroup(InputAction.CallbackContext obj)
    {
        dir = obj.ReadValue<float>();
    }
    private void OnStartSetting(InputAction.CallbackContext _)
    {
        if (unitSetList.Count == 0&&spawnUnitData!=null)
        {
            StartSetting();
            inputActions.Setting.Click.Enable();
            inputActions.Command.Select.Disable();
            inputActions.Setting.scrollUpDown.Enable();
            inputActions.Camera.CameraZoom.Disable();
            inputActions.Setting.SwitchRow.Enable();
        }
    }

    private void StartSetting(bool isHero=false)
    {
        GameObject obj = Instantiate(unitGroupPrefab, Vector3.zero, Quaternion.identity);
        unitGroupTr = obj.transform;
        unitGroup = obj.GetComponent<UnitGroup>();
        unitGroupTr.parent = transform;
        num_row = 1;
        if(!isHero)
        {
            AddUnitRow();
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
            //23
            int unitIndex = (int)hit.transform.GetComponent<Unit>().unitData.unitType - 2;
            UIMgr.Instance.SelectedSpawnButton(unitIndex);
            unitGroupTr = hit.transform.parent.parent;
            unitGroup = unitGroupTr.GetComponent<UnitGroup>();
            //unitspot??????
            
            for(int i=0;i<unitGroup.spotsTr.childCount;i++)
            {
                Destroy(unitGroup.spotsTr.GetChild(i).gameObject);
            }
            unitGroupTr.parent = transform;
            for (int i = 0; i < unitGroup.unitsTr.childCount; i++)
            {
                unitSetList.Add(unitGroup.unitsTr.GetChild(i).gameObject);
            }
            if (unitGroupTr != null && unitGroup.unitsTr.childCount == unitSetList.Count)
            {
                ShaderChange(UnitShader.transparentShader);
            }
            inputActions.Setting.Click.Enable();
            inputActions.Setting.ReSetting.Disable();
            inputActions.Command.Select.Disable();
            inputActions.Setting.scrollUpDown.Enable();
            inputActions.Camera.CameraZoom.Disable();
            inputActions.Setting.SwitchRow.Enable();
        }
    }
    public void SelectSpawnUnitType(int i)
    {
        spawnUnitData = unitDatas[i];
    }
    // ?????? ??????====================================================
    
    public void SpawnHeros()
    {
        int i = 0;
        foreach(HeroData data in DataMgr.Instance.FightingHeroDataList)
        {
            
            if(data!=null)
            {
                int classNum = (int)data.heroClass;
                spawnHeroPrefab = heroPrefabs[classNum];
                StartSetting(true);
                GameObject obj = Instantiate(spawnHeroPrefab, unitGroup.unitsTr);
                Hero hero = obj.GetComponent<Hero>();
                if (hero != null)
                {
                    hero.Data = data;
                    hero.unitData = unitDatas[2 + classNum];
                    hero.InitializeUnitStat();
                }
                ShaderChange(UnitShader.transparentShader);
                unitSetList.Add(obj);
                Ray ray = new Ray();
                ray.origin = heroSpawnSpots.GetChild(i).position;
                ray.direction = Vector3.down;
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                {
                    unitSetList[0].transform.position = hit.point;
                    CompleteUnitSetting(ray);
                }
                i++;
            }
        }
    }
    
    private void SortingUnit()
    {
        SortingUnit(Mouse.current.position.ReadValue());
    }//????????????
    private void SortingUnit(Vector3 mousePosition)
    {
        //???????????? ???????????????
        //????????? n/2-0.5  ????????? (n-1)/2 
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
                    ray.origin += UnitOffset * ((i - centerDiff) * unitGroupTr.right +unitGroupTr.forward * ((num_row-1)*0.5f-j));
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                    {
                        unitSetList[i * num_row + j].transform.position = hit.point;
                    }
                }
            }
        }
    }

    private void CompleteUnitSetting(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
        {
            unitGroup.spotsTr.position = hit.point;
        }
        unitGroup.spotsTr.forward = unitGroupTr.forward;
        for (int i = 0; i < unitGroup.unitsTr.childCount; i++)
        {
            unitGroup.rowColumn=new Vector2Int(num_row,(int)(unitGroup.unitsTr.childCount/num_row));
            GameObject spot = new GameObject();
            spot.name = $"spot{i}";
            spot.transform.position = unitGroup.unitsTr.GetChild(i).position;
            spot.transform.rotation = unitGroupTr.rotation;
            spot.transform.parent = unitGroup.spotsTr;
            Unit unit = unitGroup.unitsTr.GetChild(i).GetComponent<Unit>();
            unit.goalTr = spot.transform;
            unit.ChangeState(UnitState.Move);
            unit.InitializeUnitStat();
        }
        ShaderChange(UnitShader.normalShader);
        unitGroupTr.parent = unitGroup.AllyGroups;//?????? ??????????????? unitGroupTr?????? ????????? ??????
        unitSetList.Clear();
        if(spawnUnitData != null)
            unitGroup.unitType = spawnUnitData.unitType;                      //??????????????????
        unitGroup.InitializeUnitList();
        unitGroupTr = null;
        unitGroup = null;
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
    
    void AddUnitRow()
    {
        for(int i=0;i<num_row;i++)
        {
            if(nowUnitSpawnPoint > spawnUnitData.Cost)
            {
                NowUnitSpawnPoint-=spawnUnitData.Cost;
                GameObject obj = Instantiate(spawnUnitData.unitPrefab, unitGroup.unitsTr);
                obj.GetComponent<AllyUnit>().unitData = spawnUnitData;
                ShaderChange(UnitShader.transparentShader);
                unitSetList.Add(obj);
            }
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
            NowUnitSpawnPoint += spawnUnitData.Cost;
            Destroy(obj);
        }
    }
    void ShaderChange(UnitShader _type)
    {
        SkinnedMeshRenderer[] skinRen = unitGroup.unitsTr.GetComponentsInChildren<SkinnedMeshRenderer>();
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
