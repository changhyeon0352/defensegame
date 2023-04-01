using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitView : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    Unit unit;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        unit = GetComponent<Unit>();
    }

    public void PlayAttackAnimation(int combo)
    {
        anim.SetInteger("AttackCombo", combo);
        anim.SetTrigger("Attack");
    }
    public void PlayAttackAnimation()
    {
        anim.SetTrigger("Attack");
    }
    public void UpdateHPbar()
    {
        UIMgr.Instance.hpbar.ChangeHPbar(unit, (float)unit.UnitStat.hp / (float)unit.UnitStat.hpMax);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetComponent<Outline>().enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetComponent<Outline>().enabled = false;
    }
}
