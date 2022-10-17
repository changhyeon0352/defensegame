using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum unitShader
{
    normalShader=0,
    transparentShader
}
    

public class SettingManager : MonoBehaviour
{
    public GameObject meleeUnitPrefab;
    PlayerInput inputActions;
    List<GameObject> unitSetList;
    float unitOffset = 1.5f;
    public float scrollSpeed=1f;
    private Transform unitGroupTr;
    private int num_row;

    private void Awake()
    {
        inputActions= new PlayerInput();

        unitSetList = new List<GameObject>();
    }
    private void OnEnable()
    {
        inputActions.Setting.Enable();
        
    }
    private void OnDisable()
    {
        inputActions.Setting.Disable();
    }
   
    private void Update()
    {
        //세팅 시작 버튼
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (unitSetList.Count==0)
            {
                GameObject obj = new GameObject();
                obj.name = "unitGroup";
                unitGroupTr = obj.transform;
                unitGroupTr.parent = transform;
                num_row = 1;
                AddUnitToList();
            }
        }
        
        //세팅중
        if(unitSetList.Count>0)
        {
            //휠 변경
            if(Input.GetAxis("Mouse ScrollWheel")>0)
            {
                AddUnitToList();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if(unitSetList.Count>1)
                {
                   RemoveLastToList();
                }
            }
            
            // 유닛그룹 회전
            if (Input.GetKey(KeyCode.Q))
            {
                unitGroupTr.rotation *= Quaternion.Euler(0f, -360f * Time.deltaTime, 0f);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                unitGroupTr.rotation *= Quaternion.Euler(0f, 360f * Time.deltaTime, 0f);
            }
            //유닛세팅 완료
            if (Input.GetMouseButtonDown(0))
            {
                GameObject obj = new GameObject();
                obj.name = "unitSpots";
                
                for (int i=0;i<unitGroupTr.childCount;i++)
                {
                    GameObject spot = new GameObject();
                    spot.name = $"spot{i}";
                    spot.transform.position = unitGroupTr.GetChild(i).position;
                    spot.transform.rotation = unitGroupTr.rotation;
                    spot.transform.parent = obj.transform;
                    Unit unit = unitGroupTr.GetChild(i).GetComponent<Unit>();
                    unit.goalTr = spot.transform;
                    unit.ChangeState(unitState.Move);
                }
                obj.transform.parent = unitGroupTr;

                ShaderChange(unitShader.normalShader);

                unitGroupTr.parent = transform.parent;//원래 이거자식이 unitGroupTr인데 형제로 격상
                unitSetList.Clear();
                unitGroupTr = null;
                
            }

            //유닛그룹 정렬시키기
            //짝수면 n/2-0.5  홀수면 (n-1)/2 
            float centerDiff = 0;
            if((unitSetList.Count/num_row)%2==0)
            {
                centerDiff=(unitSetList.Count/num_row)/2-0.5f;
            }
            else
            {
                centerDiff=((unitSetList.Count / num_row)-1)/2;
            }
            for (int i = 0; i < unitSetList.Count/num_row; i++)
            {
                for(int j=0;j<num_row;j++)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                    ray.origin += unitOffset *( (i - centerDiff)*unitGroupTr.right- unitGroupTr.forward*j);
                    if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                    {
                        unitSetList[i*num_row+j].transform.position = hit.point;
                    }
                }
                
            }
            //유닛 몇행으로 설지
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeGroupRow(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeGroupRow(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeGroupRow(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                ChangeGroupRow(4);
            }
        }
        else // 유닛 세팅 고치기
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ally")))
                {
                    unitGroupTr = hit.transform.parent;
                    GameObject objDestroy = unitGroupTr.GetChild(unitGroupTr.childCount - 1).gameObject;
                    objDestroy.transform.parent = null;
                    Destroy(objDestroy);
                    unitGroupTr.parent = transform;
                    for(int i = 0; i < unitGroupTr.childCount; i++)
                    {
                        unitSetList.Add(unitGroupTr.GetChild(i).gameObject);
                    }
                    if (unitGroupTr != null && unitGroupTr.childCount == unitSetList.Count)
                    {
                        ShaderChange(unitShader.transparentShader);
                    }
                }
                
            }
        }
    }
    void ChangeGroupRow(int iRow)
    {
        if(unitSetList.Count>0)
        {
            UnitListClear();
            num_row=iRow;
            AddUnitToList();
        }
    }
    void UnitListClear()
    {
        for(int i=0;i<unitSetList.Count;i++)
        {
            RemoveLastToList();
        }
    }
    void AddUnitToList()
    {
        for(int i=0;i<num_row;i++)
        {
            GameObject obj = Instantiate(meleeUnitPrefab, unitGroupTr);
            ShaderChange(unitShader.transparentShader);
            unitSetList.Add(obj);

        }
        
    }
    void RemoveLastToList()
    {
        GameObject obj = unitSetList[unitSetList.Count - 1];
        unitSetList.Remove(obj);
        Destroy(obj);
    }
    void ShaderChange(unitShader _type)
    {
        SkinnedMeshRenderer[] skinRen = unitGroupTr.GetComponentsInChildren<SkinnedMeshRenderer>();


        for (int i = 0; i < skinRen.Length; i++)
        {
            skinRen[i].material.SetInt("_IsSpawning", (int)_type);
            if(_type==unitShader.transparentShader)
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
