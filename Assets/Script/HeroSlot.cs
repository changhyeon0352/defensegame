using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour
{
    private Image heroImage;
    private HeroData data;

    private void Awake()
    {
        heroImage=transform.GetComponentInChildren<Image>();
    }
    public HeroData Data
    {
        get { return data; }
        set 
        { 
            SettingMgr.Instance.AddFightingHeroData(data);
            heroImage.sprite=UIMgr.Instance.GetHeroSprite(value);
            heroImage.color=Color.white;
            data = value; 
        }
    }
}
