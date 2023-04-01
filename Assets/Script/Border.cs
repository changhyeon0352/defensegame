using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Border : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private  CameraMove cameraMove;
    private void Awake()
    {
        cameraMove=FindObjectOfType<CameraMove>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 dir=Vector2.zero;
        if(Input.mousePosition.x>1800)
        {
            dir = Vector2.right;
        }
        else if(Input.mousePosition.x<50)
        {
            dir=Vector2.left;
        }
        if(Input.mousePosition.y>1000)
        {
            dir = dir + Vector2.up;
        }
        else if(Input.mousePosition.y<50)
        {
            dir = dir - Vector2.up;
        }
        cameraMove.CameraGetDir(dir);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cameraMove.CameraStop();
    }
}
