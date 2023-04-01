using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeployView : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI pointText;
    [SerializeField]
    TextMeshProUGUI pointMaxText;
    [SerializeField]
    Image[] unitButtonImages;
    public void SelectSpawnUnitUI(int cost,int index)
    {
        for (int i = 0; i < unitButtonImages.Length; i++)
        {
            if (i == index)
            {
                unitButtonImages[i].transform.localScale = Vector3.one * 1.1f;
                unitButtonImages[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = cost.ToString();
            }
            else
            {
                unitButtonImages[i].transform.localScale = Vector3.one * 1f;
                unitButtonImages[i].transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
    public void ShaderChange(UnitShader _type, SkinnedMeshRenderer[] skinRen)
    {
        for (int i = 0; i < skinRen.Length; i++)
        {
            skinRen[i].material.SetInt("_IsSpawning", (int)_type);
            if (_type == UnitShader.transparentShader)
            {
                skinRen[i].material.SetFloat("_Alpha", 0.2f);
            }
            else
            {
                skinRen[i].material.SetFloat("_Alpha", 1f);
            }
        }
    }

    public void SetPoint(int point,int maxPoint)
    {
        pointText.text = point.ToString();
        pointMaxText.text=maxPoint.ToString();
    }
}
