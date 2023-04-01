using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;


public class UnitGroup:MonoBehaviour
{
    [SerializeField]List<Unit> unitList;
    public UnitType unitType = UnitType.none;
    public Transform unitsTr;
    public Transform spotsTr;
    //유닛그룹의 부모가 될 오브젝트
    public Transform AllyGroups;
    public Vector2Int rowColumn=Vector2Int.zero;

    BasicSkills groupSkill =BasicSkills.None;

    public BasicSkills GroupSkill { get => groupSkill; }
    public int NumUnitList { get => unitList.Count; }
    public List<Unit> UnitList { get => unitList; }

    private void Awake()
    {
        unitList = new List<Unit>();
        //AllyGroups = FindObjectOfType<AllyUnitGroups>().transform;
    }
    public void CheckSelected()
    {
        bool isSelected = (GameMgr.Instance.defenseMgr.SelectedGroupList.Contains(this));// this가 리스트에 속해 있다
        if(!isSelected&& GameMgr.Instance.defenseMgr.SelectedHero!=null) //선택되지 않았지만 히어로임
        {
            //이것안에 히어로가 선택된 히어로랑 일치
            isSelected = GameMgr.Instance.defenseMgr.SelectedHero==this.GetComponentInChildren<Hero>();
            if(isSelected)
            {
                HeroState.SelectedHeroUI();
            }
        }
        for(int i=0;i<unitList.Count;i++)
        {
            //unitList[i].IsSelectedUnit = isSelected;
        }
        
    }
    public void AddUnitList(Unit unit)
    {
        unitList.Add(unit);
    }
    public void SetUnitGroupSkill()
    {
        Debug.Log("이니셜라이즈");
        if (unitList.Count == 0)
            Destroy(this.gameObject);

        
        //for (int i = 0; i < unitsTr.childCount; i++)
        //{
        //    AllyUnit allyUnit = transform.GetChild(0).GetChild(i).GetComponent<AllyUnit>();
        //    unitList.Add(allyUnit);
        //}
        
        if (unitType==UnitType.soldier_Melee)
        {
            groupSkill |= BasicSkills.MoveToSpot;
            groupSkill |= BasicSkills.Charge;
        }
        else if (unitType == UnitType.soldier_Range)
        {
            groupSkill |= BasicSkills.ShootSpot;
            groupSkill |= BasicSkills.ShootEnemy;
        }
        
    }
    public void RemoveUnitFromList(Unit unit)
    {
        unitList.Remove(unit);
        Destroy(unit.gameObject);
        if(unitList.Count== 0)
        {
            //GameMgr.Instance.defenseMgr.SelectedGroupList.Remove(this);
            Destroy(gameObject);
        }
    }
    
}
