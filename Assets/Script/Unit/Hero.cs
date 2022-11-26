using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
public class Hero : AllyUnit
{
    public HeroData data;
    public HeroState heroState;
    public float range;
    public bool isattackMove = false;
    public override int Hp
    {
        get => hp;
        set 
        { 
            base.Hp = value;
            heroState.Hp(value,hpMax);
        }
    }
    public int Mp
    {
        get => mp;
        set
        {
            mp = value;
            heroState.Mp(value, mpMax);
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
