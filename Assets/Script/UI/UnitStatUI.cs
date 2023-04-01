using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitStatUI : MonoBehaviour
{
    [SerializeField] Image thisImage;
    [SerializeField] TextMeshProUGUI atkTmp;
    [SerializeField] TextMeshProUGUI magicTmp;
    [SerializeField] TextMeshProUGUI atkSpeedTmp;
    [SerializeField] TextMeshProUGUI armorTmp;
    [SerializeField] TextMeshProUGUI magicArmorTmp;
    [SerializeField] TextMeshProUGUI moveSpeedTmp;
    [SerializeField] TextMeshProUGUI nameTmp;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Image portrait;
    [SerializeField] Image hpImage;
    public bool isRefresh = false;

    private void Update()
    {
        if(unit!=null)
        {
            RefreshUnitStatWindow();
        }
    }
    private Unit unit;
    
    public void RefreshUnitStatWindow()
    {
        atkTmp.text = unit.AttackPoint.ToString();
        magicTmp.text = "0";
        atkSpeedTmp.text = unit.UnitData.AttackSpeed.ToString();
        if (unit.Armor > unit.UnitData.Armor)
            armorTmp.color = Color.green;
        else { armorTmp.color = Color.white; }
        armorTmp.text = (unit.Armor).ToString();
        magicArmorTmp.text = "0";
        moveSpeedTmp.text = unit.MoveSpeed.ToString();
        if (unit.MoveSpeed < unit.UnitData.MoveSpeed)
            moveSpeedTmp.color = Color.red;
        else { moveSpeedTmp.color = Color.white; }
        hpText.text = $"{unit.Hp} / {unit.HpMax}";
        hpImage.fillAmount = (float)unit.Hp / (float)unit.HpMax;
        if (unit.Hp == 0)
        {
            unit = null;
            this.gameObject.SetActive(false);
        }
    }
    public void RefreshUnitStatWindow(Unit unit)
    {
        if (this.unit != unit)
        {
            this.unit = unit;
            nameTmp.text = unit.UnitData.UnitName;
            portrait.sprite = unit.UnitData.UnitPortrait;

        }
    }
}
