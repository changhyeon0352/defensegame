
using UnityEngine;
using UnityEngine.InputSystem;
public enum SkillType { OnHero, UnitTarget, AreaTarget,directional }
public enum Buff { provoked,sleep,shield}
public class SkillController : MonoBehaviour
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

    private void OnEnable()
    {
        GameMgr.Instance.inputActions.Command.SkillButton1.performed += OnSkill1;
        GameMgr.Instance.inputActions.Command.SkillButton2.performed += OnSkill2;
        GameMgr.Instance.inputActions.Command.SkillButton3.performed += OnSkill3;
        GameMgr.Instance.inputActions.Command.SkillButton4.performed += OnSkill4;
        GameMgr.Instance.inputActions.Command.HeroSkillClick.performed += OnSkillClick;
        GameMgr.Instance.inputActions.Command.SkillCancel.performed += OnSkillCancel;
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Disable();
        selectedHero = GameMgr.Instance.defenseMgr.SelectedHero;
    }

    //스킬 인디케이터 표시
    private void Update()
    {
        if(indicator!=null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
            {   
                if(SkillType==SkillType.AreaTarget)
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
    private void OnSkillCancel(InputAction.CallbackContext _)
    {
        if(isUsingSkill)
            SkillEnd();
    }
    private void OnSkill1(InputAction.CallbackContext _) { StartUsingSkill(0); }
    private void OnSkill2(InputAction.CallbackContext _) { StartUsingSkill(1); }
    private void OnSkill3(InputAction.CallbackContext _) { StartUsingSkill(2); }
    private void OnSkill4(InputAction.CallbackContext _) { StartUsingSkill(3); }

    //skillTarget을 정해서 UseClinkingSkill(skillTarget) 하는 역할(StartClickingSkill()함수가 실행된 다음 인풋이 활성화)
    private void OnSkillClick(InputAction.CallbackContext _)
    {
        if (SkillType == SkillType.UnitTarget || SkillType == SkillType.AreaTarget)//타게팅,혹은 논타겟범위
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, usingSkill.TargetLayer))
            {
                Vector3 heroPos = selectedHero.transform.position;
                float distance = Vector2.Distance(new Vector2(heroPos.x, heroPos.z),
                    new Vector2(hit.point.x, hit.point.z));
                //skillTarget 지정
                if (usingSkill.data.skillType == SkillType.UnitTarget)
                {
                    Unit skillTargetUnit = hit.transform.GetComponent<Unit>();
                    skillTarget = skillTargetUnit.transform;
                }
                else if (usingSkill.data.skillType == SkillType.AreaTarget)
                {
                    skillTarget = new GameObject().transform;
                    skillTarget.position = hit.point + Vector3.up * 0.3f;
                    skillTarget.forward = indicator.transform.forward;
                }
                if (distance > usingSkill.data.range)//사거리 밖일때
                {
                    //Hero.cs ChaseUpdate에서 체크해서 사거리 이내로 오면UseClinkingSkill(skillTarget)
                    isChasingForSkill = true; 
                    SkillEnd(true);
                    selectedHero.ChaseTarget(skillTarget);
                    return;
                }
                //스킬 쓰는 방향으로 바로보고 스킬 사용
                selectedHero.transform.forward = new Vector3(skillTarget.position.x - selectedHero.transform.position.x
                    , 0, skillTarget.position.z - selectedHero.transform.position.z);
                UseClinkingSkill(skillTarget);
            }
            else
            {
                return;//마우스 위치에 skill의 targetlayer가 없을때 스킵
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

    // 스킬 취소 함수
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

    //스킬 버튼 누를시 실행되는 스킬 시작 함수
    public void StartUsingSkill(int index)
    {
        //스킬 사용 도중에 눌렀으면 기존 스킬 취소
        if (skillRangeObj != null || indicator != null)
        {
            SkillEnd();
        }
        this.index = index;
        if (!selectedHero.SkillCanUse[index]) //스킬쿨 체크
            return;
        switch (selectedHero.HeroData.heroClass) //컨트롤 중 영웅캐릭터 클래스 체크 
        {
            case (HeroClass.Knight):
                usingSkill = knigjtSkills[index];
                break;
            case (HeroClass.Mage):
                usingSkill = mageSkills[index];
                break;
        }
        usingSkill.InitSetting();
        //OnHero타입은 스킬타겟을 정하고 스킬을 바로 실행 그외의 스킬은 StartClickingSill()로 클릭을 통해 정함
        if (skillTarget == null)
        {
            if (usingSkill.data.skillType != SkillType.OnHero)
            {
                StartClickingSkill();
                return;
            }
            skillTarget = selectedHero.transform;
        }
        UseSkill(index);
    }

    //스킬이 실제 발동될때 실행되는 함수
    private void UseSkill(int index)
    {
        StartCoroutine(usingSkill.SkillCor(skillTarget, selectedHero));              //스킬사용
        StartCoroutine(selectedHero.SkillCoolCor(index, usingSkill.data.coolTime)); //스킬쿨타임 적용
        if (selectedHero.HeroData.heroClass == HeroClass.Mage)
            selectedHero.SkillAnimation(UsingSKill == mageSkills[3], UsingSKill.data.duration);
        SkillEnd();
    }

    //스킬 범위 보여주는 함수(유닛타겟,영역타겟)
    public void ShowSkillRange(float skillRange)
    {
        skillRangeObj=Instantiate(skillRangePrefab);
        skillRangeObj.transform.localScale=new Vector3(skillRange,1,skillRange);
        skillRangeObj.transform.position = selectedHero.transform.position;
        skillRangeObj.transform.parent = selectedHero.transform;
    }

    // 스킬 인디케이터 보여주는 함수
    public void ShowIndicator()
    {
        indicator = Instantiate(usingSkill.Indicator);
        if(SkillType==SkillType.AreaTarget)
        {
            indicator.transform.localScale = new Vector3(usingSkill.data.nonTargetRange, 1, usingSkill.data.nonTargetRange);
        }
        else if(SkillType==SkillType.directional)
        {
            indicator.transform.position = selectedHero.transform.position;
            indicator.transform.parent = selectedHero.transform;
        }
    }

    //버튼을 누르고 클릭으로 목표를 지정하는 스킬일 경우 StartUsingSkill()다음으로 실행되는 함수
    public void StartClickingSkill()
    {
        if(SkillType==SkillType.AreaTarget|| SkillType == SkillType.UnitTarget)
            ShowSkillRange(usingSkill.data.range);
        isUsingSkill = true;
        GameMgr.Instance.inputActions.Command.Select.Disable();
        GameMgr.Instance.inputActions.Command.HeroSkillClick.Enable();
        if (SkillType == SkillType.UnitTarget)
        {
            UIMgr.Instance.ChangeCursor(CursorType.findTarget);
        }
        else
        {
            ShowIndicator(); //스킬인디케이터온
        }
    }
    //스킬타겟 지정하고 다시 UseSkill 
    public void UseClinkingSkill(Transform skillTarget)
    {
        this.skillTarget = skillTarget;
        UseSkill(index);
    }
    
    
}
