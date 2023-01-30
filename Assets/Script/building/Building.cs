using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public enum BuildingType { Barracks=0, Blacksmith,Guild, StartStatue }
public class Building : MonoBehaviour,IPointerEnterHandler,IPointerClickHandler,IPointerExitHandler
{
    
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if(CompareTag("Barracks"))
        {
            UIMgr.Instance.OpenBuildingsUI(0);
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Barracks, Color.red);
        }
        else if(CompareTag("StartStatue"))
        {
            GameMgr.Instance.ChangePhaseAction(1);
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.StartStatue, Color.red);
        }
        else if (CompareTag("Blacksmith"))
        {
            UIMgr.Instance.OpenBuildingsUI(1);
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Blacksmith, Color.red);
        }
        else if(CompareTag("Guild"))
        {
            UIMgr.Instance.OpenBuildingsUI(2);
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Guild, Color.red);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //transform.GetComponent<Outline>().OutlineColor = Color.red;
        
        if (CompareTag("Barracks"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Barracks, Color.red);
        }
        else if (CompareTag("StartStatue"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.StartStatue, Color.red);
        }
        else if (CompareTag("Blacksmith"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Blacksmith, Color.red);
        }
        else if (CompareTag("Guild"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Guild, Color.red);
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.GetComponent<Outline>().OutlineColor = Color.white;
        if (CompareTag("Barracks"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Barracks, Color.white);
        }
        else if (CompareTag("StartStatue"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.StartStatue, Color.white);
        }
        else if (CompareTag("Blacksmith"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Blacksmith, Color.white);
        }
        else if (CompareTag("Guild"))
        {
            UIMgr.Instance.ChangeBuildingNameColor(BuildingType.Guild, Color.white);
        }
    }

    
}
