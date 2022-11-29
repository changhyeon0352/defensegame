using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
public enum HeroSkill { none,shieldAura, provoke, frenzy, finishMove }
public enum SkillMode { OnHero, Targrt, NonTarget }
public enum Buff { provoked,sleep,shield}
public class SkillMgr : MonoBehaviour
{
    public GameObject[] Buffs;
    public GameObject skillRangePrefab;
    HeroSkill usingSkill;
    SkillMode skillMode;
    public SkillMode SkillMode { get { return skillMode; } }
    Unit skillTarget;
    public Hero selectedHero;
    float skillRange;
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

    }

    private void HeroSkillClick(InputAction.CallbackContext obj)
    {
        if(skillMode==SkillMode.Targrt)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Monster")))
            {
                Vector3 heroPos=selectedHero.transform.position;
                float distance = Vector2.Distance(new Vector2(heroPos.x, heroPos.z), new Vector2(hit.point.x, hit.point.z));
                Debug.Log(distance);
                if (distance<skillRange)
                {
                    skillTarget = hit.transform.GetComponent<Unit>();
                    Debug.Log($"{skillTarget.name}이 스킬 표적이 됨");
                }
                
            }
            else
            {
                return;
            }
        }
        switch(selectedHero.data.heroClass)
        {
            case (HeroClass.Knight):
                {
                    if(skillTarget != null)
                    {
                        StartCoroutine(knight.FinishMove(skillTarget));
                        StartCoroutine(PlaySkillOnHero(HeroSkill.finishMove, 2, false));
                    }
                    break;
                }
        }
        SkillCanEnd(false);
    }

    private void OnSkillCancel(InputAction.CallbackContext obj)
    {
        SkillCanEnd(true);
    }

    private void SkillCanEnd(bool isCancel)
    {
        if (!isCancel)
        {
            //쿨타임
        }
        Destroy(skillRangeObj);
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Disable();
        GameMgr.Instance.inputActions.Command.Select.Enable();
        usingSkill = HeroSkill.none;
        skillMode = SkillMode.OnHero;
        UIMgr.Instance.ChangeCursor(CursorType.Default);
        skillTarget = null;
        
    }

    private void OnSkill1(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(HeroSkill.shieldAura, knight.SkillDurations[0]));
        StartCoroutine(knight.EnumeratorTimer(knight.shieldAuraCor, knight.SkillDurations[0]));
    }

    private void OnSkill2(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(HeroSkill.provoke, knight.SkillDurations[1]));
        knight.Provoke(knight.SkillDurations[1]);
    }

    private void OnSkill3(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(HeroSkill.frenzy, knight.SkillDurations[2]));
        StartCoroutine(knight.EnumeratorTimer(knight.frenzyCor, knight.SkillDurations[2]));
    }

    private void OnSkill4(InputAction.CallbackContext obj)
    {
        UseClickingSkill(SkillMode.Targrt, knight.SkillRadius[3]);
        //StartCoroutine(PlaySkillOnHero(KnightSkill.finishMove, 5));
    }

  
    public IEnumerator PlaySkillOnHero(HeroSkill skill, float sec,bool isOnHero=true)
    {
        Transform tr = selectedHero.transform;
        if(!isOnHero)
        {
            tr = skillTarget.transform;
        }
        GameObject obj = Instantiate(knight.skillEffects[(int)skill-1],tr);
        //skillNow=knight.FinishMove(
        yield return new WaitForSeconds(sec);
        Destroy(obj);
    }
    public void ShowSkillRange(float skillRange)
    {
        skillRangeObj=Instantiate(skillRangePrefab, GameMgr.Instance.commandMgr.SelectedHero.transform);
        skillRangeObj.transform.localScale=new Vector3(skillRange*2,0,skillRange * 2);
    }
    public void UseClickingSkill(SkillMode mode,float range)
    {
        ShowSkillRange(range);
        skillRange = range;
        GameMgr.Instance.inputActions.Command.Select.Disable();
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Enable();
        if (mode == SkillMode.Targrt)
        {
            skillMode=SkillMode.Targrt;
            UIMgr.Instance.ChangeCursor(CursorType.findTarget);
        }
        else
        {
            skillMode = SkillMode.NonTarget;
        }
    }
   
}
