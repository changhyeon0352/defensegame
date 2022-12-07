using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
public enum HeroSkill { none,shieldAura, provoke, vortex, finishMove }
public enum SkillType { OnHero, Targrt, NonTarget }
public enum Buff { provoked,sleep,shield}
public class SkillMgr : MonoBehaviour
{
    public GameObject[] Buffs;
    public GameObject skillRangePrefab;
    [SerializeField]
    Skill[] knigjtSkills;
    Skill[] mageSkills;
    Skill usingSkill;
    SkillType skillMode;
    bool isUsingSkill = false;
    public bool IsUsingSkill { get { return isUsingSkill; } }
    bool isChasingForSkill=false;
    public bool IsChasingForSkill { get { return isChasingForSkill; } }
    public SkillType SkillMode { get { return skillMode; } }
    public Hero selectedHero;
    float skillRange;
    public float SkillRange { get { return skillRange; } }
    GameObject skillRangeObj;
    //private IEnumerator skillNow;//스킬누르고 클릭해야 나가는 스킬 처리용

    

    //모든 스킬의 정보 관리
    // 쿨타임 관리
    //이펙트 소환
    // 인디케이터 표시

    public Knight knight;
    private void OnEnable()
    {
        GameMgr.Instance.inputActions.Command.SkillButton1.performed += OnSkill1;
        GameMgr.Instance.inputActions.Command.SkillButton2.performed += OnSkill2;
        GameMgr.Instance.inputActions.Command.SkillButton3.performed += OnSkill3;
        GameMgr.Instance.inputActions.Command.SkillButton4.performed += OnSkill4;
        GameMgr.Instance.inputActions.Command.HeroSkillClick.performed += HeroSkillClick;
        GameMgr.Instance.inputActions.Command.SkillCancel.performed += OnSkillCancel;
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Disable();
        selectedHero = GameMgr.Instance.commandMgr.SelectedHero;

    }

