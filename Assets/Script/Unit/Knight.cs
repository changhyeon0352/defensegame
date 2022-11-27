using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public enum KnightSkill{shieldAura,provoke,frenzy,finishMove }
public class Knight : MonoBehaviour
{
    public GameObject[] skillEffects;
    [SerializeField]LayerMask allyLayerMask;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] int shieldRadius = 5;
    float frenzyRadius = 4.5f;
    int frenzyDamage = 20;
    float provokeRadius = 9f;


    public IEnumerator shieldAuraCor;
    public IEnumerator frenzyCor;
    private void Awake()
    {
        shieldAuraCor = ShieldAuraCor();
        frenzyCor = FrenzyCor();
    }

    public IEnumerator EnumeratorTimer(IEnumerator enumerator,float sec)
    {
        StartCoroutine(enumerator);
        yield return new WaitForSeconds(sec);
        if(enumerator==shieldAuraCor)
        {
            SetShiedPlus(shieldRadius + 2, 0);
        }
        StopCoroutine(enumerator);
    }
    IEnumerator ShieldAuraCor()// 히어로에 방어력 적용이 안됨
    {
        for(int i=0;i<10000;i++)
        {
            SetShiedPlus(shieldRadius+2,0);
            SetShiedPlus(shieldRadius,10);
            yield return null;
        }
    }
    IEnumerator FrenzyCor()
    {
        for (int i = 0; i < 10000; i++)
        {
            Collider[] cols = Physics.OverlapSphere(GameMgr.Instance.commandMgr.SelectedHero.transform.position, frenzyRadius, enemyLayerMask);
            foreach (Collider col in cols)
            {
                Unit unit = col.gameObject.GetComponent<Unit>();
                unit.Hp -= frenzyDamage;

            }
            yield return new WaitForSeconds(1);
        }    
    }

    public IEnumerator ProvokeCor(float sec)
    {
        Collider[] cols = Physics.OverlapSphere(GameMgr.Instance.commandMgr.SelectedHero.transform.position, provokeRadius, enemyLayerMask);
        foreach (Collider col in cols)
        {
            Monster mon = col.gameObject.GetComponent<Monster>();
            mon.ProvokedBy(GameMgr.Instance.commandMgr.SelectedHero.transform);
        }
        yield return new WaitForSeconds(sec);
        foreach (Collider col in cols)
        {
            if(col.enabled)
            {
                Monster mon = col.gameObject.GetComponent<Monster>();
                mon.ChangeState(UnitState.Move);
            }
            
        }
    }
    

    private void SetShiedPlus(int range,int armorPlus)
    {
        Collider[] cols = Physics.OverlapSphere(GameMgr.Instance.commandMgr.SelectedHero.transform.position, range, allyLayerMask);
        foreach (Collider col in cols)
        {
            AllyUnit ally = col.gameObject.GetComponent<AllyUnit>();
            ally.armorPlus = 0;
        }
        GameMgr.Instance.commandMgr.SelectedHero.armorPlus = armorPlus * 2;
    }

    

    
    
    
    
}
