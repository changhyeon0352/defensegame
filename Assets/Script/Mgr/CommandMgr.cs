
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;




public class CommandMgr : MonoBehaviour
{
    public GameObject[] skillIndicatorPrefabs;
    [SerializeField] GameObject moveToPrefabs;
    [SerializeField] GameObject attackToPrefabs;
    List<UnitGroup> seletedGroupList = new List<UnitGroup>();
    Hero selectedHero;
    List<Hero> fightingHeroList= new List<Hero>();
    PlayerInput inputActions;
    private List<Transform> skillIndicatorTrs = new List<Transform>();
    List<Transform> skillTargets = new List<Transform>();
    BasicSkills usingSkill = BasicSkills.None;
    LayerMask monsterOrGround;


    public List<UnitGroup> SelectedGroupList
    {
        get => seletedGroupList;
    }
    public BasicSkills UsingSkill { get => usingSkill;}
    public Hero SelectedHero { get => selectedHero;
        set { selectedHero = value;
            GameMgr.Instance.skillMgr.selectedHero = value; 
            AllCheckSelected(); 
        } 
    }
    

    private void Awake()
    {
        inputActions = GameMgr.Instance.inputActions;
        monsterOrGround = LayerMask.GetMask("Monster") | LayerMask.GetMask("Ground");
    }
    private void OnEnable()
    {
        inputActions.Command.Enable();
        inputActions.Command.Select.performed += OnSelect;
        inputActions.Command.skillClick.performed += OnSkillClick;
        inputActions.Command.MoveorSetTarget.performed += OnMoveOrSetTarget;
        inputActions.Command.AttackMove.performed += OnAttackMove;
        inputActions.Command.ChangeHero.performed += OnChangeHero;
        inputActions.Command.skillClick.Disable();
        inputActions.Camera.ShowHero.Enable();
        foreach (var herodata in DataMgr.Instance.FightingHeroDataList)
        {
            fightingHeroList.Add(Hero.FindHero(herodata));
        }
        SelectHero(0);

    }

