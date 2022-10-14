using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        //���� ���� ��ư
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
        
        //������
        if(unitSetList.Count>0)
        {
            //�� ����
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
            
            // ���ֱ׷� ȸ��
            if (Input.GetKey(KeyCode.Q))
            {
                unitGroupTr.rotation *= Quaternion.Euler(0f, -360f * Time.deltaTime, 0f);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                unitGroupTr.rotation *= Quaternion.Euler(0f, 360f * Time.deltaTime, 0f);
            }
            //���ּ��� �Ϸ�
            if (Input.GetMouseButtonDown(0))
            {
                ShaderChange(0);
                unitGroupTr.parent = transform.parent;
                unitSetList.Clear();
                unitGroupTr = null;
                //Debug.Log(transform.rotation);
                //transform.rotation = Quaternion.identity;
            }

            //���ֱ׷� ���Ľ�Ű��
            //¦���� n/2-0.5  Ȧ���� (n-1)/2 
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
            //���� �������� ����
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
        else // ���� ���� ��ġ��
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Unit")))
                {
                    unitGroupTr = hit.transform.parent;
                    unitGroupTr.parent = transform;
                    for(int i = 0; i < unitGroupTr.childCount; i++)
                    {
                        unitSetList.Add(unitGroupTr.GetChild(i).gameObject);
                    }
                }
                if(unitGroupTr!=null&&unitGroupTr.childCount==unitSetList.Count)
                {
                    ShaderChange(1);
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
            ShaderChange(1);
            unitSetList.Add(obj);

        }
        
    }
    void RemoveLastToList()
    {
        GameObject obj = unitSetList[unitSetList.Count - 1];
        unitSetList.Remove(obj);
        Destroy(obj);
    }
    void ShaderChange(int isSpawning1or0)
    {
        SkinnedMeshRenderer[] skinRen = unitGroupTr.GetComponentsInChildren<SkinnedMeshRenderer>();


        for (int i = 0; i < skinRen.Length; i++)
        {
            skinRen[i].material.SetInt("_IsSpawning", isSpawning1or0);
            if(isSpawning1or0==1)
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
