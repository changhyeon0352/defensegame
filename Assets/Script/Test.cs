using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f))
            {
                Debug.Log(hit.transform.name);
            }
        }
    }
    public void ButtonClick()
    {
        Debug.Log("버튼 클릭됨");
    }
}
 
