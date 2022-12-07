using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillController : MonoBehaviour
{
    Skill skill;
    HeroSkill usingSkill;
    Hero hero;
    [SerializeField] GameObject skillRangePrefab;
    GameObject skillRangeObj;
    bool isShowRange=false;
    SkillType skillType;
    float skillRange;
    //===================================================================================

    

    //=====================================================================================
    public IEnumerator PlaySkillOnTr(Skill skill, float sec, Transform tr)
    {
        GameObject obj = Instantiate(skill.SkillPrefab, tr);
        yield return new WaitForSeconds(sec);
        Destroy(obj);
    }
    public void ShowSkillRange(float skillRange)
    {
        skillRangeObj = Instantiate(skillRangePrefab, GameMgr.Instance.commandMgr.SelectedHero.transform);
        skillRangeObj.transform.localScale = new Vector3(skillRange * 2, 0, skillRange * 2);
        isShowRange = true;
    }
    public void UseClickingSkill(SkillType mode, float range, float radius = 0)
    {
        ShowSkillRange(range);
        skillRange = range;
        GameMgr.Instance.inputActions.Command.Select.Disable();
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Enable();
        if (mode == SkillType.Targrt)
        {
            skillType = SkillType.Targrt;
            UIMgr.Instance.ChangeCursor(CursorType.findTarget);
        }
        else
        {
            skillType = SkillType.NonTarget;

        }
    }
    private void HeroSkillClick(InputAction.CallbackContext obj)
    {
        if (skillType == SkillType.Targrt)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Monster")))
            {
                Vector3 heroPos = hero.transform.position;
                float distance = Vector2.Distance(new Vector2(heroPos.x, heroPos.z), new Vector2(hit.point.x, hit.point.z));
                Unit skillTarget = hit.transform.GetComponent<Unit>();
                Debug.Log(distance);
                if (skillTarget == null)
                {
                    return;//무반응하기
                }
                if (distance > skillRange)//사거리 밖일때
                {
                    //isChasingForSkill = true;
                    //SkillEnd(true);
                    //selectedHero.ChaseTarget(skillTarget.transform);
                    //return;
                }
                ExecuteSkill(skillTarget, usingSkill);
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
        ExecuteSkill(unit, usingSkill);
    }
    public void ExecuteSkill(Unit skillTarget, HeroSkill skill)
    {
        //StartCoroutine(knight.FinishMove(skillTarget));
        //StartCoroutine(PlaySkillOnTr(HeroSkill.finishMove, 2, skillTarget.transform));
        //StartCoroutine(selectedHero.SkillCoolCor(3, knight.SkillCools[3]));
        //break;
        SkillEnd();
    }
    private void OnSkillCancel(InputAction.CallbackContext obj)
    {
        SkillEnd();
    }

    public void SkillEnd(bool isOutRange = false)
    {
        Destroy(skillRangeObj);
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Disable();
        GameMgr.Instance.inputActions.Command.Select.Enable();
        UIMgr.Instance.ChangeCursor(CursorType.Default);
        if (isOutRange)
        {
            return;
        }
        usingSkill = HeroSkill.none;
        skillType = SkillType.OnHero;
        //isUsingSkill = false;
        //isChasingForSkill = false;
    }
}
