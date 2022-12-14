using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    public GameObject hpPrefabMon;
    public GameObject hpPrefabSoldier;
    List<Unit> unitList =new List<Unit>();
    List<GameObject> hpBarList =new List<GameObject>();
    GameObject hpBarGate;
    Gate gate;
    Camera m_cam;
    // Start is called before the first frame update
    void Start()
    {
        m_cam = Camera.main;
        GameObject[] t_objects1 = GameObject.FindGameObjectsWithTag("Soldier");
        GameObject[] t_objects2 = GameObject.FindGameObjectsWithTag("Monster");
        gate = FindObjectOfType<Gate>();
        hpBarGate = Instantiate(hpPrefabSoldier, gate.transform.position, Quaternion.identity, transform);
        hpBarGate.transform.localScale = Vector3.one * 2;
        
        for (int i = 0; i < t_objects1.Length; i++)
        {
            unitList.Add(t_objects1[i].GetComponent<Unit>());
            GameObject hpBar = Instantiate(hpPrefabSoldier, t_objects1[i].transform.position, Quaternion.identity, transform);
            hpBarList.Add(hpBar);
        }
        for(int i=0; i < t_objects2.Length; i++)
        {
            unitList.Add(t_objects2[i].GetComponent<Unit>());
            GameObject hpBar = Instantiate(hpPrefabMon, t_objects2[i].transform.position, Quaternion.identity, transform);
            hpBarList.Add(hpBar);
        }
        
    }
    public void AddHpBar(Unit unit)
    {
        unitList.Add(unit);
        if (unit.CompareTag("Soldier"))
        {
            GameObject hpBar = Instantiate(hpPrefabSoldier, unit.transform.position, Quaternion.identity, transform);
            hpBarList.Add(hpBar);
        }
        else if(unit.CompareTag("Monster"))
        {
            GameObject hpBar = Instantiate(hpPrefabMon,unit.transform.position, Quaternion.identity, transform);
            hpBarList.Add(hpBar);
        }
    }

    public void ChangeHPbar(Unit unit,float hpPercent)
    {
        int index=unitList.FindIndex(x => x==unit);
        Debug.Log(index);
        if(index>=0)
        {
            hpBarList[index].transform.GetChild(0).GetComponent<Image>().fillAmount = hpPercent;
            if(hpPercent==0)
            {
                unitList.RemoveAt(index);
                GameObject obj = hpBarList[index];
                hpBarList.Remove(obj);
                Destroy(obj);
            }
        }
    }
    public void ChangeHPbarGate(float hpPercent)
    {
        hpBarGate.transform.GetChild(0).GetComponent<Image>().fillAmount = hpPercent;
        if (hpPercent <= 0)
        {
            Destroy(hpBarGate.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        for(int i=0;i<unitList.Count;i++)
        {
            if (unitList[i]==null)
            {
                unitList.RemoveAt(i);
                GameObject obj = hpBarList[i];
                hpBarList.Remove(obj);
                Destroy(obj);
            }
            else
            {
                hpBarList[i].transform.position = m_cam.WorldToScreenPoint(unitList[i].transform.position + Vector3.left);
            }
        }
        if(hpBarGate!=null&&gate!=null)
            hpBarGate.transform.position= m_cam.WorldToScreenPoint(gate.transform.position + Vector3.left*2); 
    }
}
