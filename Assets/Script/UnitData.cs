using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Object/Unit Data", order = int.MinValue)]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private string unitName;
    public string UnitName { get { return unitName; } }
    [SerializeField]
    private int hp;
    public int HP { get { return hp; } }
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField]
    private int atk;
    public int Atk { get { return atk; } }  
    [SerializeField]
    private int attackSpeed;
    public int AttackSpeed { get { return attackSpeed; } }
    [SerializeField]
    private int cost;
    public int Cost { get { return cost; } }
    public GameObject unitPrefab;
}