    private void HeroSkillClick(InputAction.CallbackContext obj)
    {
        if (skillMode == SkillType.Targrt)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Monster")))
            {
                Vector3 heroPos = selectedHero.transform.position;
                float distance = Vector2.Distance(new Vector2(heroPos.x, heroPos.z), new Vector2(hit.point.x, hit.point.z));
                Unit skillTarget = hit.transform.GetComponent<Unit>();
                Debug.Log(distance);
                if (skillTarget == null)
                {
                    return;//무반응하기
                }
                if (distance > skillRange)//사거리 밖일때
                {
                    isChasingForSkill = true;
                    SkillEnd(true);
                    selectedHero.ChaseTarget(skillTarget.transform);
                    return;
                }
                //ExecuteSkill(skillTarget,usingSkill);
            }
            else
            {
                return;//무반응하기
            }
        }
    }
    public void ExecuteSkill(Transform skillTarget)
    {
        Unit unit = skillTarget.GetComponent<Unit>();
        //ExecuteSkill(unit,usingSkill);
    }
    public void ExecuteSkill(Unit skillTarget,HeroSkill skill)
    {
        switch (skill)
        {
            case (HeroSkill.finishMove):
                {
                    StartCoroutine(knight.FinishMove(skillTarget));
                    StartCoroutine(PlaySkillOnTr(skillTarget.transform));
                    StartCoroutine(selectedHero.SkillCoolCor(3, knight.SkillCools[3]));
                    break;
                }
        }
        SkillEnd();
    }

    private void OnSkillCancel(InputAction.CallbackContext obj)
    {
        SkillEnd();
    }

    public void SkillEnd(bool isOutRange=false)
    {
        Destroy(skillRangeObj);
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Disable();
        GameMgr.Instance.inputActions.Command.Select.Enable();
        UIMgr.Instance.ChangeCursor(CursorType.Default);
        if (isOutRange)
        {
            return;
        }
        usingSkill = null;
        skillMode = SkillType.OnHero;
        isUsingSkill = false;
        isChasingForSkill = false;
    }
    
    public void UseSkill(int index,Transform tr)
    {
        if (!selectedHero.SkillCanUse[index])
            return;
        switch(selectedHero.Data.heroClass)
        {
            case(HeroClass.Knight):
                usingSkill = knigjtSkills[index];
                break;
            case (HeroClass.Mage):
                usingSkill = mageSkills[index];
                break;
        }
        StartCoroutine(EnumeratorTimer(usingSkill.SkillCor(tr,selectedHero), usingSkill.data.duration));
        StartCoroutine(PlaySkillOnTr(tr));
        StartCoroutine(selectedHero.SkillCoolCor(index, usingSkill.data.coolTime));
    }
    private void OnSkill1(InputAction.CallbackContext obj)
    {
        if (!selectedHero.SkillCanUse[0])
            return;
        switch(selectedHero.Data.heroClass)
        {
            case (HeroClass.Knight):
                StartCoroutine(PlaySkillOnTr(selectedHero.transform));
                StartCoroutine(EnumeratorTimer(knight.shieldAuraCor, knight.SkillDurations[0]));
                break;
            case (HeroClass.Mage):
                break;
        }
        
    }

    private void OnSkill2(InputAction.CallbackContext obj)
    {
        if (!selectedHero.SkillCanUse[1])
            return;
        
        switch (selectedHero.Data.heroClass)
        {
            case (HeroClass.Knight):
                StartCoroutine(PlaySkillOnTr( selectedHero.transform));
                knight.Provoke(knight.SkillDurations[1]);
                StartCoroutine(selectedHero.SkillCoolCor(1, knight.SkillCools[1]));
                break;
            case (HeroClass.Mage):
                break;
        }
    }

    private void OnSkill3(InputAction.CallbackContext obj)
    {
        if (!selectedHero.SkillCanUse[2])
            return;
        switch (selectedHero.Data.heroClass)
        {
            case (HeroClass.Knight):
                StartCoroutine(PlaySkillOnTr( selectedHero.transform));
                StartCoroutine(EnumeratorTimer(knight.frenzyCor, knight.SkillDurations[2]));
                StartCoroutine(selectedHero.SkillCoolCor(2, knight.SkillCools[2]));
                break;
            case (HeroClass.Mage):
                break;
        }
       
    }

    private void OnSkill4(InputAction.CallbackContext obj)
    {
        if (!selectedHero.SkillCanUse[3])
            return;
        switch (selectedHero.Data.heroClass)
        {
            case (HeroClass.Knight):
                UseClickingSkill(SkillType.Targrt, knight.SkillRadius[3]);
                //usingSkill = HeroSkill.finishMove;
                break;
            case (HeroClass.Mage):
                break;
        }
    }
    public IEnumerator EnumeratorTimer(IEnumerator enumerator, float sec)
    {
        StartCoroutine(enumerator);
        yield return new WaitForSeconds(sec);
        if (enumerator == knight.shieldAuraCor)
        {
            knight.SetShiedPlus(knight.SkillRadius[0] + 2, 0);
        }
        StopCoroutine(enumerator);
    }

    public IEnumerator PlaySkillOnTr(Transform tr)
    {
        GameObject obj = Instantiate(usingSkill.SkillPrefab,tr);
        //skillNow=knight.FinishMove(
        yield return new WaitForSeconds(usingSkill.data.duration);
        Destroy(obj);
    }
    public void ShowSkillRange(float skillRange)
    {
        skillRangeObj=Instantiate(skillRangePrefab, GameMgr.Instance.commandMgr.SelectedHero.transform);
        skillRangeObj.transform.localScale=new Vector3(skillRange*2,0,skillRange * 2);
        isUsingSkill = true;
    }
    public void UseClickingSkill(SkillType mode,float range,float radius=0)
    {
        ShowSkillRange(range);
        skillRange = range;
        GameMgr.Instance.inputActions.Command.Select.Disable();
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Enable();
        if (mode == SkillType.Targrt)
        {
            skillMode=SkillType.Targrt;
            UIMgr.Instance.ChangeCursor(CursorType.findTarget);
        }
        else
        {
            skillMode = SkillType.NonTarget;

        }
    }
   
}
