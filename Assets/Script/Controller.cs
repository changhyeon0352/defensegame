using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    protected PlayerInput inputActions;
    virtual protected void Awake()
    {
        inputActions = GameMgr.Instance.inputActions;
    }
}
