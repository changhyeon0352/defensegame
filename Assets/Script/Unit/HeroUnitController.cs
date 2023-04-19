using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class HeroUnitController : Controller
{
    private HeroUnit selectedHero;
    private HeroUnitView view;
    private List<HeroUnit> allHeroUnits=new List<HeroUnit>();
    private SkillController skillController;

    [SerializeField] GameObject moveToEffect;
    [SerializeField] GameObject attackToEffect;

    LayerMask monsterOrGround;

    public Transform SelectedHeroTr { get => selectedHero.transform; }

    override protected  void Awake()
    {
        base.Awake();
        monsterOrGround = LayerMask.GetMask("Monster") | LayerMask.GetMask("Ground");
        view = GetComponent<HeroUnitView>();
        skillController=FindObjectOfType<SkillController>();
        
    }
    private void OnEnable()
    {
        skillController.cancelAction += CancelAttackMove;
        allHeroUnits.AddRange(FindObjectsOfType<HeroUnit>());
        PushSkillControllerInHero();
        inputActions.Command.ChangeHero.performed += OnChangeHero;
        inputActions.Command.MoveorSetTarget.performed += OnMoveOrSetTarget;
        inputActions.Command.AttackMove.performed += OnAttackMove;
        
    }
    private void CancelAttackMove()
    {
        inputActions.Command.skillClick.performed -= OnAttackClick;
    }
    private void PushSkillControllerInHero()
    {
        foreach(HeroUnit unit in allHeroUnits)
        {
            unit.SetSkillController(skillController);
        }
    }
    public void OnAttackClick(InputAction.CallbackContext obj)
    {
            inputActions.Command.Select.Enable();
            inputActions.Command.skillClick.Disable();
            UIMgr.Instance.ChangeCursor(CursorType.Default);
            inputActions.Command.skillClick.performed -= OnAttackClick;
            skillController.SkillEnd();
            MoveOrSetTarget(true);
            
    }

    private void OnAttackMove(InputAction.CallbackContext obj)
    {
        skillController.ShowSkillRange(selectedHero.UnitData.AttackRange);
        inputActions.Command.Select.Disable();
        inputActions.Command.skillClick.Enable();
        inputActions.Command.skillClick.performed += OnAttackClick;
        UIMgr.Instance.ChangeCursor(CursorType.findTarget);
    }


    private void Start()
    {
        SelectHero(0);
    }
    private void OnChangeHero(InputAction.CallbackContext obj)
    {
        Keyboard kboard = Keyboard.current;
        if (kboard.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl k in kboard.allKeys)
            {
                if (k.wasPressedThisFrame)
                {
                    int num = (int)k.keyCode - 41;
                    if (num < allHeroUnits.Count)
                    {
                        SelectHero(num);
                    }
                }
            }
        }
    }
    private void OnMoveOrSetTarget(InputAction.CallbackContext _)
    {
        if (inputActions.Command.skillClick.enabled)
        {
            skillController.SkillEnd();
            return;
        }
        MoveOrSetTarget(false);
    }

    private void MoveOrSetTarget(bool isAttactMove)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, monsterOrGround))
        {
            //적인지 땅인지
            if ((int)Mathf.Pow(2, hit.transform.gameObject.layer) == LayerMask.GetMask("Ground"))//땅클릭
            {
                GameObject obj = isAttactMove ? attackToEffect : moveToEffect;
                obj.SetActive(true);
                obj.transform.position = hit.point;
                StartCoroutine(WaitDestroy(obj, 0.5f));
                //effectobj.transform.parent = transform;

                selectedHero.MoveSpot(hit.point, isAttactMove);
            }
            else//적클릭
            {
                selectedHero.isProvoked = true;
                selectedHero.ChangeState(UnitState.Chase);
                selectedHero.SetTarget(hit.transform);
            }
        }
    }

    IEnumerator WaitDestroy(GameObject obj, float sec)
    {
        yield return new WaitForSeconds(sec);
        obj.SetActive(false);
    }
    public void ChangeHeroAfterDie(HeroUnit hero)
    {
        allHeroUnits.Remove(hero);
        if(selectedHero==hero)
        {
            SelectHero(0);
        }
    }
    public void SelectHero(HeroUnit hero)
    {
        int n = allHeroUnits.IndexOf(hero);
        SelectHero(n);
    }
    private void SelectHero(int a)
    {
        if(a<allHeroUnits.Count)
        {
            selectedHero=allHeroUnits[a];
            selectedHero.ShowSelectedState();
            foreach(HeroUnit unit in allHeroUnits)
            {
                if(unit==selectedHero)
                    unit.ShowSelectEffect();
                else
                    unit.HideSelectEffect();
            }
            view.ChageSkillbar(selectedHero);
            skillController.selectedHero = selectedHero;
            skillController.SkillEnd();
        }
    }

}
