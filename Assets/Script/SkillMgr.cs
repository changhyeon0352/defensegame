using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillMgr : MonoBehaviour
{ 

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



    }

    private void OnSkill1(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(KnightSkill.shieldAura, 15));
        StartCoroutine(knight.EnumeratorTimer(knight.shieldAuraCor, 15));
    }

    private void OnSkill2(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(KnightSkill.provoke, 5));
    }

    private void OnSkill3(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(KnightSkill.frenzy, 5));
        StartCoroutine(knight.EnumeratorTimer(knight.frenzyCor, 5));
    }

    private void OnSkill4(InputAction.CallbackContext obj)
    {
        StartCoroutine(PlaySkillOnHero(KnightSkill.finishMove, 5));
    }

    public void UseSkill(int i)
    {

    }
    public IEnumerator PlaySkillOnHero(KnightSkill skill, float sec)
    {
        GameObject obj = Instantiate(knight.skillEffects[(int)skill], GameMgr.Instance.commandMgr.SelectedHero.transform);

        yield return new WaitForSeconds(sec);
        Destroy(obj);
    }
}
