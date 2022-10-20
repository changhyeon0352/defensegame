///////////////////////////////////////////////////////////////////////////
//  Archer Animations - BowLoadScript                                    //
//  Kevin Iglesias - https://www.keviniglesias.com/     			     //
//  Contact Support: support@keviniglesias.com                           //
///////////////////////////////////////////////////////////////////////////

// This script makes the bow animate when pulling arrow, also it makes thes
// arrow look ready to shot when drawing it from the quivers.

// To do the pulling animation, the bow mesh needs a blenshape named 'Load' 
// and the character needs an empty Gameobject in your Unity scene named 
// 'ArrowLoad' as a child, see character dummies hierarchy from the demo 
// scene as example. More information at Documentation PDF file.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias {
	public class BowLoadScript : MonoBehaviour
	{
	   
		public Transform bow;
		public Transform arrowLoad;   //평소엔 바닥에 쏠때 몸 중간 쯤으로 올라와서 화살이 로딩되는 만큼 뒤로감

        //Bow Blendshape
        SkinnedMeshRenderer bowSkinnedMeshRenderer;
		
		//Arrow draw & rotation
		public bool arrowOnHand;
		public Transform arrowToDraw;  //화살잡는 손
		public Transform arrowToShoot; //화살 쏘는척 할꺼
	   
		void Awake()
		{
			
			if(bow != null)
			{
				bowSkinnedMeshRenderer = bow.GetComponent<SkinnedMeshRenderer>();
			}
			
			if(arrowToDraw != null)
			{
				arrowToDraw.gameObject.SetActive(false);
			}
			if(arrowToShoot != null)
			{
				arrowToShoot.gameObject.SetActive(false);
			}
		}

		void Update()
		{
			//Bow blendshape animation 오른팔을 뒤로 뺄수록 활이 휘어짐
				if(bowSkinnedMeshRenderer != null && bow != null && arrowLoad != null)
				{
					float bowWeight = Mathf.InverseLerp(0, -0.7f, arrowLoad.localPosition.z);
                //위치를 백분률(0~1)로 0과 -0.7 사이 어디쯤인지 반환 
                    bowSkinnedMeshRenderer.SetBlendShapeWeight(0, bowWeight*100);//블렌더 가중치 활을 휘게함
				}
			
			//Draw arrow from quiver and rotate it  화살통에서 화살을 빼고 돌림
				if(arrowToDraw != null && arrowToShoot != null && arrowLoad != null)
				{
					if(arrowLoad.localPosition.y > 0.5f)
					{
						if(arrowToDraw != null)
						{
							arrowOnHand = true;
							arrowToDraw.gameObject.SetActive(true);// 화살(드로우용)이 보이게
						}
					}
					
					if(arrowLoad.localPosition.y > 0.5f)
					{
						if(arrowToDraw != null && arrowToShoot != null)
						{
							arrowToDraw.gameObject.SetActive(false);
							arrowToShoot.gameObject.SetActive(true);//화살(쏠) 이 보이게
						}
					}
					
					if(arrowLoad.localScale.z < 1f)//완전히 땡겨을 때
					{
						if(arrowToShoot != null)
						{
							arrowToShoot.gameObject.SetActive(false);
							arrowOnHand = false;
						}
					}
				}
		}
	}
}
