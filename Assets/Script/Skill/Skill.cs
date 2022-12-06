using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillData
{
    public SkillMode skillMode;
    public float coolTime;
    public float range;
    public float duration;
    public int damage;
    public GameObject skillEffect;
    public int order;
}
public abstract class Skill : MonoBehaviour
{
    public SkillData data;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected GameObject skillPrefab;

    public abstract void InitSetting();

    
}
