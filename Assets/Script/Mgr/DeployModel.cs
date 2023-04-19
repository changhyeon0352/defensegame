using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class DeployModel : MonoBehaviour
{
    public GameObject unitGroupPrefab;
    private int unitSpawnPoint = 4000;
    public int UnitSpawnPoint { 
        get { return unitSpawnPoint; } 
        private set { unitSpawnPoint = value;
            controller.OnSpawnPointChanged?.Invoke(unitSpawnPoint, DataMgr.Instance.GetSpawnPoint());
        }
    }
    private UnitData spawnUnitData;
    private UnitGroup unitGroup;
    private Vector2Int rowColumn = Vector2Int.one;
    private DeployController controller;
    [SerializeField]
    UnitData[] unitDatas;
    float dir;
    private float unitOffset=1.5f;
    public float UnitOffset { get { return unitOffset; } }
    [SerializeField]
    private Transform heroSpawnSpots;

    public UnitGroup UnitGroup { get => unitGroup; }
    public UnitData SpawnUnitData { get => spawnUnitData;}
    private void Awake()
    {
        controller = GetComponent<DeployController>();
    }
    private void Update()
    {
        //세팅중
        if (unitGroup != null)
        {
            //유닛 회전
            unitGroup.transform.rotation *= Quaternion.Euler(0f, dir * 360f * Time.deltaTime, 0f);
            //유닛 정렬해서 위치시키기
            SortingUnit();
        }
    }
    public void InitSpawnPoint()
    {
        UnitSpawnPoint=DataMgr.Instance.GetSpawnPoint();
    }
    public void StartSetting(bool isHero = false)
    {
        GameObject obj = Instantiate(unitGroupPrefab, transform);
        unitGroup = obj.GetComponent<UnitGroup>();
        //unitGroup.transform.SetParent(transform);
        if (!isHero)
        {
            AddUnitColumn();
            rowColumn = Vector2Int.one;
        }
    }
    private void SortingUnit()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        //유닛그룹 정렬시키기
        //짝수면 n/2-0.5  홀수면 (n-1)/2 
        if (UnitGroup != null)
        {
            float centerDiff = 0;
            //if ((UnitGroup.NumUnitList / groupRow) % 2 == 0)
            if ((rowColumn.y) % 2 == 0)
                {
                centerDiff = (rowColumn.y) / 2 - 0.5f;
            }
            else
            {
                centerDiff = (rowColumn.y - 1) / 2;
            }
            for (int i = 0; i < rowColumn.y; i++)
            {
                for (int j = 0; j < rowColumn.x; j++)
                {
                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                    ray.origin += unitOffset * ((i - centerDiff) * unitGroup.transform.right + unitGroup.transform.forward * ((rowColumn.x - 1) * 0.5f - j));
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                    {
                        
                        try
                        {
                            UnitGroup.UnitList[i * rowColumn.x + j].transform.position = hit.point;
                        }
                        catch (ArgumentException ex)
                        {
                            Debug.Log($"{i * rowColumn.x + j} 인덱스 호출");
                            Debug.Log(ex.Message);
                        }
                    }
                }
            }
        }
    }
    
    public void RemoveEveryUnit()
    {
        for (int i = 0; i < UnitGroup.NumUnitList;)
        {
            RemoveLastToList(true);
        }
    }
    public void AddUnitColumn()
    {
        if(unitSpawnPoint >= SpawnUnitData.Cost*rowColumn.x)
        {
            rowColumn.y++;
            for (int i = 0; i < rowColumn.x; i++)
            {
                if (unitSpawnPoint >= SpawnUnitData.Cost)
                {
                    UnitSpawnPoint -= SpawnUnitData.Cost;
                    GameObject obj = Instantiate(SpawnUnitData.unitPrefab, UnitGroup.UnitsTr);
                    Unit unit = obj.GetComponent<Unit>();
                    unit.SetUnitData(SpawnUnitData);
                    UnitGroup.AddUnitList(unit);
                }
            }
        }
        
        
    }
    public void RemoveLastColumn()
    {

        if (UnitGroup.NumUnitList > rowColumn.x)
        {
            rowColumn.y--;
            for (int i = 0; i < rowColumn.x; i++)
            {
                RemoveLastToList();
            }
        }
    }
    public void CompleteUnitSetting(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
        {
            UnitGroup.SpotsTr.position = hit.point;
        }
        UnitGroup.SpotsTr.forward = unitGroup.transform.forward;
        for (int i = 0; i < UnitGroup.NumUnitList; i++)
        {
            UnitGroup.rowColumn = rowColumn;
            GameObject spot = new GameObject();
            spot.name = $"spot{i}";
            spot.transform.position = UnitGroup.UnitList[i].transform.position;
            spot.transform.rotation = unitGroup.transform.rotation;
            spot.transform.SetParent(UnitGroup.SpotsTr);
            Unit unit = UnitGroup.UnitList[i]; 
            unit.SetGoalSpot(spot.transform);
            unit.InitializeUnitStat();
            unit.SetUnitGroup(unitGroup);
        }

        //unitGroup.transform.SetParent(UnitGroup.AllyGroups);//유닛그룹이 AllyGroups의 자식으로 계층이동
        if (SpawnUnitData != null)
            UnitGroup.SetUnitGroupSkill(SpawnUnitData);
        unitGroup = null;
    }
    public void Ressetting(RaycastHit hit)
    {
        int unitIndex = (int)hit.transform.GetComponent<Unit>().UnitData.Type - 1;// none 다음부터임
        SelectSpwanUnitData(unitIndex);
        unitGroup = hit.transform.parent.parent.GetComponent<UnitGroup>();
        rowColumn = UnitGroup.rowColumn;
        unitGroup.ClearSpots();

        unitGroup.transform.SetParent(transform);
    }
    private void RemoveLastToList(bool isCancel = false)
    {
        int limitNum = isCancel ? 0 : 1;
        if (UnitGroup.NumUnitList > limitNum * rowColumn.x)
        {
            Unit unit = unitGroup.UnitList[unitGroup.NumUnitList - 1];
            unitGroup.RemoveUnitFromList(unit);
            UnitSpawnPoint += SpawnUnitData.Cost;
        }
    }
    
    
    
    public void SetDir(float dir)
    {
        this.dir = dir;
    }
    public void ChangeRow(int row)
    {
        rowColumn.x = row;
        if (unitGroup == null)
            return;
        if (unitGroup.NumUnitList < row)
        {
            AddUnitColumn();
        }
        int remainder = unitGroup.NumUnitList % row;
        for (int i = 0; i < remainder; i++)
        {
            RemoveLastToList();
        }
        rowColumn.y = unitGroup.NumUnitList/rowColumn.x;
    }
    public void SelectSpwanUnitData(int num)
    {
        if (num >= unitDatas.Length)
            num = 0;
        spawnUnitData = unitDatas[num];
    }
    public void SpawnHeros()
    {
        int i = 0;
        foreach (HeroData heroData in DataMgr.Instance.FightingHeroDataList)
        {

            if (heroData != null)
            {
                UnitData unitData = unitDatas[(int)heroData.heroClass+2];
                StartSetting(true);
                GameObject obj = Instantiate(unitData.unitPrefab, heroSpawnSpots.GetChild(i).position, Quaternion.identity, unitGroup.UnitsTr);
                unitGroup.AddUnitList(obj.GetComponent<Unit>());
                HeroUnit hero = obj.GetComponent<HeroUnit>();
                if (hero != null)
                {
                    hero.SetUnitData(unitData);
                    hero.SetHeroData(heroData);
                    hero.InitializeUnitStat();
                    hero.SetUnitGroup(unitGroup);
                }
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
    public void SelectSpawnData(int n)
    {
        spawnUnitData = unitDatas[n];
    }
    public void SetSpawnPoint(int point)
    {

    }
}
