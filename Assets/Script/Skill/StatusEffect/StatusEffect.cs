using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect 
{
    protected bool isEnd = false;
    protected Unit unit;
    protected float duration;
    [SerializeField]
    protected ObjectPooling psPool;
    public bool IsEnd { get { return isEnd; } } 
    public ObjectPooling PsPool { get { return psPool; } }
    public float Duration { get { return duration; } }
    public StatusEffect(Unit unit, float duration)
    {
        this.unit = unit;
        this.duration = duration;
    }
    public abstract void StartEffect();

    public abstract void EndEffect();

    public abstract void UpdateEffect();
}
