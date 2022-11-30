using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;


public class Knight : MonoBehaviour
{
    public GameObject[] skillEffects;
    [SerializeField]LayerMask allyLayerMask;
    [SerializeField] LayerMask enemyLayerMask;
    int frenzyDamage = 20;
    int finishDamage = 15;
    float[] skillRadius = { 5, 9, 3, 5 };
    float[] skillDurations = {10,10,5,0};
    float[] skillCools = { 20, 15, 10, 50 };
    public float[] SkillCools { get { return skillCools; } }
    public float[] SkillDurations { get => skillDurations; }
    public float[] SkillRadius { get => skillRadius; }


    public IEnumerator shieldAuraCor;
    public IEnumerator frenzyCor;
    public IEnumerator finishMoveCor;
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
            SetShiedPlus(skillRadius[0] + 2, 0);
        }
        StopCoroutine(enumerator);
    }
    IEnumerator ShieldAuraCor()// 히어로에 방어력 적용이 안됨
    {
        for(int i=0;i<10000;i++)
        {
            SetShiedPlus(skillRadius[0] + 2,0);
            SetShiedPlus(skillRadius[0], 10);
            yield return null;
        }
    }
    IEnumerator FrenzyCor()
    {
        for (int i = 0; i < 10000; i++)
        {
            Collider[] cols = Physics.OverlapSphere(GameMgr.Instance.commandMgr.SelectedHero.transform.position, skillRadius[2], enemyLayerMask);
            foreach (Collider col in cols)
            {
                Unit unit = col.gameObject.GetComponent<Unit>();
                unit.Hp -= frenzyDamage;

            }
            yield return new WaitForSeconds(1);
        }    
    }

    public void Provoke(float sec)
    {
        Collider[] cols = Physics.OverlapSphere(GameMgr.Instance.commandMgr.SelectedHero.transform.position, skillRadius[1], enemyLayerMask);
        foreach (Collider col in cols)
        {
            Monster mon = col.gameObject.GetComponent<Monster>();
            StartCoroutine(mon.ProvokedBy(GameMgr.Instance.commandMgr.SelectedHero.transform, sec));
            mon.isProvoked = true;
        }
       
    }
    public IEnumerator FinishMove(Unit targetUnit)
    {
        WaitForSeconds sec = new WaitForSeconds(0.1f);
        for (int i = 0; i < 16; i++)
        {
            yield return sec;
            targetUnit.TakeDamage(finishDamage);
        }
    }
    private void SetShiedPlus(float range,int armorPlus)
    {
        Collider[] cols = Physics.OverlapSphere(GameMgr.Instance.commandMgr.SelectedHero.transform.position, range, allyLayerMask);
        foreach (Collider col in cols)
        {
            AllyUnit ally = col.gameObject.GetComponent<AllyUnit>();
            ally.ArmorPlus = armorPlus;
        }
        GameMgr.Instance.commandMgr.SelectedHero.ArmorPlus = armorPlus * 2;
    }

    

    
    
    
    
}
