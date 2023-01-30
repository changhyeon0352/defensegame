using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Barracks : MonoBehaviour
{
    public Transform[] upgladeTrs;
    private void Start()
    {
        InitBarracks();
    }
    public void InitBarracks()
    {
        //i는 어떤 타입의 업그레이드 인지 j는 몇 단계인지
        for (int i = 0; i < upgladeTrs.Length; i++)
        {
            int upgradeNum = DataMgr.Instance.barracksData.upgladeNums[i];
            UpgladeSlot[] Slots = upgladeTrs[i].Find("Upgrade").GetComponentsInChildren<UpgladeSlot>();
            upgladeTrs[i].Find("line").GetComponent<Image>().fillAmount = 0.25f * upgradeNum;
            int j = 0;
            foreach (var slot in Slots)
            {
                string name = DataMgr.Instance.GetBarrackInfo(UpgradeInfoType.name, (BarracksUpgradeType)i,j);
                string cost = DataMgr.Instance.GetBarrackInfo(UpgradeInfoType.cost, (BarracksUpgradeType)i,j);
                int iCost = int.Parse(cost);
                string value = DataMgr.Instance.GetBarrackInfo(UpgradeInfoType.value, (BarracksUpgradeType)i, j);
                if (j > upgradeNum)
                {
                    //img.sprite = upgladeCheckSprite[0];
                    slot.Initialize(i, j + 1, $"{name} {value}증가 \n 비용:{cost}",0,iCost, BuildingType.Barracks);
                }
                else if (j == upgradeNum)
                {
                    slot.Initialize(i, j + 1, $"{name} {value}증가 \n 비용:{cost}", 1, iCost, BuildingType.Barracks, true);
                }
                else
                {
                    slot.Initialize(i, j + 1, $"{name} {value}증가 \n 비용:{cost}", 2, iCost, BuildingType.Barracks);
                }
                j++;
            }
        }
    }
}
