using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle=0,
    Move,
    Chase,
    Attack,
    Dead
}
public enum UnitShader
{
    normalShader = 0,
    transparentShader
}
public enum UnitType
{
    none=0,
    soldier,
    hero
}
[System.Flags]
public enum SkillAvailable:byte
{
    None        = 0b0000_0000,
    MoveToSpot  = 0b0000_0001,
    Charge      = 0b0000_0010,
    Shoot       = 0b_0000_0100
        //
}
