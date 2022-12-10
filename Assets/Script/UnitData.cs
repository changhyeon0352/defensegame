using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Object/Unit Data", order = int.MinValue)]
public class UnitData : ScriptableObject
{
    [SerializeField]
    private UnitType type;
    public UnitType unitType { get { return type; } }
    [SerializeField]
    private Sprite unitPortrait;
    public Sprite UnitPortrait { get { return unitPortrait; } }
    [SerializeField] 
    private string unitName;
    public string UnitName { get { return unitName; } }
    [SerializeField]
    private int hp;
    public int HP { get { return hp; } }
    [SerializeField]
    private int mp;
    public int MP { get { return mp; } }
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    [SerializeField]
    private int atk;
    public int Atk { get { return atk; } }
    [SerializeField]
    private int magicPower;
    public int MagicPower { get { return magicPower; } }
    private int armor;
    public int Armor { get { return armor; } }
    [SerializeField]
    private float attackSpeed;
    public float AttackSpeed { get { return attackSpeed; } }
    [SerializeField]
    private int cost;
    public int Cost { get { return cost; } }
    [SerializeField]private float attackRange;
    public float AttackRange { get { return attackRange; } }
    public GameObject unitPrefab;
}
