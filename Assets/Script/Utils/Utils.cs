using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public static class Utils
{
    public static bool IsClickOnUI()
    {
        var pointerEventData = new PointerEventData(EventSystem.current)//마우스 혹은 터치입력 이벤트의 정보들이 담겨있음 무슨 버튼에 마우스위치 등등
        {
            position = Mouse.current.position.ReadValue()//pointerEventData안에 position이 있음
        };

        var raycastResultsList = new List<RaycastResult>();
        //ui랑 카메라의 Physics Raycaster에 컬링마스크 되어있는 레이어만 레이캐스팅함
        EventSystem.current.RaycastAll(pointerEventData, raycastResultsList);
        //return raycastResultsList.Any(result => result.gameObject is GameObject);//게임오브젝트로 캐스팅되는게 한게라도 있으면 true인듯?

        return (raycastResultsList.Count > 0) && (raycastResultsList[0].gameObject.layer!=3);
        //일반적인 오브젝트는 피직스레이캐스트올하고 raycasthit에 받는데
    }
}
