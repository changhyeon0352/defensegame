using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    CameraMove camera;
    private void Awake()
    {
        camera=FindObjectOfType<CameraMove>();
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
        camera.CameraGetDir(dir);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        camera.CameraStop();
    }
}
