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
		public Transform arrowToDraw;  //화살 쏘는척 할꺼 
		public GameObject arrowToShoot; //화살 쏠꺼
        bool isShootingEnd = true;

        [SerializeField] float accuracy = 0f;
        public Transform target;
        Transform correctionTarget;
		void Awake()
		{


            if (bow != null)
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
            if(target != null)
            {
                if(correctionTarget == null||correctionTarget!=target)
                {
                    GameObject obj = new GameObject();
                    obj.transform.position=target.position;
                    obj.transform.Translate(transform.right * -0.25f);
                    correctionTarget = obj.transform;
                    correctionTarget.parent = target;
                }
                transform.LookAt(correctionTarget);

                //Bow blendshape animation 오른팔을 뒤로 뺄수록 활이 휘어짐
                if (bowSkinnedMeshRenderer != null && bow != null && arrowLoad != null)
                {
                    float bowWeight = Mathf.InverseLerp(0, -0.7f, arrowLoad.localPosition.z);
                    //위치를 백분률(0~1)로 0과 -0.7 사이 어디쯤인지 반환 
                    bowSkinnedMeshRenderer.SetBlendShapeWeight(0, bowWeight * 100);//블렌더 가중치 활을 휘게함
                }

                //Draw arrow from quiver and rotate it  화살통에서 화살을 빼고 돌림
                if (arrowToDraw != null && arrowToShoot != null && arrowLoad != null)
                {
                    if (isShootingEnd && arrowLoad.localPosition.y > 0.5f)
                    {
                        if (arrowToDraw != null)
                        {
                            arrowOnHand = true;
                            isShootingEnd = false;
                            arrowToDraw.gameObject.SetActive(true);// 화살(드로우용)이 보이게
                        }
                    }


                    if (arrowLoad.localScale.z < 1f && arrowOnHand)//완전히 땡겨을 때
                    {
                        if (arrowToDraw != null)
                        {
                            GameObject arrowObj = Instantiate(arrowToShoot, arrowToDraw.position, arrowToDraw.rotation);
                            arrowObj.SetActive(true);
                            Transform aTr = arrowObj.transform;
                            aTr.parent = null;
                            aTr.forward = transform.forward;

                            aTr.rotation *= Quaternion.Euler(new Vector3(-45, 0, 0));
                            float distance = Vector2.Distance(new Vector2(aTr.position.x, aTr.position.z),
                                new Vector2(target.position.x, target.position.z));
                            float deltaH = aTr.position.y - target.position.y;
                            Arrow arrow = aTr.GetComponent<Arrow>();
                            arrow.ShootVel = GetArrowVelocity(distance, deltaH, arrow.GravityForce);
                            Vector3 noise = Random.insideUnitSphere * (1 - accuracy);
                            arrow.MakeNoise(noise);
                            arrowToDraw.gameObject.SetActive(false);
                            arrowOnHand = false;
                        }
                    }
                    if (arrowLoad.localPosition == Vector3.zero)
                    {
                        isShootingEnd = true;
                    }
                }
            }
		
		}
        public float GetArrowVelocity(float distance, float deltaH, float gravityForce)
        {
            Debug.Log($"distance:{distance},H:{deltaH},gravity:{gravityForce}   ");
            float vel = 0;
            vel=Mathf.Sqrt(gravityForce*distance*distance/(distance+deltaH));
            return vel;
        }
	}
}
