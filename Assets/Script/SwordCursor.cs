using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordCursor : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CommandMgr.Instance.UsingSkill != Skills.AttackMove)
            UIMgr.Instance.ChangeCursor(CursorType.Sword);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(CommandMgr.Instance.UsingSkill !=Skills.AttackMove)
            UIMgr.Instance.ChangeCursor(CursorType.Default);
    }
}
