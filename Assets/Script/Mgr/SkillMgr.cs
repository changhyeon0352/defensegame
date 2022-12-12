using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
public enum HeroSkill { none,shieldAura, provoke, vortex, finishMove }
public enum SkillType { OnHero, Target, NonTarget,directional }
public enum Buff { provoked,sleep,shield}
public class SkillMgr : MonoBehaviour
{
    
    public GameObject[] Buffs;
    public GameObject skillRangePrefab;
    [SerializeField]
    Skill[] knigjtSkills;
    [SerializeField]
    Skill[] mageSkills;
    Skill usingSkill;
    
    public Skill UsingSKill { get=> usingSkill; } 
    public SkillType SkillType { 
        get 
        {
            if (usingSkill == null)
                return SkillType.OnHero;
            else
                return usingSkill.data.skillType; 
        } 
    }
    bool isUsingSkill = false;
    public bool IsUsingSkill { get { return isUsingSkill; } }
    bool isChasingForSkill=false;
    public bool IsChasingForSkill { get { return isChasingForSkill; } }
    public Hero selectedHero;
    public float SkillRange { get { return usingSkill.data.range; } }
    GameObject indicator;
    GameObject skillRangeObj;
    public GameObject SkillRangeObj { get => skillRangeObj; }
    Transform skillTarget;
    int index = -1;
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


    private void Update()
    {
        if(indicator!=null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
            {   
                if(SkillType==SkillType.NonTarget)
                {
                    indicator.transform.position = hit.point;
                    indicator.transform.forward = hit.point - selectedHero.transform.position;
                }
                else if (SkillType == SkillType.directional)
                {
                    indicator.transform.right=-(hit.point - selectedHero.transform.position);
                }
            }
        }
    }
    private void OnSkillCancel(InputAction.CallbackContext obj)
    {
        SkillEnd();
    }

    public void SkillEnd(bool isOutRange=false)
    {
        if(skillRangeObj!=null)
            Destroy(skillRangeObj);
        if(indicator!=null)
            Destroy(indicator);
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Disable();
        GameMgr.Instance.inputActions.Command.Select.Enable();
        UIMgr.Instance.ChangeCursor(CursorType.Default);
        if (isOutRange)
        {
            return;
        }
        usingSkill = null;
        isUsingSkill = false;
        isChasingForSkill = false;
        skillTarget = null;
        index = -1;
    }
    
    public void UseSkill(int index)
    {
        this.index = index;
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
        usingSkill.InitSetting();
        if(skillTarget == null)
        {
            if (usingSkill.data.skillType != SkillType.OnHero)
            {
                StartClickingSkill();
                return;
            }
                
            skillTarget = selectedHero.transform;
        }
        StartCoroutine(usingSkill.SkillCor(skillTarget,selectedHero));
        StartCoroutine(selectedHero.SkillCoolCor(index, usingSkill.data.coolTime));
        SkillEnd();
    }
    private void OnSkill1(InputAction.CallbackContext obj) {UseSkill(0);}
    private void OnSkill2(InputAction.CallbackContext obj) {UseSkill(1);}
    private void OnSkill3(InputAction.CallbackContext obj) {UseSkill(2);}
    private void OnSkill4(InputAction.CallbackContext obj) {UseSkill(3);}
    
    public void ShowSkillRange(float skillRange)
    {
        skillRangeObj=Instantiate(skillRangePrefab, GameMgr.Instance.commandMgr.SelectedHero.transform);
        skillRangeObj.transform.localScale=new Vector3(skillRange,0,skillRange);
    }
    public void ShowIndicator()
    {
        indicator = Instantiate(usingSkill.Indicator);
        if(SkillType==SkillType.NonTarget)
        {
            indicator.transform.localScale = new Vector3(usingSkill.data.nonTargetRange, 1, usingSkill.data.nonTargetRange);
        }
        else if(SkillType==SkillType.directional)
        {
            indicator.transform.position = selectedHero.transform.position;
            indicator.transform.parent = selectedHero.transform;
        }
    }
    public void StartClickingSkill()
    {
        if(SkillType==SkillType.NonTarget|| SkillType == SkillType.Target)
            ShowSkillRange(usingSkill.data.range);
        isUsingSkill = true;
        GameMgr.Instance.inputActions.Command.Select.Disable();
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Enable();
        if (SkillType == SkillType.Target)
        {
            UIMgr.Instance.ChangeCursor(CursorType.findTarget);
        }
        else
        {
            ShowIndicator(); //스킬인디케이터온
        }
    }
    private void HeroSkillClick(InputAction.CallbackContext obj)//skillTarget을 정해서 UseClinkingSkill(skillTarget) 하는 역할
    {
        if (SkillType == SkillType.Target|| SkillType == SkillType.NonTarget)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f,usingSkill.TargetLayer))
            {
                Vector3 heroPos = selectedHero.transform.position;
                float distance = Vector2.Distance(new Vector2(heroPos.x, heroPos.z), new Vector2(hit.point.x, hit.point.z));
                Debug.Log(distance);
                if(usingSkill.data.skillType==SkillType.Target)
                {
                    Unit skillTargetUnit = hit.transform.GetComponent<Unit>();
                    if (skillTargetUnit == null)
                    {
                        return;//무반응하기
                    }
                    skillTarget = skillTargetUnit.transform;
                }
                else if(usingSkill.data.skillType == SkillType.NonTarget)
                {
                    skillTarget = new GameObject().transform;
                    skillTarget.position = hit.point+Vector3.up*0.3f;
                    skillTarget.forward = indicator.transform.forward;
                    //skillTarget.transform.up = Vector3.up;
                }
                if (distance > usingSkill.data.range)//사거리 밖일때
                {
                    isChasingForSkill = true;
                    SkillEnd(true);
                    selectedHero.ChaseTarget(skillTarget);
                    return;
                }
                UseClinkingSkill(skillTarget);
            }
            else
            {
                return;//무반응하기
            }
        }
        else if (SkillType == SkillType.directional)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, usingSkill.TargetLayer))
            {
                selectedHero.transform.LookAt(hit.point);
                UseClinkingSkill(selectedHero.transform);
            }
        }
    }
    public void UseClinkingSkill(Transform skillTarget)
    {
        this.skillTarget = skillTarget;
        UseSkill(index);
    }
    
    
}
