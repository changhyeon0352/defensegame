using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : AllyUnit
{
    private HeroData data;
    public bool isattackMove = false;

    //무브업데이트땐 적추적 ㄴㄴ
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
    //커맨드매니저에 영웅용 스킬
    //땅클릭하면 거기로 가기
    //몬스터 클릭하면 체이스타겟
    //영웅 선택시 일반 유닛과는 동시선택 불과 영웅들과만 동시선택가능
}
