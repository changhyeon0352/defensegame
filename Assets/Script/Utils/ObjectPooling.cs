using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ObjectPooling : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;
    protected int poolSize;
    protected List<GameObject> objectPool;
    protected void Start()
    {
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity, transform);
            obj.SetActive(false); // 처음에는 비활성화
            objectPool.Add(obj);
        }
    }
    protected abstract void InitializeObj(GameObject obj);
    public GameObject GetObjectFromPool()
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (!objectPool[i].activeInHierarchy)
            { // 활성화되어 있지 않은 오브젝트를 찾음
                InitializeObj(objectPool[i]);
                return objectPool[i];
            }
        }
        // 사용 가능한 오브젝트가 없으면 새로 생성
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity, transform);
        obj.SetActive(false);
        objectPool.Add(obj);
        InitializeObj(obj);
        return obj;
    }
    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        obj.transform.position = transform.position; // 위치 초기화
        obj.transform.rotation = Quaternion.identity; // 회전 초기화
    }
}
