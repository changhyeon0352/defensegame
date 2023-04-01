using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    Unit model;
    UnitView view;
    public Action OnHPChanged;

    private void Awake()
    {
        model = GetComponent<Unit>();
        view = GetComponent<UnitView>();
        OnHPChanged += view.UpdateHPbar;
    }
}
