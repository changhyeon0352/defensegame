using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordCursor : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    UIMgr m_UIMgr;
    private void Start()
    {
        m_UIMgr = GameMgr.Instance.uiMgr;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameMgr.Instance.CommandMgr.UsingSkill != Skills.AttackMove)
            m_UIMgr.ChangeCursor(CursorType.Sword);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(GameMgr.Instance.CommandMgr.UsingSkill!=Skills.AttackMove)
            m_UIMgr.ChangeCursor(CursorType.Default);
    }
}
