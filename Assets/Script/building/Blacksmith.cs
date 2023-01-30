using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Blacksmith : MonoBehaviour
{
    [SerializeField]
    GameObject upgradeUI;
    [SerializeField]
    HeroSlot heroSlot;
    [SerializeField]
    Transform[] upgladeTrs;
    [SerializeField]
    TextMeshProUGUI heroNameTMP;

    public void InitBlacksmithHeroInfo()
    {
        int level = heroSlot.Data.level;
        int lvWeapon = heroSlot.Data.level_Weapon;
        int lvAromor = heroSlot.Data.level_Armor;
        int[] lvs=new int[2] {lvWeapon, lvAromor};
        heroNameTMP.text = heroSlot.Data.name;
        for (int i = 0; i < upgladeTrs.Length; i++)
        {
            UpgladeSlot[] Slots = upgladeTrs[i].Find("Upgrade").GetComponentsInChildren<UpgladeSlot>();
            upgladeTrs[i].Find("line").GetComponent<Image>().fillAmount = 0.25f * lvs[i];
            int j = 0;
            foreach (var slot in Slots)
            {
                string name= DataMgr.Instance.GetBlacksmithInfo(UpgradeInfoType.name, i, j);
                string cost = DataMgr.Instance.GetBlacksmithInfo(UpgradeInfoType.cost, i, j);
                int iCost = int.Parse(cost);
                string value= DataMgr.Instance.GetBlacksmithInfo(UpgradeInfoType.value, i, j);
                if (j > lvs[i])
                {
                    slot.Initialize(i,j+1,$"{name}  {value}증가 \n 비용:{cost}", 0, iCost,BuildingType.Blacksmith);
                }
                else if (j == lvs[i])
                {
                    int spriteNum = 1;
                    if (level < j+1)
                        spriteNum = 0;
                    slot.Initialize(i, j + 1, $"{name}  {value}증가 \n 비용:{cost}", spriteNum, iCost, BuildingType.Blacksmith,true);
                }
                else
                {
                    slot.Initialize(i, j + 1, $"{name}  {value}증가 \n 비용:{cost}", 2, iCost, BuildingType.Blacksmith);
                }
                slot.heroData = heroSlot.Data;
                j++;
            }
        }
    }
}
