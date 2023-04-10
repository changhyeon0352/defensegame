using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Mage : HeroUnit
{
    [SerializeField] GameObject energyBoltPrefab;
    [SerializeField] Transform tipOfStaff;
    private bool isStopSkill = false;
    public bool IsStopSkill { get => isStopSkill; set { isStopSkill = value; navMesh.SetDestination(goalTr.position); } }
    protected override void Update()
    {
        if (isStopSkill)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, LayerMask.GetMask("Ground")))
            {
                Vector3 mouseDir = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;
                float dot = Vector3.Dot(Vector3.Cross(transform.forward, mouseDir).normalized, Vector3.up);
                transform.Rotate(transform.up * dot * 0.1f);
                return;
            }
        }
        base.Update();
    }
    public override void Attack()
    {
        if(targetCol.enabled)
        {
            GameObject obj = Instantiate(energyBoltPrefab, tipOfStaff.position, Quaternion.identity);
            obj.GetComponent<EnergyBolt>().SetTargetAndDamage(targetCol.transform.transform, currentStat.attackPoint);
        }
    }
}
