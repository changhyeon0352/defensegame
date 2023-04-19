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
    soldier_Melee,
    soldier_Range,
    hero,
    mon1,mon2
}
[System.Flags]
public enum SoldierSkill:byte
{
    None        = 0b_0000_0000,
    ReturnToLine  = 0b_0000_0001,
    Charge      = 0b_0000_0010,
    ShootSpot   = 0b_0000_0100,
    ShootEnemy  = 0b_0000_1000,
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
    findTarget,
    targeting
    
}
public enum Phase
{
    town,
    selectHero,
    Deployment,
    defense,
    result

}
