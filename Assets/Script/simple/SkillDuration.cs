using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDuration : MonoBehaviour
{
    public TextMeshProUGUI skillName;
    public Image gage;
    float count = 1;
    float second = 1;

    public void InitSkillDurationUI(string name,float sec)
    {
        skillName.text = name;
        count = sec;
        second = sec;
    }
    private void Update()
    {
        count-=Time.deltaTime;
        gage.fillAmount=(second-count)/second;


        if(count < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
