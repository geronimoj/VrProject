using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BeamCannon", menuName = "Weapon/Energy/Beam Cannon", order = 1)]
public class BeamCannon : Weapon
{
    public List<LineRenderer> lines = new List<LineRenderer>(0);
    public Health health;
    private int layerMask = 0;
    private int numGuns = 0;

    public override void Fire(Transform gun)
    {
        if (lines.Count == 0)
        {
            var parts = System.Enum.GetValues(typeof(Guns));
            foreach (Guns part in parts)
            {
                if ((gunParts & part) != 0)
                {
                    numGuns++;
                }

            }
        }

        if(lines.Count < numGuns)
        {
            lines.Add(gun.gameObject.GetComponent<LineRenderer>());
            Debug.Log("Adding Line Renderer from gun " + lines.Count);
        }
        

        if(layerMask == 0)
        {
            layerMask = LayerMask.GetMask("Enemy", "Wall", "EnemyProjectile");
        }
        

        if (Physics.Raycast(gun.transform.position, gun.transform.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].SetPosition(1, hit.point);
            }

            if (hit.collider.gameObject.CompareTag("Enemy"))
            {
                if (!health || hit.collider.gameObject != health.gameObject)
                    health = hit.collider.gameObject.GetComponent<Health>();

                health.DoDamage(damage);
            }
        }


        base.Fire(gun);
    }

    public override void WeaponUpdate()
    {
        if (lines.Count != 0)
        {
            if (currentRefire <= 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i].SetPosition(1, lines[i].GetPosition(0));
                }
            }
        }
        
        base.WeaponUpdate();
    }

    public override void OnEquip()
    {
        numGuns = 0;
        lines.Clear();
    }
}
