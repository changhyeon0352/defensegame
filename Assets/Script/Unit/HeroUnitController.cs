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


    bool isSelected=false;
    [SerializeField] GameObject moveToPrefabs;
    LayerMask monsterOrGround;

    public Transform SelectedHeroTr { get => selectedHero.transform; }

    override protected  void Awake()
    {
        base.Awake();
        monsterOrGround = LayerMask.GetMask("Monster") | LayerMask.GetMask("Ground");
        view = GetComponent<HeroUnitView>();
        
    }
    private void OnEnable()
    {
        allHeroUnits.AddRange(FindObjectsOfType<HeroUnit>());
        inputActions.Command.ChangeHero.performed += OnChangeHero;
        inputActions.Command.MoveorSetTarget.performed += OnMoveOrSetTarget;
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
        if (GameMgr.Instance.skillController.IsUsingSkill)
        {
            GameMgr.Instance.skillController.SkillEnd();
            return;
        }
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
                GameObject effectobj = Instantiate(moveToPrefabs, hit.point, Quaternion.identity);
                effectobj.transform.parent = transform;
                StartCoroutine(WaitDestroy(effectobj, 0.5f));
                selectedHero.MoveSpots(hit.point);
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
        Destroy(obj);
    }
    public void ChangeHeroAfterDie(HeroUnit hero)
    {
        allHeroUnits.Remove(hero);
        if(selectedHero==hero)
        {
            SelectHero(0);
        }
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
            GameMgr.Instance.skillController.selectedHero = selectedHero;
        }
    }
}