    private void OnChangeHero(InputAction.CallbackContext obj)//숫자 1~4 눌렀을때 히어로가 선택됨
    {
        Keyboard kboard = Keyboard.current;
        if (kboard.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl k in kboard.allKeys)
            {
                if (k.wasPressedThisFrame)
                {
                    int a=(int)k.keyCode - 41;
                    if(a<DataMgr.Instance.FightingHeroDataList.Count)
                    {
                        SelectHero(a);
                    }
                }
            }
        }
    }

    private void SelectHero(int a)
    {
        ClearSelectedGroups();
        SelectedHero = fightingHeroList[a];
        UIMgr.Instance.skillbarUI.ChangeHeroSkill(selectedHero);
    }

    private void OnDisable()
    {
        inputActions.Command.Disable();
    }
    /// 스킬인디케이터가 있으면 마우스 위치에 표시
    private void Update()
    {
        if (skillIndicatorTrs.Count > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, LayerMask.GetMask("Ground")))
            {
                for (int i = 0; i < skillIndicatorTrs.Count; i++)
                {
                    skillIndicatorTrs[i].position = hit.point;
                    if(SelectedGroupList.Count>0)
                    {
                        skillIndicatorTrs[i].forward = hit.point - seletedGroupList[i].spotsTr.position;
                    }
                }
            }
        }
    }
    //인풋 연결 함수or 스킬================================================================================
    //영웅 어택땅(a키)
    private void OnAttackMove(InputAction.CallbackContext _)
    {
        ClearSelectedGroups();
        
        if (IsHeroSelected())
        {
            GameMgr.Instance.skillMgr.ShowSkillRange(SelectedHero.unitData.AttackRange);
            inputActions.Command.Select.Disable();
            inputActions.Command.skillClick.Enable();
            usingSkill = BasicSkills.AttackMove;
            UIMgr.Instance.ChangeCursor(CursorType.targeting);
        }
    }

    /// 우클릭시 실행됨 땅찍으면 그쪽에 이펙트 보여주고 무브 적이면 추격
    private void OnMoveOrSetTarget(InputAction.CallbackContext _)
    {
        ClearSelectedGroups();
        if (GameMgr.Instance.skillMgr.IsUsingSkill)
        {
            GameMgr.Instance.skillMgr.SkillEnd();
            return;
        }
        if (IsHeroSelected())
        {
            selectedHero.isattackMove = false;
            MoveOrSetTarget(moveToPrefabs);
        }
    }

    private void MoveOrSetTarget(GameObject effectPrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, monsterOrGround))
        {
            //적인지 땅인지
            if ((int)Mathf.Pow(2, hit.transform.gameObject.layer) == LayerMask.GetMask("Ground"))//땅클릭
            {
                ParticleSystem moveEffect = gameObject.GetComponentInChildren<ParticleSystem>();
                if (moveEffect != null)
                {
                    Destroy(moveEffect.gameObject);
                }
                GameObject effectobj = Instantiate(effectPrefab, hit.point, Quaternion.identity);
                effectobj.transform.parent = transform;
                StartCoroutine(WaitDestroy(effectobj, 0.5f));
                selectedHero.MoveSpots(hit.point);
            }
            else//적클릭
            {
                selectedHero.isProvoked = true;
                selectedHero.ChangeState(UnitState.Chase);
                selectedHero.SetChaseTarget(hit.transform);
            }
        }
    }

    // 병사, 영웅 쉬프트,그냥 클릭
    private void OnSelect(InputAction.CallbackContext obj)
    {
        if (Utils.IsClickOnUI())//UI 클릭 하는지
        {
            return;
        }
        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.transform.GetComponent<Unit>();
            if (unit!=null)
            {
                UIMgr.Instance.unitStatUI.gameObject.SetActive(true);
                UIMgr.Instance.unitStatUI.RefreshUnitStatWindow(unit);
            }
            if ((int)MathF.Pow(2, hit.transform.gameObject.layer) == LayerMask.GetMask("Ally")) //hit한게 ally layer라면
            {
                //영웅이 클릭됐는지
                if (hit.transform.GetComponent<Hero>() != null)
                {
                    ClearSelectedGroups();
                    SelectedHero= hit.transform.GetComponent<Hero>();
                }
                else //병사들이 클릭 됐으면
                {
                    UnitGroup unitGroup = hit.transform.parent.parent.GetComponent<UnitGroup>();
                    if (unitGroup != null)
                    {
                        if (Input.GetKey(KeyCode.LeftShift) && seletedGroupList.Count > 0 && (SelectedGroupList[0].unitType != UnitType.hero))
                        {
                            if (seletedGroupList.Contains(unitGroup))
                            {
                                seletedGroupList.Remove(unitGroup);
                            }
                            else
                            {
                                seletedGroupList.Add(unitGroup);
                            }
                        }
                        else
                        {
                            ClearSelectedGroups();
                            seletedGroupList.Add(unitGroup);
                        }
                    }
                }
            }
            else //아군이 아니면 ex)땅,적
            {
                Debug.Log(hit.transform.name);
                if (!Input.GetKey(KeyCode.LeftShift))
                    ClearSelectedGroups();
            }
        }
        AllCheckSelected();
        //유닛 그룹에 스킬 교집합 체크 스킬넘버배열을 넘겨주자
        //스킬 사용가능여부 비트연산자 이용할것
        BasicSkills groupsSkills = ~BasicSkills.None; //1111 1111 로 초기화
        foreach (UnitGroup selectGroup in seletedGroupList)
        {
            groupsSkills &= selectGroup.GroupSkill; //유닛그룹들의 사용가능 스킬 교집합만 남긴다.
        }
        if (seletedGroupList.Count > 0)
        {
            UIMgr.Instance.SetButtonAvailable(groupsSkills); //버튼에 사용사능 스킬만 활성화
        }

    }
    
    //스킬실행중 셀렉트클릭잠금,스킬인디케이터 생성 스팟복사해서 인디케이터 안에 넣음 스킬타겟즈에 스팟즈를 넣음
    public void SelectShotSpot()
    {
        usingSkill = BasicSkills.ShootSpot;
        inputActions.Command.Select.Disable();
        inputActions.Command.skillClick.Enable();
        List<Transform> spots = new();
        for (int i = 0; i < seletedGroupList.Count; i++)
        {
            skillIndicatorTrs.Add(Instantiate(skillIndicatorPrefabs[0]).transform);
            skillIndicatorTrs[i].localScale = new Vector3(seletedGroupList[i].rowColumn.y, 1,
                seletedGroupList[i].rowColumn.x) * GameMgr.Instance.settingMgr.UnitOffset;
            spots.Add(Instantiate(seletedGroupList[i].spotsTr));

            spots[i].parent = skillIndicatorTrs[i];
            spots[i].position = Vector3.zero;
            spots[i].forward = skillIndicatorTrs[i].forward;
        }
        //스팟즈를 스킬타켓즈에 순서대로 넣기

        for (int i = 0; i < spots.Count; i++)
        {
            for (int a = 0; a < spots[i].childCount; a++)
            {
                skillTargets.Add(spots[i].GetChild(a));
            }
        }
    }
    
    //지금 버튼을 누른 기술을 각각 유닛에게 해라고 명령내리고 인풋시스템 클릭을 되돌림 스킬인디케이터 제거
    private void OnSkillClick(InputAction.CallbackContext obj)
    {

        UnitCommand((usingSkill));
        switch (UsingSkill)
        {
            case BasicSkills.AttackMove:
                UIMgr.Instance.ChangeCursor(CursorType.Default);
                SelectedHero.isattackMove = true;
                MoveOrSetTarget(attackToPrefabs);
                GameMgr.Instance.skillMgr.SkillEnd();
                break;
            case BasicSkills.ShootSpot:
                foreach (var indicator in skillIndicatorTrs)
                {
                    indicator.GetChild(1).parent = null;
                    Destroy(indicator.gameObject);
                }
                break;
        }

        
        inputActions.Command.Select.Enable();
        inputActions.Command.skillClick.Disable();
        usingSkill = BasicSkills.None;
        skillIndicatorTrs.Clear();
        skillTargets.Clear();
    }

    //잡====================================================================================================
    public void UnitCommand(int command)
    {
        UnitCommand((BasicSkills)command);
    }
    public void UnitCommand(BasicSkills command)
    {
        if(seletedGroupList.Count>0)
        {
            Debug.Log("스킬사용");
            foreach (UnitGroup unitGroup in seletedGroupList)
            {
                AllyUnit[] units;
                units = unitGroup.GetComponentsInChildren<AllyUnit>();

                //foreach (var unit in units)
                for (int i = 0; i < units.Length; i++)
                {
                    switch (command)
                    {
                        case (BasicSkills.MoveToSpot):
                            units[i].ChangeState(UnitState.Move);
                            units[i].isattackMove = false;
                            break;
                        case (BasicSkills.Charge):
                            units[i].ChargeToEnemy();
                            break;
                        case (BasicSkills.ShootSpot):
                            if (!inputActions.Command.skillClick.enabled)
                            {
                                SelectShotSpot();
                                return;
                            }
                            else
                            {
                                units[i].GetComponent<AllyRange>().ShotSpotMode(skillTargets[i]);
                            }
                            break;
                        case (BasicSkills.ShootEnemy):
                            units[i].GetComponent<AllyRange>().ShotEnemyMode();
                            break;
                    }
                }
            }
        }
        else
        {
            switch (command)
            {
                case(BasicSkills.AttackMove):
                    break;

                default:
                    break;
            }

        }
    }
    public bool IsHeroSelected()
    {
        return selectedHero != null;
    }
    IEnumerator WaitDestroy(GameObject obj, float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(obj);
    }
    public void ClearSelectedGroups()
    {
        seletedGroupList.Clear();
        UIMgr.Instance.ClearSkillButton();
        AllCheckSelected();
    }
    //유닛에 셀렉트될때 이펙트를 킬지 끌지
    public void AllCheckSelected()
    {
        UnitGroup[] unitGroups = FindObjectsOfType<UnitGroup>();
        foreach (UnitGroup unitGroup in unitGroups)
        {
            unitGroup.CheckSelected();
        }
    }



}
