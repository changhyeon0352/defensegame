using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KnightSkill{shieldAura,provoke,frenzy,finishMove }
public class Knight : MonoBehaviour
{
    PlayerInput inputActions;
    private void Awake()
    {
        inputActions = GameMgr.Instance.inputActions;
    }
    private void OnEnable()
    {
        inputActions.Command.SkillButton1.performed += UseSkill1;
        
    }

    private void UseSkill1(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UseSkill();
    }

    public GameObject[] skillEffects;
    
    public IEnumerator PlaySkillOnHero(KnightSkill skill,float sec)
    {
        GameObject obj = Instantiate(skillEffects[(int)skill],transform);

        yield return new WaitForSeconds(sec);
    }
    public void  UseSkill()
    {
        StartCoroutine(PlaySkillOnHero(KnightSkill.shieldAura,5));
    }
}
