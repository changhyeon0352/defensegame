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

    private void Awake()
    {
        inputActions= new PlayerInput();
        unitSetList= new List<GameObject>();
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
                transform.rotation *= Quaternion.Euler(0f, -360f * Time.deltaTime, 0f);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                transform.rotation *= Quaternion.Euler(0f, 360f * Time.deltaTime, 0f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                int unitCount=unitSetList.Count;
                for(int i=0; i<unitCount;i++)
                {
                    unitSetList[unitSetList.Count - 1].transform.parent = transform.parent;
                    unitSetList.RemoveAt(unitSetList.Count - 1);
                }
                
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
                //짝수면 n/2-0.5  홀수면 (n-1)/2 
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                ray.origin += unitOffset*(i-centerDiff)*transform.right ;
                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
                {

                    unitSetList[i].transform.position = hit.point;
                    //리스트에 따라 
                }

            }
        }
    }
    void AddUnitToList()
    {
        GameObject obj = Instantiate(meleeUnitPrefab, transform);
        unitSetList.Add(obj);
    }
    void RemoveLastToList()
    {
        GameObject obj = unitSetList[unitSetList.Count - 1];
        unitSetList.Remove(obj);
        Destroy(obj);
    }
}
