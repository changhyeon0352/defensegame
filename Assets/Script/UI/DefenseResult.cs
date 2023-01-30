using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DefenseResult : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI rewardTMP;
    [SerializeField]
    TextMeshProUGUI allyDieTMP;
    [SerializeField]
    TextMeshProUGUI allyDieGoldTMP;
    [SerializeField]
    TextMeshProUGUI townDamageTMP;
    [SerializeField]
    TextMeshProUGUI townDamageGoldTMP;
    [SerializeField]
    TextMeshProUGUI netRewardTMP;
    int reward = 10000;
    private void OnEnable()
    {
        int allyDieGold = DataMgr.Instance.DefenseData.allyDieNum * 100;
        int townDamageGold = (DataMgr.Instance.DefenseData.townHpMax - DataMgr.Instance.DefenseData.townHp) * 50;
        rewardTMP.text = $"{reward}";
        allyDieTMP.text = DataMgr.Instance.DefenseData.allyDieNum.ToString();
        allyDieGoldTMP.text = $"-{allyDieGold}";
        townDamageTMP.text=(DataMgr.Instance.DefenseData.townHpMax- DataMgr.Instance.DefenseData.townHp).ToString();
        townDamageGoldTMP.text = $"-{townDamageGold}";
        int netReward = reward - allyDieGold - townDamageGold;
        netRewardTMP.text = $"{netReward}";
        DataMgr.Instance.Money+=netReward;
    }
}
