using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

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
        GiveheroDataToSlot(data);//자기 데이터를 그대로 넣으면 제거됨
    }

    public HeroData Data
    {
        get { return data; }
    }
    public void GiveheroDataToSlot(HeroData data)
    {
        if(GameMgr.Instance.Phase==Phase.selectHero)
        {
            if (DataMgr.Instance.AddFightingHeroData(data))//만약 이미 추가된걸 보내면 삭제됨
            {
                heroImage.sprite = UIMgr.Instance.GetHeroSprite(data);
                heroImage.color = Color.white;
                this.data = data;
            }
            else
            {
                heroImage.color = Color.clear;//이미 추가된거면 UI투명 ,if안에서 세팅의 heroData는 삭제
                heroDataCard.AddorRemoveFromList(false);
                this.data = null;//UI매니저에서 
            }
        }
        else if(GameMgr.Instance.Phase==Phase.town)
        {
            heroImage.sprite = UIMgr.Instance.GetHeroSprite(data);
            heroImage.color = Color.white;
            this.data= data;
        }
    }

}
