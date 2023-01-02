/* 아군 병사들을 배치하는 기능을 함*/
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
        inputActions.Setting.Cancel.performed += OnCancel;
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
        //세팅중
        if (unitGroup!=null)
        {
            //유닛 회전
            unitGroupTr.rotation *= Quaternion.Euler(0f, dir * 360f * Time.deltaTime, 0f);
            //유닛 정렬해서 위치시키기
            SortingUnit();
        }
    }
    //inputActions 연결함수=======================================================================
    private void OnCancel(InputAction.CallbackContext _)
    {
        for(int i=0;i<unitGroup.NumUnitList;)
        {
            RemoveLastToList(true);
            Debug.Log(unitGroup.unitsTr.childCount);
        }
        Ray ray=new Ray();
        CompleteUnitSetting(ray);
    }
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
    }
    private void OnRotateUnitGroup(InputAction.CallbackContext obj)
    {
        dir = obj.ReadValue<float>();
    }
    private void OnStartSetting(InputAction.CallbackContext _)
    {
        if (unitGroup==null&&spawnUnitData!=null)
        {
            StartSetting();
            inputActions.Setting.Click.Enable();
            inputActions.Command.Select.Disable();
            inputActions.Setting.scrollUpDown.Enable();
            inputActions.Camera.CameraZoom.Disable();
            inputActions.Setting.SwitchRow.Enable();
        }
    }
    //space바를 누르면 유닛그룹 배치를 시작
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
    //유닛 배치중 스크롤로 유닛그룹에 Add/Remove
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
    //유닛 재배치함수
    private void OnResetting(InputAction.CallbackContext _)
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
        {
            int unitIndex = (int)hit.transform.GetComponent<Unit>().unitData.unitType - 2;
            UIMgr.Instance.SelectedSpawnButton(unitIndex);
            unitGroupTr = hit.transform.parent.parent;
            unitGroup = unitGroupTr.GetComponent<UnitGroup>();
            //unitspot제거
            
            for(int i=0;i<unitGroup.spotsTr.childCount;i++)
            {
                Destroy(unitGroup.spotsTr.GetChild(i).gameObject);
            }
            unitGroupTr.parent = transform;
            for (int i = 0; i < unitGroup.unitsTr.childCount; i++)
            {
                AllyUnit allyunit = unitGroup.unitsTr.GetChild(i).GetComponent<AllyUnit>();
                unitGroup.AddUnitList(allyunit);
            }
            if (unitGroupTr != null && unitGroup.unitsTr.childCount == unitGroup.NumUnitList)
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
    //스폰할 유닛 변경함수
    public int SelectSpawnUnitType(int i)
    {
        spawnUnitData = unitDatas[i];
        return unitDatas[i].Cost;
    }
    // 로컬 함수====================================================
    
    //지정된 위치에 선택한 영웅캐릭터를 배치하는 함수
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
                unitGroup.AddUnitList(obj.GetComponent<AllyUnit>());
                Hero hero = obj.GetComponent<Hero>();
                if (hero != null)
                {
                    hero.Data = data;
                    hero.unitData = unitDatas[2 + classNum];
                    hero.InitializeUnitStat();
                }
                ShaderChange(UnitShader.transparentShader);
                Ray ray = new Ray();
                ray.origin = heroSpawnSpots.GetChild(i).position;
                ray.direction = Vector3.down;
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                {
                    obj.transform.position = hit.point;
                    CompleteUnitSetting(ray);
                }
                i++;
            }
        }
    }
    
    private void SortingUnit()
    {
        SortingUnit(Mouse.current.position.ReadValue());
    }//오버로딩
    //유닛그룹을 행열에 맞게 정렬해서 배치되게 하는 함수
    private void SortingUnit(Vector3 mousePosition)
    {
        //유닛그룹 정렬시키기
        //짝수면 n/2-0.5  홀수면 (n-1)/2 
        if(unitGroup !=null)
        {
            float centerDiff = 0;
            if ((unitGroup.NumUnitList / num_row) % 2 == 0)
            {
                centerDiff = (unitGroup.NumUnitList / num_row) / 2 - 0.5f;
            }
            else
            {
                centerDiff = ((unitGroup.NumUnitList / num_row) - 1) / 2;
            }
            for (int i = 0; i < unitGroup.NumUnitList / num_row; i++)
            {
                for (int j = 0; j < num_row; j++)
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                    ray.origin += UnitOffset * ((i - centerDiff) * unitGroupTr.right +unitGroupTr.forward * ((num_row-1)*0.5f-j));
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                    {
                        unitGroup.UnitList[i * num_row + j].transform.position = hit.point;
                    }
                }
            }
        }
    }
    //유닛그룹 세팅을 마치는 함수 spot정하기/ 인풋관련처리/ 쉐이더 처리함
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
        unitGroupTr.parent = unitGroup.AllyGroups;//유닛그룹이 AllyGroups의 자식으로 계층이동
        if(spawnUnitData != null)
            unitGroup.unitType = spawnUnitData.unitType;                      //유닛타입결정
        unitGroup.InitializeUnitList();
        unitGroupTr = null;
        unitGroup = null;
        inputActions.Setting.Click.Disable();
        inputActions.Setting.scrollUpDown.Disable();
        inputActions.Camera.CameraZoom.Enable();
        inputActions.Setting.ReSetting.Enable();
        inputActions.Command.Select.Enable();
        inputActions.Setting.SwitchRow.Disable();
    }
    //유닛그룹을 몇 열로 할지 변경
    void ChangeGroupRow(int iRow)
    {
        if(unitGroup!=null>0)
        {
            num_row = iRow;
            int remainder = unitGroup.NumUnitList % iRow;
            for(int i=0; i< remainder; i++)
            {
                RemoveLastToList();
            }
        }
    }
    
    //유닛그룹에 유닛 1열을 추가
    void AddUnitRow()
    {
        for(int i=0;i<num_row;i++)
        {
            if(nowUnitSpawnPoint > spawnUnitData.Cost)
            {
                NowUnitSpawnPoint-=spawnUnitData.Cost;
                GameObject obj = Instantiate(spawnUnitData.unitPrefab, unitGroup.unitsTr);
                AllyUnit allyUnit = obj.GetComponent<AllyUnit>();
                allyUnit.unitData = spawnUnitData;
                unitGroup.AddUnitList(allyUnit);
                ShaderChange(UnitShader.transparentShader);
            }
        }
    }
    //배치중인 유닛그룹의 유닛 마지막 열 삭제
    void RemoveLastRow()
    { 
        if (unitGroup.NumUnitList > num_row)
        { 
            for(int i=0; i<num_row;i++)
            {
                RemoveLastToList();
            }
        }
    }
    //배치중인 유닛그룹의 마지막 유닛을 삭제하는 함수
    void RemoveLastToList(bool isCancel=false)
    {
        int limitNum = isCancel ? 0 : 1;
        if(unitGroup.NumUnitList > limitNum)
        {
            GameObject obj = unitGroup.unitsTr.GetChild(unitGroup.NumUnitList - 1).gameObject;
            AllyUnit allyUnit = obj.GetComponent<AllyUnit>();
            unitGroup.RemoveUnitFromList(allyUnit);
            NowUnitSpawnPoint += spawnUnitData.Cost;
            Destroy(obj);
        }
    }
    //배치중인 유닛은 파란색으로 보이게 쉐이더를 조절하는 함수
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
