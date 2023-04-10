using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordCursor : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    


    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if(GameMgr.Instance.skillController.SkillType == SkillType.UnitTarget)
        {
            UIMgr.Instance.ChangeCursor(CursorType.targeting);
        }
        //else if (GameMgr.Instance.heroController.UsingSkill != BasicSkills.AttackMove)
        //    UIMgr.Instance.ChangeCursor(CursorType.Sword);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameMgr.Instance.skillController.SkillType == SkillType.UnitTarget)
        {
            UIMgr.Instance.ChangeCursor(CursorType.findTarget);
        }
        //else if (GameMgr.Instance.heroController.UsingSkill != BasicSkills.AttackMove)
        //    UIMgr.Instance.ChangeCursor(CursorType.Default);
    }
}
