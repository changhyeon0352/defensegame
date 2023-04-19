using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordCursor : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    


    public void OnPointerEnter(PointerEventData eventData)
    {
        UIMgr.Instance.CursorOnMonster(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIMgr.Instance.CursorOnMonster(false);
    }
}
