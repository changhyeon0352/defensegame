using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnitHpBar : MonoBehaviour
{
    [SerializeField]
    Image hpImg;
    Transform unitTr;

    public void UpdateHpBar(int hp,int hpMax)
    {
        hpImg.fillAmount=(float)hp/(float)hpMax;
    }
    private void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(unitTr.transform.position + Vector3.left);
    }

    public void SetUnitInfo(Unit unit)
    {
        unitTr = unit.transform;
        hpImg.color = unit.CompareTag("Monster") ? Color.red : Color.blue;
    }
}
