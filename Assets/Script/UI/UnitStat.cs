using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitStat : MonoBehaviour
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

    private Unit unit;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if(unit!=null)
        {
            RefreshUnitStatWindow(unit);
        }
    }

    public void RefreshUnitStatWindow(Unit unit)
    {
        this.unit = unit;
        nameTmp.text = unit.unitData.UnitName;
        atkTmp.text = unit.Attack.ToString();
        magicTmp.text = "0";
        atkSpeedTmp.text = unit.unitData.AttackSpeed.ToString();
        if (unit.ArmorPlus > 0)
            armorTmp.color = Color.blue;
        else { armorTmp.color = Color.white;}
        armorTmp.text = (unit.Armor + unit.ArmorPlus).ToString();
        magicArmorTmp.text = "0";
        moveSpeedTmp.text = unit.MoveSpeed.ToString();
        if(unit.MoveSpeed<unit.unitData.MoveSpeed)
            moveSpeedTmp.color = Color.red;
        else { moveSpeedTmp.color = Color.white;}
        
        portrait.sprite = unit.unitData.UnitPortrait;
        hpText.text = $"{unit.Hp } / {unit.HpMax}";
        hpImage.fillAmount= (float)unit.Hp/(float)unit.HpMax;
        if (unit.Hp == 0)
        {
            unit = null;
            this.gameObject.SetActive(false);
        }
    }
}
