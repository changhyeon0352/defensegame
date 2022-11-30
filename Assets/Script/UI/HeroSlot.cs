using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeroSlot : MonoBehaviour,IPointerClickHandler
{
    public HeroCard heroDataCard;
    private Image heroImage;
    private HeroData data;

    private void Awake()
    {
        heroImage=transform.GetChild(0).GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Data = data;//자기 데이터를 그대로 넣으면 제거됨
    }

    public HeroData Data
    {
        get { return data; }
        set 
        { 
            if(DataMgr.Instance.AddFightingHeroData(value))//만약 이미 추가된걸 보내면 삭제됨
            {
                heroImage.sprite = UIMgr.Instance.GetHeroSprite(value);
                heroImage.color = Color.white;
                data = value;
            }
            else
            {
                heroImage.color = Color.clear;//이미 추가된거면 UI투명 ,if안에서 세팅의 heroData는 삭제
                heroDataCard.AddorRemoveFromList(false);
                data=null;//UI매니저에서 

            }
        }
    }

}
