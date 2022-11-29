using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitGroup:MonoBehaviour
{
    List<AllyUnit> unitList;
    private UnitType unitType = UnitType.none;
    public Transform unitsTr;
    public Transform spotsTr;
    public Transform AllyGroups;
    public Vector2Int rowColumn=Vector2Int.zero;

    BasicSkills groupSkill =BasicSkills.None;

    public BasicSkills GroupSkill { get => groupSkill; }
    public UnitType UnitType { get => unitType;}

    private void Awake()
    {
        unitList = new List<AllyUnit>();
        AllyGroups = FindObjectOfType<AllyUnitGroups>().transform;
    }
    public void CheckSelected()
    {
        bool isSelected = (GameMgr.Instance.commandMgr.SelectedGroupList.Contains(this));// this가 리스트에 속해 있다
        if(!isSelected&& GameMgr.Instance.commandMgr.SelectedHero!=null) //선택되지 않았지만 히어로임
        {
            //이것안에 히어로가 선택된 히어로랑 일치
            isSelected = GameMgr.Instance.commandMgr.SelectedHero==this.GetComponentInChildren<Hero>();
            if(isSelected)
            {
                HeroState.SelectedHeroUI();
            }
        }
        for(int i=0;i<unitList.Count;i++)
        {
            unitList[i].IsSelectedUnit = isSelected;
        }
        
    }
    public void InitializeUnitList()
    {
        unitList.Clear();
        
        for (int i = 0; i < unitsTr.childCount; i++)
        {
            AllyUnit allyUnit = transform.GetChild(0).GetChild(i).GetComponent<AllyUnit>();
            unitList.Add(allyUnit);
        }
        
        tag = transform.GetChild(0).GetChild(0).tag;
        if (CompareTag("Melee"))
        {
            groupSkill |= BasicSkills.MoveToSpot;
            groupSkill |= BasicSkills.Charge;
            unitType = UnitType.soldier;
        }
        else if (CompareTag("Range"))
        {
            groupSkill |= BasicSkills.Shoot;
            unitType = UnitType.soldier;
        }
        else if(CompareTag("Knight"))
        {
            unitType = UnitType.hero;
        }
    }
    public void RemoveUnitFromList(AllyUnit unit)
    {
        unitList.Remove(unit);
        if(unitList.Count== 0)
        {
            GameMgr.Instance.commandMgr.SelectedGroupList.Remove(this);
            Destroy(gameObject);
        }
    }
    
}
