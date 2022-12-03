using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class Hero : AllyUnit
{
    private HeroData data;
    public HeroData Data { get=>data;  
        set { data = value;
            for(int i=0;i<skillCools.Length;i++)
            {
                skillCools[i] = GameMgr.Instance.skillMgr.knight.SkillCools[i];
            }
        } 
    }
    public HeroState heroState;
    public float range;
    public bool isattackMove = false;
    float[] skillCoolsLeft = { -1, -1, -1, -1 };
    public float[] SkillCoolLeft { get => skillCoolsLeft; }
    float[] skillCools = new float[4];
    public float[] SkillCools { get => skillCools; }
    bool[] skillCanUse = { true, true, true, true };
    public bool[] SkillCanUse { get { return skillCanUse; } }
    public IEnumerator SkillCoolCor(int num,float coolTime)
    {
        skillCanUse[num] = false;
        skillCoolsLeft[num]=coolTime;
        for (int i=0; i<10000;i++)
        {
            skillCoolsLeft[num]-=Time.deltaTime;
            if (skillCoolsLeft[num] < 0)
            {
                skillCanUse[num]=true;
                Debug.Log($"{num + 1}번째 스킬사용가능");
                break;
            }
                
            yield return null;
        }
    }
    public bool CheckSkillCool(int skillNum)
    {
        bool result = false;
        if (skillCoolsLeft[skillNum]<0)
        {
            result = true;
        }
        return result;
    }
    public override int Hp
    {
        get => hp;
        set 
        { 
            base.Hp = value;
            if (hp <= 0)
            {
                hp = 0;
                //죽음
                if (state != UnitState.Dead)
                {
                    Die();
                }
            }
            heroState.ShowHpMp(this);
        }
    }
    public new int Mp
    {
        get => mp;
        set
        {
            mp = value;
            heroState.ShowHpMp(this);
        }
    }

    protected override void Update()
    {
        base.Update();
        for(int i=0;i<4;i++)
        {
           
        }

    }

    //무브업데이트땐 적추적 ㄴㄴ
    public static Hero FindHero(HeroData herodata)
    {
        Hero result = new();
        Hero[] heros=FindObjectsOfType<Hero>();
        foreach(Hero hero in heros)
        {
            if(hero.data == herodata)
            {
                result = hero;
            }
        }
        return result;
    }
    protected override void MoveUpdate()
    {
        if(isattackMove)
        {
            base.MoveUpdate();
        }
        else
        {
            agent.SetDestination(goalTr.position);          //목표로 가기
            if (agent.remainingDistance < stopRange && !agent.pathPending)                  //목표에 다가가면 Idle로 변경
            {
                ChangeState(UnitState.Idle);
            }
        }       
    }
    public void MoveSpots(Vector3 position)
    {
        UnitGroup unitgroup =transform.parent.parent.GetComponent<UnitGroup>();
        unitgroup.spotsTr.position = position;
        ChangeState(UnitState.Move);
    }
    public void SetChaseTarget(Transform targetTr)
    {
        chaseTargetTr = targetTr;
    }
    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position, transform.up, range);
    }

   
}
