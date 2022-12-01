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
    Monster,
    soldier_Melee,
    soldier_Range,
    hero
}
[System.Flags]
public enum BasicSkills:byte
{
    None        = 0b_0000_0000,
    MoveToSpot  = 0b_0000_0001,
    Charge      = 0b_0000_0010,
    Shoot       = 0b_0000_0100,
    AttackMove  = 0b_0000_1000
    //
}
public enum HeroClass
{
    Knight,
    Mage
}

public enum CursorType
{
    Default=0,
    Sword,
    targeting,
    findTarget
}
public enum Phase
{
    town,
    selectHero,
    setting,
    defense
}
