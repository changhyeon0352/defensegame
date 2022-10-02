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
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (unitSetList.Count==0)
            {
                GameObject obj = new GameObject();
                obj.name = "unitGroup";
                unitGroupTr = obj.transform;
                unitGroupTr.parent = transform;

                AddUnitToList();
            }
        }
        
        
        if(unitSetList.Count>0)
        {

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
            

            if (Input.GetKey(KeyCode.Q))
            {
                unitGroupTr.rotation *= Quaternion.Euler(0f, -360f * Time.deltaTime, 0f);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                unitGroupTr.rotation *= Quaternion.Euler(0f, 360f * Time.deltaTime, 0f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                unitGroupTr.parent = transform.parent;
                unitSetList.Clear();
                unitGroupTr = null;
                //Debug.Log(transform.rotation);
                //transform.rotation = Quaternion.identity;
            }

            float centerDiff = 0;

            if(unitSetList.Count%2==0)
            {
                centerDiff=unitSetList.Count/2-0.5f;
            }
            else
            {
                centerDiff=(unitSetList.Count-1)/2;
            }


            for (int i = 0; i < unitSetList.Count; i++)
            {
                //¦���� n/2-0.5  Ȧ���� (n-1)/2 
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                ray.origin += unitOffset*(i-centerDiff)*unitGroupTr.right ;
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                {

                    unitSetList[i].transform.position = hit.point;
                    //����Ʈ�� ���� 
                }

            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Unit")))
                {
                    unitGroupTr = hit.transform.parent;
                    unitGroupTr.parent = transform;
                    //transform.rotation = unitGroupTr.rotation;
                    for(int i = 0; i < unitGroupTr.childCount; i++)
                    {
                        unitSetList.Add(unitGroupTr.GetChild(i).gameObject);
                    }
                    //����Ʈ�� ���� 
                }
            }
        }
    }
    void AddUnitToList()
    {
        GameObject obj = Instantiate(meleeUnitPrefab, unitGroupTr);
        unitSetList.Add(obj);
    }
    void RemoveLastToList()
    {
        GameObject obj = unitSetList[unitSetList.Count - 1];
        unitSetList.Remove(obj);
        Destroy(obj);
    }
}
